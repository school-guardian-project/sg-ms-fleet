using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using ms_fleet.Api.Infrastructure.Grpc;
using NUnit.Framework;

namespace ms_fleet.Api.Tests.Infrastructure;

[FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
public class InternalApiKeyInterceptorTests
{
    private const string ExpectedKey = "super-secret-shared-key";

    private static InternalApiKeyInterceptor Create(string? configuredKey)
    {
        var values = new Dictionary<string, string?>();
        if (configuredKey is not null) values["Fleet:ApiKey"] = configuredKey;

        return new InternalApiKeyInterceptor(
            new ConfigurationBuilder().AddInMemoryCollection(values).Build());
    }

    [Test]
    public void IsValidKey_CorrectKey_ReturnsTrue()
    {
        Assert.That(Create(ExpectedKey).IsValidKey(ExpectedKey), Is.True);
    }

    [Test]
    public void IsValidKey_WrongKey_ReturnsFalse()
    {
        Assert.That(Create(ExpectedKey).IsValidKey("wrong-key"), Is.False);
    }

    [Test]
    public void IsValidKey_PrefixIsNotEnough_ReturnsFalse()
    {
        Assert.That(Create(ExpectedKey).IsValidKey(ExpectedKey[..^1]), Is.False);
        Assert.That(Create(ExpectedKey).IsValidKey(ExpectedKey + "-extra"), Is.False);
    }

    [Test]
    public void IsValidKey_MissingHeader_ReturnsFalse()
    {
        Assert.That(Create(ExpectedKey).IsValidKey(null), Is.False);
    }

    [Test]
    public void IsValidKey_WithoutConfiguredKey_FailsClosed()
    {
        // Sin secreto configurado no se deja pasar nada (ni la clave vacia).
        var interceptor = Create(null);

        Assert.That(interceptor.IsValidKey(null), Is.False);
        Assert.That(interceptor.IsValidKey(string.Empty), Is.False);
        Assert.That(interceptor.IsValidKey(ExpectedKey), Is.False);
    }

    [Test]
    public void IsValidKey_ConfiguredEmptyKey_FailsClosed()
    {
        var interceptor = Create(string.Empty);

        Assert.That(interceptor.IsValidKey(string.Empty), Is.False);
    }
}
