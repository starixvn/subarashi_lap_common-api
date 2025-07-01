using System.Collections.Generic;

namespace Fanex.CommonAPI.Presentation.Services;

public interface IProductResourceService
{
    Dictionary<string, string> GetProductResources(string language);
}