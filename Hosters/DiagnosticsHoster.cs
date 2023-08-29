using System;
using WpfApp1.Models;

namespace WpfApp1.Hosters
{
    public class DiagnosticsHoster
    {
        private _AppContext _appContext;

        public event Action<string> OnMessage;

        public DiagnosticsHoster(_AppContext appConext)
        {
            _appContext = appConext;
        }

        public void SendMessage(string message)
        {
            OnMessage?.Invoke(message);
        }
    }
}
