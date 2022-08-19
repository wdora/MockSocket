using MockSocket.Core.Tcp;
using MockSocket.ForwardServer;
using Moq;
using Shouldly;

namespace MockSocket.xUnit
{
    public class ForwardTests : BaseTest
    {
        [Fact]
        public async Task should_forward_from_src_to_dst()
        {
            // arrange
            var sendData = new List<byte[]>
            {
                new byte[] { 1, 2, 3 },
            };
            var receiveData = new List<byte[]>();

            var srcConnMock = new Mock<ITcpConnection>();

            var srcConnSeq = srcConnMock.SetupSequence(x => x.ReceiveAsync(It.IsAny<Memory<byte>>(), default));
            sendData.ForEach(data => srcConnSeq.ReturnsAsync(data.Length));

            var dstConnMock = new Mock<ITcpConnection>();
            //dstConnMock.SetupSequence(x => x.SendAsync(It.Is<ReadOnlyMemory<byte>>(c => someBytes.ToArray().SequenceEqual(c.ToArray())), default));

            var forwarder = new TcpForwarder();

            // act
            await forwarder.ForwardAsync(srcConnMock.Object, dstConnMock.Object);

            // assert
            for (int i = 0; i < sendData.Count; i++)
                sendData[i].SequenceEqual(receiveData[i]).ShouldBeTrue();
        }

        private void ShouldWriteConnect(Mock<ITcpConnection> dstConn, Memory<byte> someBytes)
        {
            dstConn.Verify(c => c.SendAsync(It.Is<ReadOnlyMemory<byte>>(c => someBytes.ToArray().SequenceEqual(c.ToArray())), default));
        }

        private void SetupReceiveBytes(Mock<ITcpConnection> srcConn, Memory<byte> someBytes)
        {
            srcConn
                .Setup(x => x.ReceiveAsync(It.IsAny<Memory<byte>>(), default))
                .Callback<Memory<byte>, CancellationToken>((m, c) => someBytes.CopyTo(m))
                .ReturnsAsync(someBytes.Length);
        }

        private Memory<byte> GetSomeBytes()
        {
            var max = Random.Shared.Next(200);
            return Enumerable.Range(0, max).Select(i => (byte)i).ToArray();
        }
    }
}