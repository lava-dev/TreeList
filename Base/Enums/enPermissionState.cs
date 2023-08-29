namespace Gamanet.C4.Client.Permissions.BusinessLogic.Base.Enums
{
    public enum enPermissionState : byte
    {
        DeniedInherited = 0,
        AllowedInherited = 1,

        AllowedDirect = 2,
        DeniedDirect = 3,

        AllowedWithInheritance = 4,
        DeniedWithInheritance = 5,
    }
}
