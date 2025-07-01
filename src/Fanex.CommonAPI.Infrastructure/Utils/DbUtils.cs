namespace Fanex.CommonAPI.Infrastructure.Utils;

using System.Collections;
using Constants;

public static class DbUtils
{
    public static (int code, string message) GetErrorOutputParameters(Hashtable output)
    {
        var errorCode = Converter.ToInt(output[DbParam.ErrCode]);
        var errorMessage = Converter.ToString(output[DbParam.ErrMsg]);

        return (errorCode, errorMessage);
    }
}