namespace Fanex.CommonAPI.Domain.Entities;

public class BetLimit
{
    public int ProductId { get; set; }

    public int SportType { get; set; }

    public int BetType { get; set; }

    public decimal? MinBet { get; set; }

    public decimal? MaxBet { get; set; }

    public decimal? MaxBetPerMatch { get; set; }

    public decimal? MaxPayoutPerMatch { get; set; }

    public decimal? MaxBetPerBall { get; set; }
}