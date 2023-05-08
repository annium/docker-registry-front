namespace Server.Models;

public sealed record ScopeType
{
    public static readonly ScopeType Registry = new("registry");
    public static readonly ScopeType Repository = new("repository");

    private readonly string _name;

    private ScopeType(string name)
    {
        _name = name;
    }

    public override string ToString() => _name;

    public static implicit operator string(ScopeType type) => type._name;
    public static implicit operator ScopeType(string type) => new(type);
}