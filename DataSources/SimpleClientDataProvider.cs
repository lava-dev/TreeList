using Gamanet.C4.SimpleInterfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using WpfApp1.Helpers;
using WpfApp1.Hosters;
using WpfApp1.Models;

namespace WpfApp1
{
    public class SimpleClientDataProvider
    {
        private _AppContext _appContext;
        private Stopwatch _stopwatch = new Stopwatch();

        private Guid PermissionTypesByTypesForTrusteeFilterId = Guid.Parse("C9EA3905-3A64-4079-A32F-9BFD57CCC6C9");
        private Guid PermissionTypesByTypesForObjectsFilterId = Guid.Parse("465372C7-A109-4487-A83C-379FAB139570");

        private DiagnosticsHoster _diagnosticsHost => _appContext.DiagnosticsHost;
        private ISimpleClientV2 SimpleClientHost => _appContext.SimpleClientHost.SimpleClient;

        public SimpleClientDataProvider(_AppContext appContext)
        {
            _appContext = appContext;
        }

        public IEnumerable<SimpleEntity> GetAllParentsForSelectedEntity(Guid entityId, Guid entityTypeId)
        {
            //IEnumerable<SimpleEntity> entities =
            //    SimpleClientHost.Permissions.GetParentsForCalculation(entityId, entityTypeId);
            //return entities;
            return GetAllEntities();
        }

        public IEnumerable<SimpleEntity> GetChildrenWithParentsForCalculating(Guid entityId, Guid entityTypeId)
        {
            //IEnumerable<SimpleEntity> entities = 
            //    SimpleClientHost.Permissions.GetChildrenWithParentsForCalculation(entityId, entityTypeId);
            //return entities;
            return GetAllEntities();
        }

        public List<ColumnEntity> GetTrusteePermissionDefinitions(Guid trusteeTypeId, Guid entityTypeId, string panel = null)
        {
            var permDefs = new List<ColumnEntity>();

            var definitions = SimpleClientHost.Permissions.GetDefinitionsByTrusteeType(trusteeTypeId)
                                                          .Where(i => i.EntityTypeId.Equals(entityTypeId));

            var defTypes = definitions.SelectMany(i => i.PermissionTypes)
                                      .Where(j => j.PanelName == panel)
                                      .OrderBy(i => i.OrderIndex);

            foreach (var defType in defTypes)
            {
                var permDef = new ColumnEntity();
                permDef.CopyFrom(defType);
                permDefs.Add(permDef);
            }

            return permDefs;
        }

        public List<ColumnEntity> GetEntityPermissionDefinitions(Guid entityTypeId, Guid trusteeTypeId, string panel = null)
        {
            var permDefs = new List<ColumnEntity>();
            var definitions = SimpleClientHost.Permissions.GetDefinitionsByEntityType(entityTypeId)
                                                          .Where(i => i.TrusteeTypeId.Equals(trusteeTypeId));

            var defTypes = definitions.SelectMany(i => i.PermissionTypes)
                                      .Where(i => i.PanelName == panel)
                                      .OrderBy(i => i.OrderIndex);

            foreach (var defType in defTypes)
            {
                var permDef = new ColumnEntity();
                permDef.CopyFrom(defType);
                permDefs.Add(permDef);
            }

            return permDefs;
        }

