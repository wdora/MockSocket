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
        [Theory]
        [InlineData("wdora.com", 10000)]
        public async ValueTask HeartBeatAsync(string host, int port)
        {
            var agent = new MockTcpClient();

            await agent.ConnectAsync(host, port);

            //await agent.HeartBeatAsync();
        }

        //[Theory]
        //public ValueTask CreateAppServerAsync(string host, int port, int remotePort)
        //{
        //    return default;
        //}


    }



    public class FastEncodingTest : AgentTestBase
    {
        IEncoder encoder;

        public FastEncodingTest()
        {
            this.encoder = serviceProvider.GetService<IEncoder>()!;
        }

        [Theory]
        //[InlineData("hello world")]
        [MemberData(nameof(GetData))]
        public void Encode_And_Decode<T>(T message)
            where T : class
        {
            Memory<byte> buffer = new byte[1024];

            var len = encoder.Encode(message, buffer);

            var data1 = encoder.Decode<T>(buffer, len);
            var data2 = (T?)encoder.Decode<object>(buffer, len);

            Assert.Equal(message, data1);
            Assert.Equal(data1, data2);
        }

        public static IEnumerable<object[]> GetData()
        {
            yield return new object[] { new User { Id = 100 } };
            yield return new object[] { null };
        }
    }

    [Serializable]
    record class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class AgentTestBase
    {
        protected IServiceProvider serviceProvider;

        public AgentTestBase()
        {
            this.serviceProvider = new ServiceCollection()
                .AddTransient<ITcpClient, MockTcpClient>()
                .AddTransient<IEncoder, JsonEncoder>()
                .BuildServiceProvider();
        }
    }
}
