using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using WpfApp1.Base.Enums;

namespace WpfApp1.Models
{
    public static class CommandImageMaper
    {
        public static ImageSource GetPermissionState(enPermissionStateCommand state)
        {
            switch (state)
            {
                case enPermissionStateCommand.Inherited:
                    return _inheritPermissionIcon;

                case enPermissionStateCommand.AllowedDirect:
                    return _allowedDirectPermissionIcon;

                case enPermissionStateCommand.DeniedDirect:
                    return _deniedDirectPermissionIcon;

                case enPermissionStateCommand.AllowedWithInheritance:
                    return _allowedWithInheritancePermissionIcon;

                case enPermissionStateCommand.DeniedWithInheritance:
                    return _deniedWithInheritancePermissionIcon;
            }

            return _inheritPermissionIcon;
        }

        private static ImageSource _allowedWithInheritancePermissionIcon
            = new BitmapImage(new Uri("pack://application:,,,/WpfApp1;component/Resources/Allow.png"));
        private static ImageSource _allowedDirectPermissionIcon
            = new BitmapImage(new Uri("pack://application:,,,/WpfApp1;component/Resources/AllowDirect.png"));
        private static ImageSource _deniedWithInheritancePermissionIcon
            = new BitmapImage(new Uri("pack://application:,,,/WpfApp1;component/Resources/Deny.png"));
        private static ImageSource _deniedDirectPermissionIcon
            = new BitmapImage(new Uri("pack://application:,,,/WpfApp1;component/Resources/DenyDirect.png"));

        private static ImageSource _inheritPermissionIcon
            = new BitmapImage(new Uri("pack://application:,,,/WpfApp1;component/Resources/Inherit.png"));
    }
}
