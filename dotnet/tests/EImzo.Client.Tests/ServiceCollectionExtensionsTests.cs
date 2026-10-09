using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace EImzo.Client.Tests;

public class ServiceCollectionExtensionsTests
{
    [Fact]
    public void AddEImzoClient_ShouldRegisterClientAndOptions()
    {
        var services = new ServiceCollection();

        services.AddEImzoClient(options =>
        {
            options.BaseUrl = new Uri("https://custom-server:8443/");
            options.DefaultHost = "myportal.uz";
            options.DefaultRealIp = "10.0.0.1";
        });

        var provider = services.BuildServiceProvider();
        var client = provider.GetService<IEImzoClient>();

        Assert.NotNull(client);
        Assert.IsType<EImzoClient>(client);
    }
}
