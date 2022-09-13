using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using MockSocket.Message;
using Shouldly;

namespace MockSocket.xUnit
{
    public class MessageTests
    {
        [Theory]
        [InlineData("hello", "5:hello")]
        [InlineData("hello我", "8:hello我")]
        [InlineData("1", "1:1")]
        [InlineData("1:1", "3:1:1")]
        public void Encode(string srcStr, string dstStr)
        {
            Memory<byte> buffer = new byte[1024];

            var len = MessageEncoding.Encode(srcStr, buffer);

            var bytes = buffer.Slice(0, len);

            bytes.ShouldBe(Encoding.UTF8.GetBytes(dstStr));
        }

        [Theory]
        [InlineData("1:h", "h")]
        [InlineData("5:hello", "hello")]
        [InlineData("10:helloworld", "helloworld")]
        [InlineData("3:我aaaaa", "我")]
        public void RestoreMessage(string srcStr, string dstStr)
        {
            var sourceBytes = Encoding.UTF8.GetBytes(srcStr);

            var str = MessageEncoding.Decode(sourceBytes);

            str.ShouldBe(dstStr);
        }
    }
}
