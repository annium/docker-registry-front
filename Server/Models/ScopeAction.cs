using System.Collections.Immutable;

namespace Server.Models;

public sealed record ScopeAction
{
    public static bool IsKnown(ScopeAction action) => KnownActions.Contains(action);
    public static readonly ScopeAction Push = new("push");
    public static readonly ScopeAction Pull = new("pull");
    public static readonly ScopeAction Delete = new("delete");
    public static readonly ScopeAction Any = new("*");
    private static readonly ImmutableArray<ScopeAction> KnownActions = ImmutableArray.Create(Push, Pull, Delete, Any);

    private readonly string _name;

    private ScopeAction(string name)
    {
        _name = name;
    }

    public override string ToString() => _name;

    public static implicit operator string(ScopeAction type) => type._name;
    public static implicit operator ScopeAction(string type) => new(type);
}