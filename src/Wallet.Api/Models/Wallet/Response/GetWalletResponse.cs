namespace Wallet.Api.Models.Wallet.Response;

public record GetWalletResponse
{
    public decimal Balance { get; set; }
}