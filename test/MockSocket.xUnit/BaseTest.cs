using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MockSocket.Core.Tcp;
using MockSocket.HoleServer;

namespace MockSocket.xUnit
{
    public class BaseTest
    {
        public const int ListenPort = 10000;

        protected IServiceProvider CreateServiceProvider<T>(T mockInstance, Dictionary<string, string> configSource = default)
            where T : class
        {
            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(configSource).Build();
            
            var collection = new ServiceCollection();

            collection
                .AddHoleServer(config)
                .AddSingleton(mockInstance)
                .AddLogging();

            return collection.BuildServiceProvider();
        }
    }
}