using System;

namespace WpfApp1.Base
{
    public class RowEntity : PropertyChangedBase
    {
        public RowEntity(Guid id, 
                         Guid parentId, 
                         Guid virtualId, 
                         Guid categoryId, 
                         Guid displayCategoryId, 
                         string name, 
                         int orderIndex, 
                         int iconIndex, 
                         ref byte[,] matrix)
        {
            Id = id;
            ParentId = parentId;
            VirtualId = virtualId;
            CategoryId = categoryId;
            DisplayCategoryId = displayCategoryId;
            Name = name;
            OrderIndex = orderIndex;
            IconIndex = iconIndex;

            _columnType = matrix;
        }

        private Guid _virtualId = Guid.Empty;
        public Guid VirtualId
        {
            get { return _virtualId; }
            set
            {
                if (_virtualId != value)
                {
                    _virtualId = value;
                    OnPropertyChanged(nameof(VirtualId));
                }
            }
        }

        private Guid _id = Guid.Empty;
        public Guid Id
        {
            get { return _id; }
            set
            {
                if (_id != value)
                {
                    _id = value;
                    OnPropertyChanged(nameof(Id));
                }
            }
        }

        private Guid _parentId = Guid.Empty;
        public Guid ParentId
        {
            get { return _parentId; }
            set
            {
                if (_parentId != value)
                {
                    _parentId = value;
                    OnPropertyChanged(nameof(ParentId));
                }
            }
        }

        private Guid _categoryId = Guid.Empty;
        public Guid CategoryId
        {
            get { return _categoryId; }
            set
            {
                if (_categoryId != value)
                {
                    _categoryId = value;
                    OnPropertyChanged(nameof(CategoryId));
                }
            }
        }

        private Guid _displayCategoryId = Guid.Empty;
        public Guid DisplayCategoryId
        {
            get { return _displayCategoryId; }
            set
            {
                if (_displayCategoryId != value)
                {
                    _displayCategoryId = value;
                    OnPropertyChanged(nameof(DisplayCategoryId));
                }
            }
        }

        private string _name = string.Empty;
        public string Name
        {
            get { return _name; }
            set
            {
                if (_name != value)
                {
                    _name = value;
                    OnPropertyChanged(nameof(Name));
                }
            }
        }

        private int _level = 0;
        public int Level
        {
            get { return _level; }
            set
            {
                if (_level != value)
                {
                    _level = value;
                    OnPropertyChanged(nameof(Level));
                }
            }
        }

        private byte[,] _columnType;
        public int Index;
        public byte this[int i]
        {
            get
            {
                return _columnType[Index, i];
            }
            set
            {
                _columnType[Index, i] = value;
            }
        }

        private CellImage _cells;
        public CellImage Cells
        {
            get
            {
                if (_cells == null)
                {
                    _cells = new CellImage(Index, ref _columnType);
                }
                return _cells;
            }
        }

        private bool _isExpanded = false;
        public bool IsExpanded
        {
            get { return _isExpanded; }
            set
            {
                if (_isExpanded != value)
                {
                    _isExpanded = value;
                    OnPropertyChanged(nameof(IsExpanded));
                }
            }
        }

        private bool _isVisible = false;
        public bool IsVisible
        {
            get { return _isVisible; }
            set
            {
                if (_isVisible != value)
                {
                    _isVisible = value;
                    OnPropertyChanged(nameof(IsVisible));
                }
            }
        }

        private bool _isExpandable = false;
        public bool IsExpandable
        {
            get { return _isExpandable; }
            set
            {
                if (_isExpandable != value)
                {
                    _isExpandable = value;
                    OnPropertyChanged(nameof(IsExpandable));
                }
            }
        }

        private int _orderIndex = 0; // todo: spytat: ake tu dat' cislo?
        public int OrderIndex
        {
            get { return _orderIndex; }
            set
            {
                if (_orderIndex != value)
                {
                    _orderIndex = value;
                    OnPropertyChanged(nameof(OrderIndex));
                }
            }
        }

        private int _iconIndex = 0;
        public int IconIndex
        {
            get { return _iconIndex; }
            set
            {
                if (_iconIndex != value)
                {
                    _iconIndex = value;
                    OnPropertyChanged(nameof(IconIndex));
                }
            }
        }

        private bool _isModifyPermission = false;
        public bool IsModifyPermission
        {
            get { return _isModifyPermission; }
            set
            {
                if (_isModifyPermission != value)
                {
                    _isModifyPermission = value;
                    OnPropertyChanged(nameof(IsModifyPermission));
                }
            }
        }

        private bool _isDefineAccess = false;
        public bool IsDefineAccess
        {
            get { return _isDefineAccess; }
            set
            {
                if (_isDefineAccess != value)
                {
                    _isDefineAccess = value;
                    OnPropertyChanged(nameof(IsDefineAccess));
                }
            }
        }
    }
}