using Gamanet.C4.SimpleInterfaces;
using System;

namespace WpfApp1.Models
{
    public class SessionContext
    {
        public SessionContext(Type leftSideEntityType,
                              Type rightSideEntityType,
                              Guid leftSideRootId,
                              Guid rightSideRootId,
                              string panel = null)
        {
            LeftSideEntityType = leftSideEntityType;
            RightSideEntityType = rightSideEntityType;
            LeftSideRootId = leftSideRootId;
            RightSideRootId = rightSideRootId;

            PanelName = panel;

            SetupTrustee();
            SetupEntityTypeId();
        }

        private Type LeftSideEntityType { get; }
        private Type RightSideEntityType { get; }

        public Guid LeftSideRootId { get; private set; }
        public Guid RightSideRootId { get; private set; }
        public Guid RightSideParentRootId { get; private set; }
        public Guid LeftSideTypeId { get; private set; }
        public Guid RightSideTypeId { get; private set; }

        public string PanelName { get; }
        public bool IsTrustee { get; private set; }

        private void SetupEntityTypeId()
        {
            if (LeftSideEntityType.Equals(typeof(SimplePersonV1)))
            {
                LeftSideTypeId = EntityType.Persons;
            }
            else if (LeftSideEntityType.Equals(typeof(SimpleDeviceV1)))
            {
                LeftSideTypeId = EntityType.Devices;
            }
            else if (LeftSideEntityType.Equals(typeof(SimpleRegionV1)))
            {
                LeftSideTypeId = EntityType.Regions;
            }
            else if (LeftSideEntityType.Equals(typeof(SimpleAgentV1)))
            {
                LeftSideTypeId = EntityType.Agents;
            }
            else if (LeftSideEntityType.Equals(typeof(SimplePanelV1)))
            {
                LeftSideTypeId = EntityType.Panels;
            }
            else if (LeftSideEntityType.Equals(typeof(SimpleCommandV1)))
            {
                LeftSideTypeId = EntityType.Commands;
            }
            else if (LeftSideEntityType.Equals(typeof(SimpleEventV1)))
            {
                LeftSideTypeId = EntityType.Events;
            }

            if (RightSideEntityType.Equals(typeof(SimplePersonV1)))
            {
                RightSideTypeId = EntityType.Persons;
                RightSideParentRootId = EntityRoot.PersonSuperRoot;
            }
            else if (RightSideEntityType.Equals(typeof(SimpleDeviceV1)))
            {
                RightSideTypeId = EntityType.Devices;
                RightSideParentRootId = EntityRoot.DeviceSuperRoot;
            }
            else if (RightSideEntityType.Equals(typeof(SimpleRegionV1)))
            {
                RightSideTypeId = EntityType.Regions;
                RightSideParentRootId = EntityRoot.RegionSuperRoot;
            }
            else if (RightSideEntityType.Equals(typeof(SimpleAgentV1)))
            {
                RightSideTypeId = EntityType.Agents;
                RightSideParentRootId = EntityRoot.AgentSuperRoot;
            }
            else if (RightSideEntityType.Equals(typeof(SimplePanelV1)))
            {
                RightSideTypeId = EntityType.Panels;
                RightSideParentRootId = Guid.Empty;
            }
            else if (RightSideEntityType.Equals(typeof(SimpleCommandV1)))
            {
                RightSideTypeId = EntityType.Commands;
                RightSideParentRootId = Guid.Empty;
            }
            else if (RightSideEntityType.Equals(typeof(SimpleEventV1)))
            {
                RightSideTypeId = EntityType.Events;
                RightSideParentRootId = Guid.Empty;
            }
        }

        private void SetupTrustee()
        {
            if (LeftSideEntityType.Equals(typeof(SimplePersonV1)))
            {
                IsTrustee = true;
            }
        }
    }
}
