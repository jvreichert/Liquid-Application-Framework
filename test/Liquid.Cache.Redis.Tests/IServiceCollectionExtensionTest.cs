using Liquid.Cache.Redis.Extensions.DependencyInjection;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using System.Linq;
using Xunit;

namespace Liquid.Cache.Redis.Tests
{
    public class IServiceCollectionExtensionTest
    {
        private IServiceCollection _sut;
        private IConfiguration _configProvider = Substitute.For<IConfiguration>();
        private IConfigurationSection _configurationSection = Substitute.For<IConfigurationSection>();
        private readonly IDistributedCache _distributedCache = Substitute.For<IDistributedCache>();

        private void SetCollection()
        {
            _configProvider.GetSection(Arg.Any<string>()).Returns(_configurationSection);
            _sut = new ServiceCollection();
            _sut.AddSingleton(_configProvider);
        }

        [Fact]
        public void AddLiquidRedisDistributedCache_WhenWithTelemetryTrue_GetServicesReturnLiqudCache()
        {
            SetCollection();
            _sut.AddSingleton(_distributedCache);
            _sut.AddLogging();
            _sut.AddLiquidRedisDistributedCache(options =>
            {
                options.Configuration = _configProvider.GetConnectionString("Test");
                options.InstanceName = "TestInstance";
            }, true);

            var provider = _sut.BuildServiceProvider();

            Assert.NotNull(provider.GetService<ILiquidCache>());
            Assert.NotNull(_sut.FirstOrDefault(x => x.ServiceType == typeof(ILiquidCache) && x.Lifetime == ServiceLifetime.Scoped));
            Assert.NotNull(_sut.FirstOrDefault(x => x.ImplementationType == typeof(RedisCache)));

        }

        [Fact]
        public void AddLiquidDistributedCache_WhenWithTelemetryfalse_GetServicesReturnLiqudCache()
        {
            SetCollection();
            _sut.AddSingleton(_distributedCache);
            _sut.AddLiquidRedisDistributedCache(options =>
            {
                options.Configuration = _configProvider.GetConnectionString("Test");
                options.InstanceName = "TestInstance";
            }, false);

            var provider = _sut.BuildServiceProvider();

            Assert.NotNull(provider.GetService<ILiquidCache>());
            Assert.NotNull(_sut.FirstOrDefault(x => x.ServiceType == typeof(ILiquidCache) && x.Lifetime == ServiceLifetime.Scoped));
            Assert.NotNull(_sut.FirstOrDefault(x => x.ImplementationType == typeof(RedisCache)));

        }
    }
}