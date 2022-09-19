using MediatR;
using MockSocket.Cache;
using MockSocket.Core.Exchange;
using MockSocket.Core.Tcp;

namespace MockSocket.Message.Tcp
{
    public class DataAgent_HoleServer_Init_Message : TcpBaseMessage, IRequest
    {
        public string UserClientId { get; set; } = "";
    }

    public class DataAgentInitHandle : IRequestHandler<DataAgent_HoleServer_Init_Message>
    {
        ICacheService cacheService;

        IExchangeConnection exchangeConnection;

        public DataAgentInitHandle(ICacheService cacheService, IExchangeConnection exchangeConnection)
        {
            this.cacheService = cacheService;
            this.exchangeConnection = exchangeConnection;
        }

        public async Task<Unit> Handle(DataAgent_HoleServer_Init_Message request, CancellationToken cancellationToken)
        {
            var userClient = cacheService.Get<ITcpConnection>(request.UserClientId);

            var dataClient = request.Connection;

            if (dataClient == null)
                throw new ArgumentNullException(nameof(dataClient));

            await exchangeConnection.ExchangeAsync(userClient, dataClient, cancellationToken);

            cacheService.Remove(request.UserClientId);

            return Unit.Value;
        }
    }
}
