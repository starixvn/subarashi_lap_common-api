namespace Fanex.CommonAPI.Domain.Params;

public class GetProductAccessParam
{
    public int LicenseeId { get; set; }

    public string FunctionKey { get; set; }

    public string Language { get; set; }
}