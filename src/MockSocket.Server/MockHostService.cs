using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MockSocket.Server
{
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
