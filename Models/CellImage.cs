using System.Windows.Media;
using WpfApp1.Models;

namespace WpfApp1.Base
{
    public class CellImage
    {
        private int _index;
        private byte[,] Matrix;

        public CellImage(int index, ref byte[,] matrix)
        {
            _index = index;
            Matrix = matrix;
        }

        public ImageSource this[int i]
        {
            get
            {
                return CellImageMaper.GetPermissionStateByIndex(Matrix[_index, i]);
            }
        }
    }
}
