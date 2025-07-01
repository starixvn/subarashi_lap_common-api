namespace Fanex.CommonAPI.Presentation.DTOs.Request;

public class ProductAccessRequest
{
    public int LicenseeId { get; set; }

    public string FunctionKey { get; set; }

    public string Language { get; set; }
}