using MockSocket.Common.Interfaces;

namespace MockSocket.Common.Services;
public class CancellationTokenService : ICancellationTokenService
{
    public TokenResult CreateToken(CancellationToken token, Action? disposeAction = null)
    {
        var tokenSource = CancellationTokenSource.CreateLinkedTokenSource(token);

        if (disposeAction != default)
            tokenSource.Token.Register(disposeAction);

        return new TokenResult(tokenSource);
    }
}
