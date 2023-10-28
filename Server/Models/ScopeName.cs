namespace Server.Models;

public sealed record ScopeName
{
    public static readonly ScopeName Catalog = new("catalog");
    public static readonly ScopeName Any = new("*");

    private readonly string _name;

    private ScopeName(string name)
    {
        _name = name;
    }

    public override string ToString() => _name;

    public static implicit operator string(ScopeName type) => type._name;

    public static implicit operator ScopeName(string type) => new(type);
}
