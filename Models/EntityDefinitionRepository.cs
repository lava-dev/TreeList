using System;
using System.Collections.Generic;

namespace WpfApp1.Models
{
    public class EntityDefinitionRepository
    {
        private _AppContext _appContext;
        
        public Dictionary<Guid, int> EntityOrderIndexValues { get; set; }
         = new Dictionary<Guid, int>();
        public Dictionary<Guid, IconEntity> IconEntityMapper { get;set; }
         = new Dictionary<Guid, IconEntity>();
        public Dictionary<int, IconEntity> IconIndexMapper { get; set; }
         = new Dictionary<int, IconEntity>();

        public EntityDefinitionRepository(_AppContext appContext)
        {
            _appContext = appContext;
        }

        public bool LoadData()
        {
            EntityOrderIndexValues.Clear();
            IconEntityMapper.Clear();
            IconIndexMapper.Clear();

            var dataProvider = new SimpleClientDataProvider(_appContext);
            var definitions = dataProvider.GetAllDefinitions();

            for (int i = 0; i < definitions.Count; i++)
            {
                var definition = definitions[i];
                if (definition.Data.TryGetValue("OrderIndex", out var orderIndex)
                    && int.TryParse(orderIndex.ToString(), out var index))
                {
                    EntityOrderIndexValues.Add(definition.Id, index);
                }

                if (definition.Data.TryGetValue("Icon", out var icon))
                {
                    var ico = icon.ToString();
                    var iconMapper = new IconEntity(definition.Id, i, ico);

                    IconEntityMapper.Add(definition.Id, iconMapper);
                    IconIndexMapper.Add(i, iconMapper);
                }
            }

            return true;
        }
    }
}