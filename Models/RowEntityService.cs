using Gamanet.Utilities.Themes.Model;
using System.Diagnostics;
using WpfApp1.Base;
using WpfApp1.Hosters;

namespace WpfApp1.Models
{
    public class RowEntityService
    {
        private Stopwatch _stopWatch = new Stopwatch();
        private _AppContext _appContext;

        private SmartCollection<RowEntity> Rows => _appContext.RowRepo.Rows;
        private DiagnosticsHoster DiagnosticsHost => _appContext.DiagnosticsHost;

        public RowEntityService(_AppContext appContext)
        {
            _appContext = appContext;
        }

        public bool ExpandNext(int index)
        {
            _stopWatch.Reset();
            _stopWatch.Start();

            var currentRow = Rows[index];
            currentRow.IsExpanded = true;

            var expandLevel = currentRow.Level + 1;
            for (int i = ++index; i < Rows.Count; i++)
            {
                var nextRow = Rows[i];
                if (expandLevel == nextRow.Level)
                {
                    nextRow.IsVisible = true;
                    if (nextRow.IsExpanded)
                    {
                        ExpandNext(nextRow.Index);
                    }
                }
                else if (expandLevel < nextRow.Level)
                {
                    // do nothing here
                    continue;
                }
                else if (expandLevel > nextRow.Level)
                {
                    break;
                }
            }

            _stopWatch.Stop();
            //DiagnosticsHost.SendMessage($"ExpandNext with FOR Cycle took {_stopWatch.ElapsedMilliseconds} ms.");

            return true;
        }

        public bool CollapseNext(int index)
        {
            _stopWatch.Reset();
            _stopWatch.Start();

            var currentRow = Rows[index];
            currentRow.IsExpanded = false;

            var collapseLevel = currentRow.Level + 1;
            for (int i = ++index; i < Rows.Count; i++)
            {
                var nextRow = Rows[i];
                if (collapseLevel <= nextRow.Level)
                {
                    nextRow.IsVisible = false;
                }
                else if (collapseLevel > nextRow.Level)
                {
                    break;
                }
            }
            _stopWatch.Stop();
            //DiagnosticsHost.SendMessage($"CollapseNext with FOR Cycle took {_stopWatch.ElapsedMilliseconds} ms.");

            return true;
        }

        public bool Expand(int index)
        {
            _stopWatch.Reset();
            _stopWatch.Start();

            for (int i = index; i < Rows.Count; i++)
            {
                var nextNode = Rows[i];
                nextNode.IsExpanded = true;
                nextNode.IsVisible = true;
            }
            _stopWatch.Stop();
            //DiagnosticsHost.SendMessage($"Expand(count = {Rows.Count}) with FOR Cycle took {_stopWatch.ElapsedMilliseconds} ms.");

            return true;
        }

        public bool Collapse(int index)
        {
            _stopWatch.Reset();
            _stopWatch.Start();

            var root = Rows[index];
            if (index != 0)
            {
                root.IsExpanded = false;
                index++;
            }

            for (int i = index; i < Rows.Count; i++)
            {
                var nextNode = Rows[i];
                nextNode.IsExpanded = false;
                nextNode.IsVisible = false;
            }
            _stopWatch.Stop();
            //DiagnosticsHost.SendMessage($"Collapse(count = {Rows.Count}) with FOR Cycle took {_stopWatch.ElapsedMilliseconds} ms.");

            return true;
        }
    }
}