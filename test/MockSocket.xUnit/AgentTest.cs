using Microsoft.Extensions.DependencyInjection;
using MockSocket.Agent;
using Shouldly;
using System;
using System.Collections.Generic;
using System.IO.Pipelines;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace MockSocket.xUnit
{
    public class AgentTest : AgentTestBase
    {
        readonly ITcpAgent tcpAgent;

        public AgentTest()
        {
            tcpAgent = serviceProvider.GetService<ITcpAgent>()!;
        }

        [Theory]
        [InlineData("wdora.com", 80)]
        [InlineData("baidu.com", 80)]
        public async Task ConnectAsync_Ok(string host, int port)
        {
            await tcpAgent.ConnectAsync(host, port);
        }
    }

    public class FastEncodingTest : AgentTestBase
    {
        IEncoder encoder;

        public FastEncodingTest()
        {
            this.encoder = serviceProvider.GetService<IEncoder>()!;
        }

        [Theory]
        [InlineData("hello world")]
        [MemberData(nameof(GetData))]
        public void Encode_And_Decode<T>(T message)
            where T : class
        {
            Span<byte> buffer = new byte[1024];

            var len = encoder.Encode(message, buffer);

            len.ShouldBeGreaterThan(0);

            var data = encoder.Decode<T>(buffer);

            Assert.Equal(message, data);
        }

        public static IEnumerable<object[]> GetData()
        {
            yield return new object[] { new User { Id = 100 } };
        }
    }

    record class User
    {
        public int Id { get; set; }
    }

    public class AgentTestBase
    {
        protected IServiceProvider serviceProvider;

        public AgentTestBase()
        {
            this.serviceProvider = new ServiceCollection()
                .AddTransient<ITcpAgent, TcpAgent>()
                .AddTransient<IEncoder, FastEncoder>()
                .BuildServiceProvider();
        }
    }
}
