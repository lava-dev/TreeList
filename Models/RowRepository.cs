using Gamanet.Utilities.Themes.Model;
using System.Diagnostics;
using WpfApp1.Base;
using WpfApp1.Hosters;

namespace WpfApp1.Models
{
    public class RowRepository : PropertyChangedBase
    {
        private _AppContext _appContext;
        private DiagnosticsHoster _diagnosticsHost => _appContext.DiagnosticsHost;
        private Stopwatch _stopwatch = new Stopwatch();

        public RowRepository(_AppContext appContext)
        {
            _appContext = appContext;
        }

        public PermissionMatrixEntity _matrix
         = new PermissionMatrixEntity();

        public SmartCollection<RowEntity> Rows { get; set; }
         = new SmartCollection<RowEntity>();

        public bool LoadData()
        {
            Rows.Clear();
            //_stopwatch.Reset();
            //_stopwatch.Start();
            new RowSimpleClientDataProvider(_appContext).FillInRows();
            //_stopwatch.Stop();
            //_diagnosticsHost.SendMessage($" Whole time LoadData RowRepository {_stopwatch.ElapsedMilliseconds} ms.");

            return true;
        }
    }
}
