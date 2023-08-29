using Gamanet.C4.SimpleInterfaces;
using Gamanet.Utilities.Themes.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using WpfApp1.Base;
using WpfApp1.Hosters;

namespace WpfApp1.Models
{
    public class RowSimpleClientDataProvider
    {
        private _AppContext _appContext;
        private Stopwatch _stopwatch = new Stopwatch();

        private DiagnosticsHoster _diagnosticsHost => _appContext.DiagnosticsHost;
        private Dictionary<Guid, int> OrderIndexDict => _appContext.EntityDefinitionRepo.EntityOrderIndexValues;
        private Dictionary<Guid, IconEntity> IconMapper => _appContext.EntityDefinitionRepo.IconEntityMapper;
        private Dictionary<int, Guid> ColumnToTypeMapper => _appContext.ColumnDefinitionRepo.ColumnToTypeMapper;
        private RowRepository RowRepo => _appContext.RowRepo;
        private SmartCollection<RowEntity> Rows => RowRepo.Rows;
        private PermissionMatrixEntity _matrix => RowRepo._matrix;
        private Guid RightSideTypeId => _appContext.SessionContext.RightSideTypeId;

        public RowSimpleClientDataProvider(_AppContext appContext)
        {
            _appContext = appContext;
        }

        public bool FillInRows()
        {
            var stopwwatch = new Stopwatch();
            var dataProvider = new SimpleClientDataProvider(_appContext);
            var simpleEntities = dataProvider.GetAllEntities();
            if (simpleEntities.Count == 0) return true;

            _matrix.Arr = new byte[simpleEntities.Count, ColumnToTypeMapper.Count];

            stopwwatch.Start();

            var rowsDict = ConvertSimpleEntitiesToRows(simpleEntities, ref _matrix.Arr);

            SortDictionaryAndMakeList(rowsDict);

            stopwwatch.Stop();
            _diagnosticsHost.SendMessage($"Converting and Sorting took {stopwwatch.ElapsedMilliseconds} ms.");

            return true;
        }

        private Dictionary<Guid, RowEntity> ConvertSimpleEntitiesToRows(List<SimpleEntity> simpleEntities, ref byte[,] matrix)
        {
            var stopwwatch = new Stopwatch();
            stopwwatch.Start();

            var rowEntities = new Dictionary<Guid, RowEntity>(simpleEntities.Count);

            var isDeviceOrRegion = RightSideTypeId.Equals(EntityType.Devices)
                                   || RightSideTypeId.Equals(EntityType.Regions);

            if (isDeviceOrRegion)
            {
                ConvertForDeviceRegionEntitiesToRows(rowEntities, simpleEntities, ref matrix);
            }
            else
            {
                ConvertForAnyEntitiesToRows(rowEntities, simpleEntities, ref matrix);
            }

            _diagnosticsHost.SendMessage($"Conversion took {stopwwatch.ElapsedMilliseconds} ms.");
            stopwwatch.Stop();

            return rowEntities;
        }

        private bool ConvertForAnyEntitiesToRows(Dictionary<Guid, RowEntity> rowEntities,
                                                 List<SimpleEntity> simpleEntities,
                                                 ref byte[,] matrix)
        {
            for (int i = 0; i < simpleEntities.Count; i++)
            {
                var entity = simpleEntities[i];
                var virtualId = GetVirtualId(entity);
                OrderIndexDict.TryGetValue(entity.CategoryId, out var orderIndex);
                IconMapper.TryGetValue(entity.CategoryId, out var iconInfo);
                int iconIndex = iconInfo?.IconIndex ?? 0;

                var row = new RowEntity(entity.Id,
                    entity.ParentId,
                    virtualId,
                    entity.CategoryId,
                    entity.CategoryId,
                    entity.Name,
                    orderIndex,
                    iconIndex,
                    ref matrix);

                row.CopyFrom(entity);
                rowEntities.Add(row.VirtualId, row);
            }

            return true;
        }

        private bool ConvertForDeviceRegionEntitiesToRows(Dictionary<Guid, RowEntity> rowEntities,
                                                          List<SimpleEntity> simpleEntities,
                                                          ref byte[,] matrix)
        {
            for (int i = 0; i < simpleEntities.Count; i++)
            {
                var entity = simpleEntities[i];
                var virtualId = GetVirtualId(entity);
                var displayCategoryId = GetDisplayCategoryId(entity);

                OrderIndexDict.TryGetValue(entity.CategoryId, out var orderIndex);
                IconMapper.TryGetValue(displayCategoryId, out var iconInfo);
                int iconIndex = iconInfo?.IconIndex ?? 0;

                var row = new RowEntity(entity.Id,
                    entity.ParentId,
                    virtualId,
                    entity.CategoryId,
                    displayCategoryId,
                    entity.Name,
                    orderIndex,
                    iconIndex,
                    ref matrix);

                row.CopyFrom(entity);
                rowEntities.Add(row.VirtualId, row);
            }

            return true;
        }

        private Guid GetVirtualId(SimpleEntity entity)
        {
            if (entity.Data.ContainsKey("GroupRefId"))
            {
                return Guid.NewGuid();
            }

            return entity.Id;
        }

        private Guid GetDisplayCategoryId(SimpleEntity entity)
        {
            var displayCategoryId = entity.Data.GetValueOrDefault<Guid>("DisplayCategoryId");
            if (displayCategoryId.Equals(Guid.Empty))
            {
                displayCategoryId = entity.Data.GetValueOrDefault<Guid>("DeviceCategoryId");
            }

            if (displayCategoryId.Equals(Guid.Empty))
            {
                displayCategoryId = entity.CategoryId;
            }

            return displayCategoryId;
        }

        public bool SortDictionaryAndMakeList(Dictionary<Guid, RowEntity> rowsDict)
        {
            _stopwatch.Reset();
            _stopwatch.Start();

            var parentLookup = rowsDict.Values.ToLookup(x => x.ParentId);
            if (!rowsDict.TryGetValue(EntityRoot.PersonSuperRoot, out var root))
            {
                return true;
            }

            root.Level = 0;
            root.Index = 0;
            root.IsExpanded = true;
            root.IsVisible = true;

            Rows.Add(root);
            RawToRows(parentLookup, root);

            _stopwatch.Stop();
            _diagnosticsHost.SendMessage($"Sorting took {_stopwatch.ElapsedMilliseconds} ms.");

            return true;
        }

        private bool RawToRows(
               ILookup<Guid, RowEntity> parentLookup,
               RowEntity parent)
        {
            // OrdinalIgnoreCase
            // InvariantCultureIgnoreCase
            // CurrentCultureIgnoreCase
            var values = parentLookup[parent.Id];

            var orderedCollection =
                values.OrderBy(i => i.OrderIndex)
                      .ThenBy(i => i.Name, StringComparer.OrdinalIgnoreCase);

            foreach (var row in orderedCollection)
            {
                Rows.Add(row);

                parent.IsExpandable = true;

                row.Level = parent.Level + 1;
                row.IsExpanded = false;
                row.IsVisible = parent.IsExpanded;
                row.Index = Rows.Count - 1;

                RawToRows(parentLookup, row);
            }

            return true;
        }
    }
}