using Gamanet.C4.SimpleInterfaces;
using WpfApp1.Hosters;

namespace WpfApp1.Models
{
    public class _AppContext
    {
        public _AppContext(ISimpleClient simpleClient, SessionContext sessionContext)
        {
            DiagnosticsHost = new DiagnosticsHoster(this);
            CalculatorHost = new CalculatorHoster(this);

            SimpleClientHost = new SimpleClientHoster(simpleClient);
            RowRepo = new RowRepository(this);
            EntityDefinitionRepo = new EntityDefinitionRepository(this);
            PermissionRepo = new PermissionRepository(this);
            ColumnDefinitionRepo = new ColumnDefinitionRepository(this);

            SessionContext = sessionContext;
        }

        public SessionContext SessionContext { get; }
        public DiagnosticsHoster DiagnosticsHost { get; }
        public CalculatorHoster CalculatorHost { get; }
        public SimpleClientHoster SimpleClientHost { get; }
        public RowRepository RowRepo { get; }
        public EntityDefinitionRepository EntityDefinitionRepo { get; }
        public PermissionRepository PermissionRepo { get; }
        public ColumnDefinitionRepository ColumnDefinitionRepo { get; }
    }
}
