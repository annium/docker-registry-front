using Microsoft.JSInterop;

namespace Site.Shared.Storage;

public class LocalStorage : StorageBase
{
    public LocalStorage(IJSRuntime js) : base(js, "localStorage")
    {
    }
}