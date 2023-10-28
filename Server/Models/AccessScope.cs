using System.Collections.Generic;
using System.Text;

namespace Server.Models;

public sealed record AccessScope(ScopeType Type, ScopeName Name, IReadOnlyCollection<ScopeAction> Actions)
{
    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.Append(Type);
        sb.Append(':');
        sb.Append(Name);
        sb.Append(':');
        sb.AppendJoin(',', Actions);

        return sb.ToString();
    }
}
