using System;

namespace Server.Models;

[Flags]
public enum RepositoryAction
{
    None,
    Read,
    Write
}