        public List<PermissionV1> GetPermissionsByPermissionTypesForTrustee(SimpleCollection<Guid> permissionTypeIds,
                                                                        Guid trusteeId,
                                                                        Guid objectId,
                                                                        Guid trusteeTypeId,
                                                                        Guid objectTypeId)
        {
            var args = new Dictionary<string, object>
            {
                { "TrusteeId", trusteeId },
                { "TrusteeTypeId", trusteeTypeId },
                { "EntityId", objectId },
                { "EntityTypeId", objectTypeId },
                { "PermissionTypeIds", permissionTypeIds }
            };

            var permissions = SimpleClientHost.Permissions.Filter(PermissionTypesByTypesForTrusteeFilterId, args);
            //var permissions = new List<PermissionV1>()
            //{
            //    new PermissionV1(Guid.NewGuid(),
            //                     PermissionType.Create,
            //                     EntityRoot.PersonSuperRoot,
            //                     _itemWithPermissions.Id,
            //                     EntityType.Persons)
            //    {
            //        Status = PermissionStatus.Allowed,
            //        Inherited = true
            //    },
            //    new PermissionV1(Guid.NewGuid(),
            //                     PermissionType.Read,
            //                     EntityRoot.PersonSuperRoot,
            //                     _itemWithPermissions.Id,
            //                     EntityType.Persons)
            //    {
            //        Status = PermissionStatus.Allowed,
            //        Inherited = true
            //    },
            //    new PermissionV1(Guid.NewGuid(),
            //                     PermissionType.Delete,
            //                     EntityRoot.PersonSuperRoot,
            //                     _itemWithPermissions.Id,
            //                     EntityType.Persons)
            //    {
            //        Status = PermissionStatus.Allowed,
            //        Inherited = true
            //    },
            //    new PermissionV1(Guid.NewGuid(),
            //                     PermissionType.ModifyPermissions,
            //                     EntityRoot.PersonSuperRoot,
            //                     _itemWithPermissions.Id,
            //                     EntityType.Persons)
            //    {
            //        Status = PermissionStatus.Allowed,
            //        Inherited = true
            //    },
            //    new PermissionV1(Guid.NewGuid(),
            //                     PermissionType.DefineAccessEntry,
            //                     EntityRoot.PersonSuperRoot,
            //                     _itemWithPermissions.Id,
            //                     EntityType.Persons)
            //    {
            //        Status = PermissionStatus.Allowed,
            //        Inherited = true
            //    },
            //    new PermissionV1(Guid.NewGuid(),
            //                     PermissionType.Write,
            //                     EntityRoot.PersonSuperRoot,
            //                     _itemWithPermissions.Id,
            //                     EntityType.Persons)
            //    {
            //        Status = PermissionStatus.Allowed,
            //        Inherited = true
            //    },
            //};

            return permissions.ToList();
        }

        public List<PermissionV1> GetPermissionsByPermissionTypesForObject(SimpleCollection<Guid> permissionTypeIds,
                                                                        Guid trusteeId,
                                                                        Guid objectId,
                                                                        Guid trusteeTypeId,
                                                                        Guid objectTypeId)
        {
            var args = new Dictionary<string, object>
            {
                { "TrusteeId", trusteeId },
                { "TrusteeTypeId", trusteeTypeId },
                { "EntityId", objectId },
                { "EntityTypeId", objectTypeId },
                { "PermissionTypeIds", permissionTypeIds }
            };

            //var permissions = SimpleClientHost.Permissions.Filter(PermissionTypesByTypesForObjectsFilterId, args);
            var permissions = new List<PermissionV1>();

            return permissions;
        }

        public IEnumerable<CalendarV1> GetAllCalendars()
        {
            IEnumerable<CalendarV1> calendars = SimpleClientHost.Calendars.GetAll();
            return calendars;
        }

        public bool CreatePermission(PermissionV1 permission)
        {
            try
            {
                SimpleClientHost.Permissions.Create(permission);
            }
            catch (ValidationException ex)
            {
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }

            return true;
        }

        public bool UpdatePermission(PermissionV1 permission)
        {
            try
            {
                SimpleClientHost.Permissions.Update(permission);
            }
            catch (ValidationException ex)
            {
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }

            return true;
        }

        public bool DeletePermission(PermissionV1 permission)
        {
            try
            {
                SimpleClientHost.Permissions.Delete(permission);
            }
            catch (ValidationException ex)
            {
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }

            return true;
        }

        private static List<SimpleEntity> _entities = new List<SimpleEntity>();
        private static SimpleEntity _itemWithPermissions;

        public List<SimpleEntity> GetAllEntities()
        {
            if (_entities.Count == 0)
            {
                _entities = SimpleClientHost.Persons.GetAll(EntityRoot.PersonSuperRoot, Gamanet.C4.SimpleInterfaces.Properties.None, false).Cast<SimpleEntity>().ToList();
                //var generate = new Generate();
                //_entities = generate.Tree(510, 5, 4);
                _itemWithPermissions = _entities[1];
            }

            return _entities;
        }

        public ISimpleCollection<EntityDefinitionV2> GetAllDefinitions()
        {
            return SimpleClientHost.EntityDefinitions.GetAll();
        }
    }
}