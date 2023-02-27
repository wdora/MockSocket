using Microsoft.Extensions.Logging;
using MockSocket.Core.Interfaces;
using Polly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace MockSocket.Core.Services
{
    public class LimitIPService : ILimitIPService
    {
        static HttpClient client = new HttpClient();

        ILogger logger;

        public LimitIPService(ILogger<LimitIPService> logger)
        {
            this.logger = logger;
        }

        public async ValueTask<bool> ValidAsync(IPAddress address)
        {
            var policy = Policy<string>
                .Handle<Exception>()
                .RetryAsync(5);
            
            var html = await policy.ExecuteAsync(() => client.GetStringAsync($"https://www.ipshudi.com/{address}.htm"));

            var area = new Regex("<td class=\"th\">归属地</td>\n<td>\n<span>(.+)</span>").Match(html).Groups[1].Value;

            logger.LogInformation($"{address} is in {area}");

            // todo replace by user setting
            return area.Contains("北京");
        }
    }
}
