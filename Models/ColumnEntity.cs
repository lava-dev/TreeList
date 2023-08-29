using Gamanet.Common;
using System;

namespace WpfApp1.Models
{
    public class ColumnEntity : PropertyChangedBase
    {
        private Guid _typeId = Guid.Empty;
        public Guid TypeId
        {
            get { return _typeId; }
            set
            {
                if (_typeId != value)
                {
                    _typeId = value;
                    OnPropertyChanged(nameof(TypeId));
                }
            }
        }

        private string _typeName = string.Empty;
        public string TypeName
        {
            get { return _typeName; }
            set
            {
                if (_typeName != value)
                {
                    _typeName = value;
                    OnPropertyChanged(nameof(TypeName));
                }
            }
        }

        private string _resourceKey = string.Empty;
        public string ResourceKey
        {
            get { return _resourceKey; }
            set
            {
                if (_resourceKey != value)
                {
                    _resourceKey = value;
                    OnPropertyChanged(nameof(ResourceKey));
                }
            }
        }

        private int _orderIndex = 0;
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
    }
}
