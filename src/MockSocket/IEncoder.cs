using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MockSocket
{
    public interface IEncoder
    {
        /// <summary>
        /// 编码到字节里
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <param name="bytes"></param>
        /// <returns></returns>
        int Encode<T>(T model, Memory<byte> bytes);

        /// <summary>
        /// 从字节中解码
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="bytes"></param>
        /// <param name="len"></param>
        /// <returns></returns>
        T? Decode<T>(ReadOnlyMemory<byte> bytes, int len) where T : class;

        object Decode(ReadOnlyMemory<byte> bytes, int len, Type type);
    }
}
