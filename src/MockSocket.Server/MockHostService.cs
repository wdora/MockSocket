using Microsoft.Extensions.Hosting;
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

        public MockHostService(IMockServer mockServer)
        {
            this.mockServer = mockServer;
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
