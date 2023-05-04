namespace MockSocket.Common.Models;

public class BufferResult : IDisposable
{
    byte[] buffer;

    Action disposeHandle;

    private bool disposed = false;

    ~BufferResult()
    {
        Dispose();
    }

    public BufferResult(byte[] buffer, Action disposeHandle)
    {
        this.buffer = buffer;
        this.disposeHandle = disposeHandle;
    }

    public void Dispose()
    {
        if (disposed)
            return;

        disposeHandle?.Invoke();

        disposed = true;
    }

    public ReadOnlyMemory<byte> SliceTo(int len)
    {
        Memory<byte> memory = buffer;

        return memory.Slice(0, len);
    }

    public static implicit operator Memory<byte>(BufferResult result) => result.buffer;

    public static implicit operator ReadOnlyMemory<byte>(BufferResult result) => result.buffer;

    public static implicit operator Span<byte>(BufferResult result) => result.buffer;
}
