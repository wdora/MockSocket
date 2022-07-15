using Microsoft.Extensions.DependencyInjection;
using MockSocket.Core.Tcp;
using MockSocket.HoleServer;
using Moq;
using System.Net;
using Xunit.Abstractions;

[assembly: CollectionBehavior(DisableTestParallelization = true)]
namespace MockSocket.xUnit
{
    public class TcpHoleServerTests : BaseTest, IDisposable
    {
        readonly ITestOutputHelper output;

        public TcpHoleServerTests(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Theory]
        [InlineData(9090)]
        [InlineData(10000)]
        public async Task StartServer_Should_Listen_Port_And_Accept_When_Config_Port(int listenPort)
        {
            // arrange
            var mock = new Mock<ITcpServerConnection>();
            var config = new Dictionary<string, string>
            {
                { nameof(HoleServerOption.ListenPort) , listenPort.ToString() }
            };
            using var holeServer = CreateServiceProvider(mock.Object, config).GetService<IHoleServer>()!;

            // act
            await holeServer.StartAsync();

            // assert
            mock.Verify(x => x.ListenAsync(new IPEndPoint(IPAddress.Any, listenPort)));
            mock.Verify(x => x.AcceptAsync(default));
        }

        public void Dispose()
        {
            output.WriteLine("disposed");
        }
    }
}