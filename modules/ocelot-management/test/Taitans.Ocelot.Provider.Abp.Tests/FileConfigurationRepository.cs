﻿using Shouldly;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Moq;
using Ocelot.Cache;
using Ocelot.Configuration.File;
using Ocelot.Configuration.Repository;
using Ocelot.Logging;
using Shouldly;
using Taitans.Abp.OcelotManagement;
using Taitans.Ocelot.Provider.Abp.Configuration;
using Taitans.Ocelot.Provider.Abp.Repository;
using Xunit;

namespace Taitans.Ocelot.Provider.Abp.Tests
{

    public class FileConfigurationRepository : AbpOcelotProviderAbpTestBase
    {
        private readonly Mock<IOcelotCache<FileConfiguration>> _cachOptions;
        private readonly ConfigCacheOptions _configCacheOptions;
        private readonly Mock<IOcelotCache<FileConfiguration>> _fileConfiguration;
        private readonly IOcelotRepository _ocelotRepository;
        private readonly Mock<IOcelotLoggerFactory> _loggerFactory;
        private IFileConfigurationRepository _fileConfigRepository;

        public FileConfigurationRepository()
        {
            _cachOptions = new Mock<IOcelotCache<FileConfiguration>>();
            _fileConfiguration = new Mock<IOcelotCache<FileConfiguration>>();
            _ocelotRepository = GetRequiredService<IOcelotRepository>();
            _loggerFactory = new Mock<IOcelotLoggerFactory>();
            var logger = new Mock<IOcelotLogger>();
            _loggerFactory.Setup(x => x.CreateLogger<AbpEfCoreFileConfigurationRepository>()).Returns(logger.Object);
            _configCacheOptions = new ConfigCacheOptions { GatewayName = "middleware" };
        }

        private void GivenIHaveAConfiguration()
        {
            _fileConfigRepository = new AbpEfCoreFileConfigurationRepository(_configCacheOptions, _cachOptions.Object, _ocelotRepository, _loggerFactory.Object);
        }

        [Fact]
        public async Task Should_Get_Config()
        {
            GivenIHaveAConfiguration();
            var response = await _fileConfigRepository.Get();
            response.ShouldNotBeNull();
            response.Errors.Count.ShouldBe(0);
            response.IsError.ShouldBe(false);
            var routes = response.Data.ReRoutes;
            var reRoute = routes.FirstOrDefault(c => c.UpstreamPathTemplate.Equals("/connect/token"));
            reRoute.Timeout.ShouldBe(4399);
            reRoute.Priority.ShouldBe(3389);
            reRoute.DelegatingHandlers.Count.ShouldBeGreaterThan(0);
            reRoute.DelegatingHandlers[0].ShouldBe("Taitans");
            reRoute.Key.ShouldBe("WO_CAO");
            reRoute.UpstreamHost.ShouldBe("http://www.taitans.com");
            reRoute.DownstreamHostAndPorts.ShouldNotBeNull();

            reRoute.HttpHandlerOptions.ShouldNotBeNull();
            reRoute.HttpHandlerOptions.AllowAutoRedirect.ShouldBe(true);
            reRoute.HttpHandlerOptions.UseCookieContainer.ShouldBe(true);
            reRoute.HttpHandlerOptions.UseProxy.ShouldBe(true);
            reRoute.HttpHandlerOptions.UseTracing.ShouldBe(true);

            reRoute.AuthenticationOptions.ShouldNotBeNull();
            reRoute.AuthenticationOptions.AllowedScopes.Count.ShouldBe(1);
            reRoute.RateLimitOptions.ShouldNotBeNull();
            reRoute.RateLimitOptions.ClientWhitelist.Count.ShouldBe(1);
            reRoute.LoadBalancerOptions.ShouldNotBeNull();
            reRoute.LoadBalancerOptions.Expiry.ShouldBe(95);
            reRoute.LoadBalancerOptions.Key.ShouldBe("Taitans");
            reRoute.LoadBalancerOptions.Type.ShouldBe("www.taitans.com");
            reRoute.QoSOptions.ShouldNotBeNull();
            reRoute.QoSOptions.DurationOfBreak.ShouldBe(24300);
            reRoute.QoSOptions.ExceptionsAllowedBeforeBreaking.ShouldBe(802);
            reRoute.QoSOptions.TimeoutValue.ShouldBe(30624);
            reRoute.DownstreamScheme.ShouldBe("http");
            reRoute.ServiceName.ShouldBe("taitans-cn");
            reRoute.ReRouteIsCaseSensitive.ShouldBe(true);
            reRoute.FileCacheOptions.ShouldNotBeNull();
            reRoute.FileCacheOptions.TtlSeconds.ShouldBe(2020);
            reRoute.FileCacheOptions.Region.ShouldBe("github.com/taitans");
            reRoute.RequestIdKey.ShouldNotBeNull("ttgzs.cn");
            reRoute.AddQueriesToRequest.ShouldContainKeyAndValue("NB", "www.taitans.com");
            reRoute.RouteClaimsRequirement.ShouldContainKeyAndValue("MVP", "www.taitans.com");
            reRoute.AddClaimsToRequest.ShouldContainKeyAndValue("AT", "www.taitans.com");
            reRoute.DownstreamHeaderTransform.ShouldContainKeyAndValue("CVT", "www.taitans.com");
            reRoute.UpstreamHeaderTransform.ShouldContainKeyAndValue("DCT", "www.taitans.com");
            reRoute.AddHeadersToRequest.ShouldContainKeyAndValue("Trubost", "www.taitans.com");
            reRoute.ChangeDownstreamPathTemplate.ShouldContainKeyAndValue("EVCT", "www.taitans.com");
            reRoute.UpstreamHost.ShouldBe("http://www.taitans.com");
            reRoute.UpstreamHttpMethod.Count.ShouldBe(1);
            reRoute.UpstreamPathTemplate.ShouldBe("/connect/token");
            reRoute.DownstreamPathTemplate.ShouldBe("/connect/token");
            reRoute.DangerousAcceptAnyServerCertificateValidator.ShouldBe(true);
            reRoute.SecurityOptions.IPAllowedList.Count.ShouldBe(1);
            reRoute.SecurityOptions.IPBlockedList.Count.ShouldBe(1);
        }
    }
}
