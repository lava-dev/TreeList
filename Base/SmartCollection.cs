using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows;
using System.Windows.Threading;

namespace Gamanet.Utilities.Themes.Model
{
    public class SmartCollection<T> : ObservableCollection<T>
    {
        private DispatcherTimer _timer;
        private long _lastcounter = -1;
        private long _refreshInterval = 100;    

        public SmartCollection() : base()
        {
            ActivateTimer();
        }

        private void ActivateTimer()
        {
            if (_timer != null) return;
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromMilliseconds(_refreshInterval);
            _timer.Tick += StatusTimer_Tick;
            _timer.Start();
        }

        public void SuspendNotifications()
        {
            _timer.Stop();
            Application.Current.Dispatcher.Invoke(() => NotifyChanges());
        }

        public void ResumeNotifications()
        {
            if (_timer.IsEnabled) return;
            _timer.Start();
        }

        private void StatusTimer_Tick(object sender, EventArgs e) => NotifyChanges();

        private void NotifyChanges()
        {
            if (_lastcounter == Count) return;
            _lastcounter = Count;

            base.OnPropertyChanged(new PropertyChangedEventArgs("Count"));
            base.OnPropertyChanged(new PropertyChangedEventArgs("Item[]"));
            base.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        public void AddRange(IEnumerable<T> range)
        {
            foreach (var item in range)
            {
                Items.Add(item);
            }
        }

        public void Reset(IEnumerable<T> range)
        {
            this.Items.Clear();
            AddRange(range);
        }

        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {

        }
    }
}
