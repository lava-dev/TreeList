using Gamanet.C4.SimpleInterfaces;
using System;
using System.Collections.Generic;
using WpfApp1.Hosters;
using WpfApp1.Models;

namespace WpfApp1.DataSources
{
    public class CalculatorDataProvider
    {
        private _AppContext _appContext;
        private CalculatorHoster _calculatorHoster => _appContext.CalculatorHost;

        private List<SimpleEntity> LeftTree => _calculatorHoster.LeftTree;
        private List<SimpleEntity> RightTree => _calculatorHoster.RightTree;
        private List<CalendarV1> Calendars => _calculatorHoster.Calendars;
        private bool FullData => _calculatorHoster.FullData;

        private Guid LeftSideTypeId => _appContext.SessionContext.LeftSideTypeId;
        private Guid RightSideTypeId => _appContext.SessionContext.RightSideTypeId;


        public CalculatorDataProvider(_AppContext appContext)
        {
            _appContext = appContext;
        }

        public bool Initialize(Guid contextId)
        {
            LoadCalendars();
            LoadLeftSubTree(contextId);
            LoadRightSubTree(_appContext.SessionContext.RightSideRootId);

            return true;
        }

        public bool UpdateDataForNewContext(Guid contextId)
        {
            if (!FullData)
            {
                LoadLeftSubTree(contextId);
                LoadRightSubTree(_appContext.SessionContext.RightSideRootId);
            }

            return true;
        }

        private bool LoadCalendars()
        {
            var simpleClientDataProvider = new SimpleClientDataProvider(_appContext);
            var calendars = simpleClientDataProvider.GetAllCalendars();

            // update calendars in CalculatorHost
            foreach(var calendar in calendars)
            {
                Calendars.Add(calendar);
            }

            return true;
        }

        public bool LoadLeftSubTree(Guid contextId)
        {
            LeftTree.Clear();
            var simpleClientDataProvider = new SimpleClientDataProvider(_appContext);
            var simpleLeftEntities = simpleClientDataProvider.GetAllParentsForSelectedEntity(contextId, LeftSideTypeId);

            foreach (var simpleEntity in simpleLeftEntities)
            {
                LeftTree.Add(simpleEntity);
            }

            return true;
        }

        public bool LoadRightSubTree(Guid clickedItemId)
        {
            RightTree.Clear();
            var simpleClientDataProvider = new SimpleClientDataProvider(_appContext);
            var simpleRightEntities = simpleClientDataProvider.GetChildrenWithParentsForCalculating(clickedItemId, RightSideTypeId);
            
            foreach (var simpleEntity in simpleRightEntities)
            {
                RightTree.Add(simpleEntity);
            }

            return true;
        }
    }
}
