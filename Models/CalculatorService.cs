using Gamanet.C4.Client.Permissions.BusinessLogic.Base.Enums;
using Gamanet.C4.PermissionCalculator;
using Gamanet.C4.SimpleInterfaces;
using Gamanet.Utilities.Themes.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using WpfApp1.Base;
using WpfApp1.Base.Enums;
using WpfApp1.DataSources;
using WpfApp1.Hosters;

namespace WpfApp1.Models
{
    public class CalculatorService
    {
        private _AppContext _appContext;
        private DiagnosticsHoster _diagnosticsHost => _appContext.DiagnosticsHost;
        private CalculatorHoster CalculatorHost => _appContext.CalculatorHost;
        private PermissionRepository PermissionRepo => _appContext.PermissionRepo;
        private SmartCollection<RowEntity> Rows => _appContext.RowRepo.Rows;
        private bool IsTrustee => _appContext.SessionContext.IsTrustee;
        private Dictionary<int, Guid> ColumnToTypeMapper => _appContext.ColumnDefinitionRepo.ColumnToTypeMapper;
        private Dictionary<Guid, int> TypeToColumnMapper => _appContext.ColumnDefinitionRepo.TypeToColumnMapper;
        private Guid LeftSideTypeId => _appContext.SessionContext.LeftSideTypeId;
        private Guid RightSideTypeId => _appContext.SessionContext.RightSideTypeId;

        public CalculatorService(_AppContext appContext)
        {
            _appContext = appContext;
        }

        public bool Activate(Guid contextId)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            var entityPermissions = CalculatorHost.Calculate(contextId);

            stopwatch.Stop();
            _diagnosticsHost.SendMessage($"Calculation time: {stopwatch.ElapsedMilliseconds} ms");
            stopwatch.Reset();
            stopwatch.Start();

            // then update cells
            UpdateAllCells(entityPermissions);

            stopwatch.Stop();
            _diagnosticsHost.SendMessage($"Full time for updating all Cells: {stopwatch.ElapsedMilliseconds} ms");

            return true;
        }

        public bool CalculateForCell(Guid contextId, int row, int column)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            var selectedRow = Rows[row];
            var currentPermissionState = selectedRow[column];
            var nextPermissionState = GetNextPermissionState(currentPermissionState);
            MakePermissionDataChanges(contextId, selectedRow.Id, currentPermissionState, nextPermissionState, column);

            var permissionTypeId = _appContext.ColumnDefinitionRepo.ColumnToTypeMapper[column];

            stopwatch.Stop();
            _diagnosticsHost.SendMessage($"Data preparation time for calculating for Cell: {stopwatch.ElapsedMilliseconds} ms");
            stopwatch.Reset();

            var entityPermissions = CalculatorHost.CalculateForColumn(contextId, selectedRow.Id, permissionTypeId);

            stopwatch.Start();

            UpdateColumn(entityPermissions, column);

            stopwatch.Stop();
            _diagnosticsHost.SendMessage($"Full Time Update for Column: {stopwatch.ElapsedMilliseconds} ms");

