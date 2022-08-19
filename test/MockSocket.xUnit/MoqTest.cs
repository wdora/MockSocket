using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit.Abstractions;

namespace MockSocket.xUnit
{
    /// <summary>
    /// https://github.com/Moq/moq4/wiki/Quickstart
    /// </summary>
    public class MoqTest
    {
        public interface IUser
        {
            string Name { get; set; }
        }

        readonly ITestOutputHelper output;

        public MoqTest(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Fact]
        public void Should_Null_When_Set_MockObject_Property()
        {
            var mockUser = new Mock<IUser>();
            var user = mockUser.Object;

            user.Name = "123";

            Assert.Null(user.Name);

            mockUser.Setup(x => x.Name).Returns("123");
            Assert.Equal("123", user.Name);
            Assert.Equal("123", user.Name);
         
            mockUser.Setup(x => x.Name).Returns("1234");
            Assert.Equal("1234", user.Name);
        }

        [Fact]
        public void Should_Not_Exception()
        {
            var a = false;
            var b = false;

            var c = a == b == true;

            Assert.True(c);

            foreach (var item in GetStrings(true))
                output.WriteLine(item);

            output.WriteLine("ok");
        }

        private IEnumerable<string> GetStrings(bool isBreak)
        {
            if (isBreak)
                yield break;

            yield return "123";
        }
    }
}
