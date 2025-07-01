namespace Fanex.CommonAPI.Domain.Entities;

public class Product
{
    public int ProductID { get; set; }

    public string ProductName { get; set; }

    public string ProductCode { get; set; }

    public bool AllowCommission { get; set; }

    public bool AllowWinloss { get; set; }

    public bool IsOfferedSingleWallet { get; set; }
}