            return true;
        }

        public bool ChangeCellToTargetState(Guid contextId, int row, int column, enPermissionStateCommand selectedState)
        {
            var selectedRow = Rows[row];
            var currentPermissionState = selectedRow[column];
            var nextPermissionState = (byte)selectedState;

            MakePermissionDataChanges(contextId, selectedRow.Id, currentPermissionState, nextPermissionState, column);

            var permissionTypeId = _appContext.ColumnDefinitionRepo.ColumnToTypeMapper[column];
            var entityPermissions = CalculatorHost.CalculateForColumn(contextId, selectedRow.Id, permissionTypeId);
            UpdateColumn(entityPermissions, column);

            return true;
        }

        public bool ContextChanged(Guid contextId)
        {
            var calculatorDataProvider = new CalculatorDataProvider(_appContext);
            calculatorDataProvider.UpdateDataForNewContext(contextId);

            Activate(contextId);

            return true;
        }

        private void UpdateColumn(List<EntityPermission> entityPermissions, 
                                 int columnIndex)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var columnPermissions = entityPermissions.ToDictionary(key => key.EntityId, value => value);
            stopwatch.Stop();
            _diagnosticsHost.SendMessage($"Grouping by EntityId: {stopwatch.ElapsedMilliseconds} ms");

            stopwatch.Reset();
            stopwatch.Start();

            for (int i = 0; i < Rows.Count; i++)
            {
                var row = Rows[i];
                if (columnPermissions.TryGetValue(row.Id, out EntityPermission permission))
                {
                    row[columnIndex] = GetStateFromEntityPermission(permission);
                }
            }

            stopwatch.Stop();
            _diagnosticsHost.SendMessage($"Updating Cells in loop: {stopwatch.ElapsedMilliseconds} ms");
        }

        private void UpdateAllCells(List<EntityPermission> entityPermissions)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            var groupedPermissionsByType = entityPermissions.GroupBy(i => i.PermissionType)
                                                            .ToDictionary(key => key.Key, 
                                                                          value => value.ToDictionary(i => i.EntityId, j => j));

            stopwatch.Stop();
            _diagnosticsHost.SendMessage($"Grouping by PermissionType result from Calculator: {stopwatch.ElapsedMilliseconds} ms");
            stopwatch.Reset();
            stopwatch.Start();

            int columnIndex = -1;
            // go through different types
            for (int i = 0; i < groupedPermissionsByType.Count; i++)
            {
                columnIndex++;
                var groupedPermission = groupedPermissionsByType.ElementAt(i);

                for (int itemIndex = 0; itemIndex < Rows.Count; itemIndex++)
                {
                    var row = Rows[itemIndex];
                    if (groupedPermission.Value.TryGetValue(row.Id, out EntityPermission permission))
                    {
                        row[columnIndex] = GetStateFromEntityPermission(permission);
                    }
                }
            }

            stopwatch.Stop();
            _diagnosticsHost.SendMessage($"Update for all Cells: {stopwatch.ElapsedMilliseconds} ms");
        }

        private void MakePermissionDataChanges(Guid contextId,
                                               Guid clieckedRowId,
                                               byte currentPermissionState, 
                                               byte nextPermissionState,   
                                               int column)
        {
            var currentState = (enPermissionState)currentPermissionState;
            var nextState = (enPermissionState)nextPermissionState;

            if (currentState.Equals(enPermissionState.DeniedInherited)
                || currentState.Equals(enPermissionState.AllowedInherited))
            {
                // create permission
                PermissionV1 permission = null;
                if (IsTrustee)
                {
                    permission = new PermissionV1(Guid.NewGuid(), ColumnToTypeMapper[column], contextId, clieckedRowId, RightSideTypeId);
                }
                else
                {
                    permission = new PermissionV1(Guid.NewGuid(), ColumnToTypeMapper[column], clieckedRowId,  contextId, LeftSideTypeId);
                }

                FromStateToPermission(permission, nextState);
                PermissionRepo.AddPermission(permission);
            }
            else if (nextState.Equals(enPermissionState.DeniedInherited))
            {
                // delete permission
                var permissionType = ColumnToTypeMapper[column];
                PermissionV1 permission = null;
                if (IsTrustee)
                {
                    PermissionRepo.PermissionsByTripleKey.TryGetValue(new TripleKey(contextId, clieckedRowId, permissionType), out permission);
                }
                else
                {
                    PermissionRepo.PermissionsByTripleKey.TryGetValue(new TripleKey(clieckedRowId, contextId, permissionType), out permission);
                }

                if (permission != null)
                {
                    PermissionRepo.DeletePemission(permission);
                }
            }
            else
            {
                // update permission
                var permissionType = ColumnToTypeMapper[column];
                PermissionV1 permission = null;
                if (IsTrustee)
                {
                    PermissionRepo.PermissionsByTripleKey.TryGetValue(new TripleKey(contextId, clieckedRowId, permissionType), out permission);
                }
                else
                {
                    PermissionRepo.PermissionsByTripleKey.TryGetValue(new TripleKey(clieckedRowId, contextId, permissionType), out permission);
                }

                if (permission != null)
                {
                    FromStateToPermission(permission, nextState);
                    PermissionRepo.UpdatePemission(permission);
                }
            }
        }

        public byte GetNextPermissionState(byte index)
        {
            var state = (enPermissionState)index;

            if (state.Equals(enPermissionState.AllowedInherited)
                || state.Equals(enPermissionState.DeniedInherited)
                || state.Equals(enPermissionState.DeniedDirect))
            {
                return (byte)enPermissionState.AllowedWithInheritance;
            }
            else if (state.Equals(enPermissionState.AllowedWithInheritance)
                  || state.Equals(enPermissionState.AllowedDirect))
            {
                return (byte)enPermissionState.DeniedWithInheritance;
            }

            return (byte)enPermissionState.DeniedInherited;
        }

        private byte GetStateFromEntityPermission(EntityPermission entity)
        {
            if (entity.IsAllowed && entity.IsDirect && entity.IsInheritable)
            {
                return (byte)enPermissionState.AllowedWithInheritance;
            }
            else if (entity.IsAllowed && entity.IsDirect && !entity.IsInheritable)
            {
                return (byte)enPermissionState.AllowedDirect;
            }
            else if (entity.IsAllowed && !entity.IsDirect)
            {
                return (byte)enPermissionState.AllowedInherited;
            }
            else if (!entity.IsAllowed && entity.IsDirect && entity.IsInheritable)
            {
                return (byte)enPermissionState.DeniedWithInheritance;
            }
            else if (!entity.IsAllowed && entity.IsDirect && !entity.IsInheritable)
            {
                return (byte)enPermissionState.DeniedDirect;
            }
            else if (!entity.IsAllowed && !entity.IsDirect)
            {
                return (byte)enPermissionState.DeniedInherited;
            }

            return (int)enPermissionState.DeniedInherited;
        }
        
        private bool FromStateToPermission(PermissionV1 permission, enPermissionState state)
        {
            if (state.Equals(enPermissionState.AllowedWithInheritance))
            {
                permission.Status = PermissionStatus.Allowed;
                permission.Inherited = true;
            }
            else if (state.Equals(enPermissionState.AllowedDirect))
            {
                permission.Status = PermissionStatus.Allowed;
                permission.Inherited = false;
            }
            else if (state.Equals(enPermissionState.DeniedWithInheritance))
            {
                permission.Status = PermissionStatus.Denied;
                permission.Inherited = true;
            }
            else if (state.Equals(enPermissionState.DeniedDirect))
            {
                permission.Status = PermissionStatus.Denied;
                permission.Inherited = false;
            }

            return true;
        }
    }
}