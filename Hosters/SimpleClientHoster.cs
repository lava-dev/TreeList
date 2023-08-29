using Gamanet.C4.SDK;
using Gamanet.C4.SimpleInterfaces;
using System;

namespace WpfApp1.Hosters
{
    public class SimpleClientHoster
    {
        public ISimpleClientV2 SimpleClient { get; set; }

        public SimpleClientHoster(ISimpleClient simpleClient)
        {
            if (simpleClient is ISimpleClientV2 scV2)
            {
                SimpleClient = scV2;

                Connect();
            }
        }

        public void Connect()
        {
            SimpleClient = new SimpleClient(ConnectorConfiguration.HttpClient);
            var result = SimpleClient.Connect(new Uri("https://c4clienttrunk.gamanet.com"),
                                              "Support",
                                              "Support*1",
                                              out var connectionInfo);

            if (result != ConnectionResult.Successful)
            {
                throw new Exception("Can't get definitions, connection failed");
            }
        }
    }
}
