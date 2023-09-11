using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MockSocket.xUnit;
public class UpdateTest
{
    [Fact]
    public async Task CheckUpdate()
    {
        await new Updater().UpdateAsync();

        var check = await new Updater().CheckAsync("MockSocket.Agent");

        var currentVersion = Assembly.GetExecutingAssembly().GetName().Version;

        check.ShouldNotBeNull();

        check.CurrentVersion.ShouldBe(currentVersion);

        check.LatestVersion.ShouldBeGreaterThanOrEqualTo(currentVersion);
    }
}

public class Updater
{
    // MockAgent.exe
    // https://update.wdora.com/MockAgent
    // 
    // https://update.wdora.com/MockAgent/1.0.0

    // check: https://update.wdora.com/{appName}
    // download: https://update.wdora.com/{appName}/{version}

    static HttpClient client = new HttpClient();

    public async ValueTask<CheckResult> CheckAsync(string appName, CancellationToken cancellationToken = default)
    {
        try
        {
            var currentVersion = Assembly.GetExecutingAssembly().GetName().Version!;

            var json = await client.GetStringAsync($"https://update.wdora.com/{appName}", cancellationToken);

            var remoteResult = JsonSerializer.Deserialize<CheckResult>(json)!;

            return new CheckResult(currentVersion, remoteResult.LatestVersion, remoteResult.Content);
        }
        catch (Exception)
        {
            return CheckResult.Default;
        }
    }

    public async ValueTask UpdateAsync()
    {
        var appName = Assembly.GetExecutingAssembly().GetName().Name;

        appName.ShouldBeEmpty();
    }
}

public record class CheckResult(Version CurrentVersion, Version LatestVersion, string Content)
{
    public static CheckResult Default = new CheckResult(default, default, default);

    public bool ShouldUpdate => LatestVersion > CurrentVersion;
}
