using Gamanet.C4.PermissionCalculator;
using Gamanet.C4.SimpleInterfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using WpfApp1.DataSources;
using WpfApp1.Models;

namespace WpfApp1.Hosters
{
    public class CalculatorHoster
    {
        public CalculatorHoster(_AppContext context)
        {
            _appContext = context;
        }

        private _AppContext _appContext;
        private DiagnosticsHoster _diagnosticsHost => _appContext.DiagnosticsHost;

        private bool IsTrustee => _appContext.SessionContext.IsTrustee;

        private ClientPermissionCalculatorForObject CalculatorForObject { get; set; }
        private ClientPermissionCalculatorForTrustee CalculatorForTrustee { get; set; }

        public List<SimpleEntity> LeftTree { get; set; }
         = new List<SimpleEntity>();
        public List<SimpleEntity> RightTree { get; set; }
         = new List<SimpleEntity>();
        public List<CalendarV1> Calendars { get; set; }
         = new List<CalendarV1>();

        public bool FullData = true;

        private Dictionary<Guid, List<PermissionV1>> Permissions => _appContext.PermissionRepo.Permissions;

        public bool Initialize(Guid contextId)
        {
            var calculatorDataProvider = new CalculatorDataProvider(_appContext);
            return calculatorDataProvider.Initialize(contextId);
        }

        public List<EntityPermission> Calculate(Guid contextId)
        {
            var permissions = Permissions.SelectMany(i => i.Value).ToList();
            FullData = true;

            if (IsTrustee)
            {
                return CalculateForTrustee(contextId, permissions);
            }
            else
            {
                return CalculateForObject(contextId, permissions);
            }
        }

        public List<EntityPermission> CalculateForColumn(Guid contextId, Guid clickedItemId, Guid typeId)
        {
            if (!Permissions.TryGetValue(typeId, out var permissions))
            {
                return new List<EntityPermission>();
            }

            var calculatorDataProvider = new CalculatorDataProvider(_appContext);
            calculatorDataProvider.LoadRightSubTree(clickedItemId);
            FullData = false;

            var stopwatch = new Stopwatch();
            stopwatch.Start();
            List<EntityPermission> entityPermissions = new List<EntityPermission>();
            if (IsTrustee)
            {
                entityPermissions = CalculateForTrustee(contextId, permissions);
            }
            else
            {
                entityPermissions = CalculateForObject(contextId, permissions);
            }

            stopwatch.Stop();
            _diagnosticsHost.SendMessage($"Calculation time for clicked Cell: {stopwatch.ElapsedMilliseconds} ms");
            stopwatch.Reset();

            return entityPermissions;
        }

        private List<EntityPermission> CalculateForTrustee(Guid trusteeId, List<PermissionV1> permissions)
        {
            var resultInfos = new List<EntityPermission>();

            CalculatorForTrustee = new ClientPermissionCalculatorForTrustee(LeftTree, RightTree, permissions, Calendars, trusteeId);
            var infos = CalculatorForTrustee.GetPermissions();

            resultInfos.AddRange(infos);

            return resultInfos;
        }

        private List<EntityPermission> CalculateForObject(Guid objectId, List<PermissionV1> permissions)
        {
            var resultInfos = new List<EntityPermission>();

            CalculatorForObject = new ClientPermissionCalculatorForObject(RightTree, LeftTree, permissions, Calendars, objectId);
            resultInfos = CalculatorForObject.GetPermissions();

            return resultInfos;
        }
    }
}