using Gamanet.C4.SimpleInterfaces;
using Gamanet.Utilities.Themes.Model;
using System;
using System.ComponentModel;
using System.Windows.Data;
using WpfApp1.Base;
using WpfApp1.Base.Enums;
using WpfApp1.Hosters;
using WpfApp1.Models;

namespace WpfApp1
{
    public class MainWindowViewModel : PropertyChangedBase
    {
        private _AppContext _appContext;
        private DiagnosticsHoster _diagnosticsHost => _appContext.DiagnosticsHost;
        private Action _dataInitialized;

        public MainWindowViewModel(_AppContext appContext, Action dataInitialized)
        {
            _appContext = appContext;
            _appContext.DiagnosticsHost.OnMessage += DiagnosticsHost_OnMessage;

            RowCollectionView = CollectionViewSource.GetDefaultView(Rows);
            RowCollectionView.Filter = CheckFilter;

            _dataInitialized = dataInitialized;
        }

        public SmartCollection<RowEntity> Rows => _appContext.RowRepo.Rows;

        public ICollectionView RowCollectionView { get; set; }

        private string _statusInfo;

        public string StatusInfo
        {
            get => _statusInfo;
            set
            {
                if (_statusInfo != value)
                {
                    _statusInfo = value;
                    OnPropertyChanged(nameof(StatusInfo));
                }
            }
        }

        public void GetDataAndBuildTree()
        {
            Rows.SuspendNotifications();

            _appContext.EntityDefinitionRepo.LoadData();
            _appContext.ColumnDefinitionRepo.LoadData();
            _appContext.RowRepo.LoadData();

            var contextId = EntityRoot.PersonSuperRoot;
            _appContext.PermissionRepo.LoadData(contextId);
            
            _dataInitialized?.Invoke();

            _appContext.CalculatorHost.Initialize(contextId);

            // initial calculation
            var calculatorService = new CalculatorService(_appContext);
            calculatorService.Activate(contextId);

            Rows.ResumeNotifications();
        }

        public void CalculateForCell(int row, int column)
        {
            var contextId = EntityRoot.PersonSuperRoot;

            var calculatorService = new CalculatorService(_appContext);
            calculatorService.CalculateForCell(contextId, row, column);

            RowCollectionView.Refresh();
        }

        public void ChangeCellToTargetState(int row, int column, enPermissionStateCommand selectedState)
        {
            var contextId = EntityRoot.PersonSuperRoot;

            var calculatorService = new CalculatorService(_appContext);
            calculatorService.ChangeCellToTargetState(contextId, row, column, selectedState);
            
            RowCollectionView.Refresh();
        }

        private bool CheckFilter(object item)
        {
            return ((RowEntity)item).IsVisible;
        }

        private void DiagnosticsHost_OnMessage(string message)
        {
            StatusInfo += $"{message}\r\n";
        }

        public void ExpandAll()
        {
            var rowEntitySrv = new RowEntityService(_appContext);
            rowEntitySrv.Expand(0);
            RowCollectionView.Refresh();
        }

        public void CollapseAll()
        {
            var rowEntitySrv = new RowEntityService(_appContext);
            rowEntitySrv.Collapse(0); 
            RowCollectionView.Refresh();
        }

        public void ExpandNext(int index)
        {
            var rowEntitySrv = new RowEntityService(_appContext);
            rowEntitySrv.ExpandNext(index);
            RowCollectionView.Refresh();
        }

        public void CollapseNext(int index)
        {
            var rowEntitySrv = new RowEntityService(_appContext);
            rowEntitySrv.CollapseNext(index); 
            RowCollectionView.Refresh();
        }
    }
}
