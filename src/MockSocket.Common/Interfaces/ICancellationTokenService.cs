using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MockSocket.Common.Interfaces;
public interface ICancellationTokenService
{
    TokenResult CreateToken(CancellationToken token, Action? disposeAction = null);
}

public class TokenResult : IDisposable
{
    private CancellationTokenSource cancellationTokenSource;

    bool isStop;

    public TokenResult(CancellationTokenSource cancellationTokenSource)
    {
        this.cancellationTokenSource = cancellationTokenSource;
    }

    public void Dispose()
    {
        if (!isStop)
        {
            isStop = true;

            cancellationTokenSource.Cancel();

            cancellationTokenSource.Dispose();
        }
    }

    public static implicit operator CancellationTokenSource(TokenResult result) => result.cancellationTokenSource;

    public static implicit operator CancellationToken(TokenResult result) => result.cancellationTokenSource.Token;
}
