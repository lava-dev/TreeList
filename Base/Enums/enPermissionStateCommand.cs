namespace WpfApp1.Base.Enums
{
    public enum enPermissionStateCommand : byte
    {
        Inherited = 0,

        AllowedDirect = 2,
        DeniedDirect = 3,

        AllowedWithInheritance = 4,
        DeniedWithInheritance = 5,
    }
}
