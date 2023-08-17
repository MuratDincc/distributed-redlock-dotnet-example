using RedLockNet.SERedis.Configuration;
using System.Net;

namespace Wallet.Api.Constant;

public static class RedisConstants
{
    public static List<RedLockEndPoint> RedisEndpoints { get; } = new List<RedLockEndPoint>
    {
        new DnsEndPoint("host.docker.internal", 6379)
    };
}