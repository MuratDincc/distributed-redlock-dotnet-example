using Microsoft.AspNetCore.Mvc;
using RedLockNet.SERedis;
using Wallet.Api.Models.Wallet.Response;
using Wallet.Api.Constant;

namespace Wallet.Api.Controllers;

[ApiController]
[Route("api/v1/wallets")]
public class WalletController : ControllerBase
{
    private readonly ILogger<WalletController> _logger;

    public WalletController(ILogger<WalletController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    [ProducesResponseType(typeof(GetWalletResponse), 200)]
    public async Task<IActionResult> Get()
    {
        using (var redLockFactory = RedLockFactory.Create(RedisConstants.RedisEndpoints))
        {
            var resource = $"wallet-{WalletConstants.WalletId}";
            var expiry = TimeSpan.FromSeconds(30);

            using (var redLock = redLockFactory.CreateLock(resource, expiry))
            {
                // Lock statüsü IsAcquired parametresi altýnda dönmektedir.
                if (redLock.IsAcquired)
                {
                    return Ok(new GetWalletResponse
                    {
                        Balance = 400
                    });
                }
                else
                {
                    _logger.LogInformation("WalletController - Error - I can't access the wallet");

                    return BadRequest();
                }
            }
        }
    }
}