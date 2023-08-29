using Gamanet.C4.Client.Permissions.BusinessLogic.Base.Enums;
using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace WpfApp1.Models
{
    public static class CellImageMaper
    {
        public static ImageSource GetPermissionStateByIndex(int index)
        {
            return GetPermissionState((enPermissionState)index);
        }

        private static ImageSource GetPermissionState(enPermissionState state)
        {
            switch (state)
            {
                case enPermissionState.DeniedInherited:
                    return GetDefault;

                case enPermissionState.AllowedInherited:
                    return _allowedInheritedPermissionIcon;

                case enPermissionState.AllowedDirect:
                    return _allowedDirectPermissionIcon;

                case enPermissionState.DeniedDirect:
                    return _deniedDirectPermissionIcon;

                case enPermissionState.AllowedWithInheritance:
                    return _allowedWithInheritancePermissionIcon;

                case enPermissionState.DeniedWithInheritance:
                    return _deniedWithInheritancePermissionIcon;
            }

            return GetDefault;
        }

        public static ImageSource GetDefault => _deniedInheritedPermissionIcon;

        private static ImageSource _allowedWithInheritancePermissionIcon
            = new BitmapImage(new Uri("pack://application:,,,/WpfApp1;component/Resources/Allow.png"));
        private static ImageSource _allowedDirectPermissionIcon
            = new BitmapImage(new Uri("pack://application:,,,/WpfApp1;component/Resources/AllowDirect.png"));
        private static ImageSource _allowedInheritedPermissionIcon
            = new BitmapImage(new Uri("pack://application:,,,/WpfApp1;component/Resources/AllowInherited.png"));
        private static ImageSource _deniedWithInheritancePermissionIcon
            = new BitmapImage(new Uri("pack://application:,,,/WpfApp1;component/Resources/Deny.png"));
        private static ImageSource _deniedDirectPermissionIcon
            = new BitmapImage(new Uri("pack://application:,,,/WpfApp1;component/Resources/DenyDirect.png"));
        private static ImageSource _deniedInheritedPermissionIcon
            = new BitmapImage(new Uri("pack://application:,,,/WpfApp1;component/Resources/DenyInherited.png"));
    }
}
