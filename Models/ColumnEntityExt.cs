using Gamanet.C4.SimpleInterfaces;

namespace WpfApp1.Models
{
    public static class ColumnEntityExt
    {
        public static bool CopyFrom(this ColumnEntity to, PermissionTypeDefinitionV2 from)
        {
            to.TypeId = from.PermissionTypeId;
            to.TypeName = from.Name;
            to.ResourceKey = from.ResourceName;
            to.OrderIndex = from.OrderIndex;

            return true;
        }
    }
}
