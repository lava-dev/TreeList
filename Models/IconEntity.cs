using Gamanet.Common;
using System;
using System.Windows.Media;

namespace WpfApp1.Models
{
    public class IconEntity : PropertyChangedBase
    {
        public IconEntity(Guid categoryId, int index, string iconName)
        {
            DisplayCategoryId = categoryId;
            IconIndex = index;
            IconName = iconName;
        }

        private ImageSource _imageSource;
        public ImageSource ImageSource
        {
            get { return _imageSource; }
            set
            {
                _imageSource = value;
                OnPropertyChanged();
            }
        }

        private int _iconIndex;
        public int IconIndex
        {
            get { return _iconIndex; }
            set
            {
                _iconIndex = value;
                OnPropertyChanged();
            }
        }

        private string _iconName = string.Empty;
        public string IconName
        {
            get { return _iconName; }
            set
            {
                _iconName = value;
                OnPropertyChanged();
            }
        }

        private Guid _displayCategoryId = Guid.Empty;
        public Guid DisplayCategoryId
        {
            get { return _displayCategoryId; }
            set
            {
                _displayCategoryId = value;
                OnPropertyChanged();
            }
        }
    }
}
