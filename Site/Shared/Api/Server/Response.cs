using System.Net;

namespace Site.Shared.Api.Server;

public sealed record Response<T>(bool IsOk, T Data);