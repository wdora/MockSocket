﻿namespace MockSocket.Core.Extensions
{
    public static class ValueTaskExtension
    {
        public static async ValueTask<T> TimeoutAsync<T>(this ValueTask<T> actual, Func<T> degrade, int maxTimeSeconds)
        {
            var delay = Task.Delay(TimeSpan.FromSeconds(maxTimeSeconds))
                    .ContinueWith(t => degrade());

            return await await Task.WhenAny(actual.AsTask(), delay);
        }
    }
}