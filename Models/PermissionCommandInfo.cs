using WpfApp1.Base.Enums;

namespace WpfApp1.Models
{
    public class PermissionCommandInfo
    {
        public PermissionCommandInfo(int columnIndex, enPermissionStateCommand selectedState)
        {
            ColumnIndex = columnIndex;
            SelectedState = selectedState;
        }

        public int ColumnIndex { get; }
        public enPermissionStateCommand SelectedState { get; }
    }
}
