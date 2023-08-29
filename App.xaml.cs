using Gamanet.C4.SDK;
using Gamanet.C4.SimpleInterfaces;
using System.Windows;
using WpfApp1.Models;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            var simpleClient = new SimpleClient(ConnectorConfiguration.HttpClient);

            var dataContext = new _AppContext(simpleClient,
                                              new SessionContext(typeof(SimplePersonV1),
                                                                 typeof(SimplePersonV1),
                                                                 EntityRoot.PersonSuperRoot,
                                                                 EntityRoot.Person));
            var view = new MainWindowView();
            view.DataContext = dataContext;
            view.Show();
        }
    }
}
