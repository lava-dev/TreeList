using Gamanet.C4.SimpleInterfaces;

namespace WpfApp1.Base
{
    public static class RowEntityExt
    {
        public static bool CopyFrom(this RowEntity to, SimpleEntity from)
        {
            if (from.EntityPermission != null)
            {
                to.IsModifyPermission = from.EntityPermission.Contains(PermissionType.ModifyPermissions);
                to.IsDefineAccess = from.EntityPermission.Contains(PermissionType.DefineAccessEntry);
            }

            return true;
        }

        public static bool CopyTo(this RowEntity to, RowEntity from)
        {
            to.Id = from.Id;
            to.ParentId = from.ParentId;
            to.VirtualId = from.VirtualId;
            to.CategoryId = from.CategoryId;
            to.DisplayCategoryId = from.DisplayCategoryId;
            to.Name = from.Name;
            to.OrderIndex = from.OrderIndex;
            to.IconIndex = from.IconIndex;

            return true;
        }
    }
}
