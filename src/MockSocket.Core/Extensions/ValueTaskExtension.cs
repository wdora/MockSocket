namespace MockSocket.Core.Extensions
{
    public static class ValueTaskExtension
    {
        public static async ValueTask<T> TimeoutAsync<T>(this ValueTask<T> actual, Func<T> degrade, int maxTimeSeconds = 3)
        {
            var delay = Task.Delay(TimeSpan.FromSeconds(maxTimeSeconds))
                    .ContinueWith(t => degrade());

            try
            {
                var response = await await Task.WhenAny(actual.AsTask(), delay);

                return response;
            }
            catch (Exception)
            {
                return degrade();
            }
        }

        public static async ValueTask RetryAsync(this Func<CancellationToken, ValueTask> valueTask, int retryIntervalSeconds = 3, CancellationToken cancellationToken = default)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    await valueTask(cancellationToken);

                    return;
                }
                catch (Exception)
                {
                    var delay = TimeSpan.FromSeconds(retryIntervalSeconds);

                    await Task.Delay(delay);
                }
            }
        }
    }
}