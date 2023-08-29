using Gamanet.C4.SimpleInterfaces;
using System;
using System.Collections.Generic;
using WpfApp1.Base;

namespace WpfApp1.Models
{
    public class PermissionRepository
    {
        public PermissionRepository(_AppContext context)
        {
            _appContext = context;
        }

        private _AppContext _appContext;

        private bool IsTrustee => _appContext.SessionContext.IsTrustee;

        private Dictionary<Guid, ColumnEntity> ColumnDefinitions
           => _appContext.ColumnDefinitionRepo.ColumnDefinitions;

        public Dictionary<Guid, List<PermissionV1>> Permissions { get; }
          = new Dictionary<Guid, List<PermissionV1>>();
        public Dictionary<TripleKey, PermissionV1> PermissionsByTripleKey { get; }
          = new Dictionary<TripleKey, PermissionV1>();

        private Dictionary<Guid, PermissionV1> CreatePermissions { get; }
          = new Dictionary<Guid, PermissionV1>();
        private Dictionary<Guid, PermissionV1> UpdatePemissions { get; }
          = new Dictionary<Guid, PermissionV1>();
        private Dictionary<Guid, PermissionV1> DeletePermissions { get; }
          = new Dictionary<Guid, PermissionV1>();

        public bool LoadData(Guid contextId)
        {
            ClearCashData();

            var typeIds = ColumnDefinitions.Keys.ToSimpleCollection();

            var simpleClientDataProvider = new SimpleClientDataProvider(_appContext);
            List<PermissionV1> permissions = new List<PermissionV1>();
            if (IsTrustee)
            {
                permissions =
                    simpleClientDataProvider.GetPermissionsByPermissionTypesForTrustee(typeIds,
                                                                                       contextId,
                                                                                       _appContext.SessionContext.RightSideRootId,
                                                                                       _appContext.SessionContext.LeftSideTypeId,
                                                                                       _appContext.SessionContext.RightSideTypeId);
            }
            else
            {
                permissions =
                    simpleClientDataProvider.GetPermissionsByPermissionTypesForObject(typeIds,
                                                                                      _appContext.SessionContext.RightSideRootId,
                                                                                      contextId,
                                                                                      _appContext.SessionContext.RightSideTypeId,
                                                                                      _appContext.SessionContext.LeftSideTypeId);
            }

            foreach (var permission in permissions)
            {
                PermissionsByTripleKey[new TripleKey(permission.PersonId, permission.EntityId, permission.PermissionType)] = permission;
                if (Permissions.TryGetValue(permission.PermissionType, out var collection))
                {
                    collection.Add(permission);
                }
                else
                {
                    Permissions[permission.PermissionType] = new List<PermissionV1> { permission };
                }
            }

            return true;
        }

        public bool ClearCashData()
        {
            Permissions.Clear();
            PermissionsByTripleKey.Clear();

            CreatePermissions.Clear();
            UpdatePemissions.Clear();
            DeletePermissions.Clear();

            return true;
        }

        public void AddPermission(PermissionV1 simplePermission)
        {
            PermissionsByTripleKey[new TripleKey(simplePermission.PersonId, simplePermission.EntityId, simplePermission.PermissionType)] = simplePermission;
            if (Permissions.TryGetValue(simplePermission.PermissionType, out var permissions))
            {
                permissions.Add(simplePermission);
            }
            else
            {
                Permissions[simplePermission.PermissionType] = new List<PermissionV1> { simplePermission };
            }

            CreatePermissions.Add(simplePermission.Id, simplePermission);
        }

        public void UpdatePemission(PermissionV1 permission)
        {
            // chcek if it's not present in Created collection
            if (!CreatePermissions.TryGetValue(permission.Id, out var permissions))
            {
                UpdatePemissions[permission.Id] = permission;
            }
        }

        public void DeletePemission(PermissionV1 permission)
        {
            if (Permissions.TryGetValue(permission.PermissionType, out var perms))
            {
                perms.Remove(permission);
                PermissionsByTripleKey.Remove(new TripleKey(permission.PersonId, permission.EntityId, permission.PermissionType));
            }

            // check is it's not present in Created collection
            if (CreatePermissions.TryGetValue(permission.Id, out var createdPermission))
            {
                CreatePermissions.Remove(permission.Id);
            }
            else
            {
                DeletePermissions[permission.Id] = permission;
            }

            // check is it's not present in Updated collection
            if (UpdatePemissions.TryGetValue(permission.Id, out var updatedPermission))
            {
                UpdatePemissions.Remove(permission.Id);
            }
        }

        public bool SaveData()
        {
            var simpleClientDataProvider = new SimpleClientDataProvider(_appContext);

            foreach (var permission in DeletePermissions)
            {
                if (!simpleClientDataProvider.DeletePermission(permission.Value))
                {
                    return false;
                }
            }

            foreach (var permission in CreatePermissions)
            {
                if (!simpleClientDataProvider.CreatePermission(permission.Value))
                {
                    return false;
                }
            }

            foreach (var permission in UpdatePemissions)
            {
                if (!simpleClientDataProvider.UpdatePermission(permission.Value))
                {
                    return false;
                }
            }

            CreatePermissions.Clear();
            UpdatePemissions.Clear();
            DeletePermissions.Clear();

            return true;
        }
    }
}
