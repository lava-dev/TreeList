using System.Collections.Generic;
using System;

namespace WpfApp1.Models
{
    public class ColumnDefinitionRepository
    {
        public ColumnDefinitionRepository(_AppContext appContext)
        {
            _appContext = appContext;
        }

        private _AppContext _appContext;

        private string Panel => _appContext.SessionContext.PanelName;
        private bool IsTrustee => _appContext.SessionContext.IsTrustee;
        private Guid LeftSideTypeId => _appContext.SessionContext.LeftSideTypeId;
        private Guid RightSideTypeId => _appContext.SessionContext.RightSideTypeId;

        public Dictionary<Guid, ColumnEntity> ColumnDefinitions { get; set; }
         = new Dictionary<Guid, ColumnEntity>();

        public Dictionary<int, Guid> ColumnToTypeMapper { get; set; }
         = new Dictionary<int, Guid>();
        public Dictionary<Guid, int> TypeToColumnMapper { get; set; }
         = new Dictionary<Guid, int>();

        public bool LoadData()
        {
            ColumnDefinitions.Clear(); 
            ColumnToTypeMapper.Clear(); 
            TypeToColumnMapper.Clear();

            var simpleClientDataProvider = new SimpleClientDataProvider(_appContext);
            var results = new List<ColumnEntity>();

            if (IsTrustee)
            {
                results =
                    simpleClientDataProvider.GetTrusteePermissionDefinitions(LeftSideTypeId, RightSideTypeId, Panel);
            }
            else
            {
                results = 
                    simpleClientDataProvider.GetEntityPermissionDefinitions(LeftSideTypeId, RightSideTypeId, Panel);
            }

            for (int index = 0; index < results.Count; index++)
            {
                var result = results[index];
                ColumnDefinitions.Add(result.TypeId, result);
                ColumnToTypeMapper.Add(index, result.TypeId);
                TypeToColumnMapper.Add(result.TypeId, index);
            }

            return true;
        }
    }
}