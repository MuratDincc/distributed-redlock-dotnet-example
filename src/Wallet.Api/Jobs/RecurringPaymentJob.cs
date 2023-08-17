﻿using Quartz;
using RedLockNet.SERedis;
using Wallet.Api.Constant;

namespace Wallet.Api.Jobs;

public class RecurringPaymentJob : IJob
{
    private ILogger<RecurringPaymentJob> _logger;

    public RecurringPaymentJob(ILogger<RecurringPaymentJob> logger)
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
                    _logger.LogInformation("RecurringPaymentJob - Info - Lock OK");

                    Thread.Sleep(TimeSpan.FromSeconds(10));
                }
                else
                {
                    _logger.LogInformation("RecurringPaymentJob - Error - I can't access the wallet");
                }
            }
        }

        _logger.LogInformation("RecurringPaymentJob - Lock Release OK");

        await Task.CompletedTask;
    }
}