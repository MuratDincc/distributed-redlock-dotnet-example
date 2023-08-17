using Quartz;
using RedLockNet.SERedis;
using Wallet.Api.Constant;

namespace Wallet.Api.Jobs;

public class BonusCalculatorJob : IJob
{
    private ILogger<BonusCalculatorJob> _logger;

    public BonusCalculatorJob(ILogger<BonusCalculatorJob> logger)
    {
        _logger = logger;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        using (var redLockFactory = RedLockFactory.Create(RedisConstants.RedisEndpoints))
        {
            var resource = $"wallet-{WalletConstants.WalletId}";
            var expiry = TimeSpan.FromSeconds(30);

            using (var redLock = redLockFactory.CreateLock(resource, expiry))
            {
                if (redLock.IsAcquired)
                {
                    _logger.LogInformation("BonusCalculatorJob - Info - Lock OK");

                    Thread.Sleep(TimeSpan.FromSeconds(10));
                }
                else
                {
                    _logger.LogInformation("BonusCalculatorJob - Error - I can't access the wallet");
                }
            }
        }

        _logger.LogInformation("BonusCalculatorJob - Lock Release OK");

        await Task.CompletedTask;
    }
}