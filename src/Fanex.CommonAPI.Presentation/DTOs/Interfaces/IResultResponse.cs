namespace Fanex.CommonAPI.Presentation.DTOs.Interfaces;

public interface IResultResponse<T> where T : class
{
    public int ErrorCode { get; set; }

    public string ErrorMessage { get; set; }

    public T Result { get; set; }
}