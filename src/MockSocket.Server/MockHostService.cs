using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace MockSocket.Server2
{
    using IMockServer = MockClient.Udp.Interfaces.IMockServer;

    public class MockHostService : IHostedService
    {
        IMockServer mockServer;

        ILogger logger;

        public MockHostService(IMockServer mockServer, ILogger<MockHostService> logger)
        {
            this.mockServer = mockServer;
            this.logger = logger;

            TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;
        }

        private void TaskScheduler_UnobservedTaskException(object? sender, UnobservedTaskExceptionEventArgs e)
        {
            logger.LogError(e.Exception, "UnobservedTaskException");

            e.SetObserved();
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await mockServer.StartAsync(cancellationToken);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
