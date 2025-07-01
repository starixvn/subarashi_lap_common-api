namespace Fanex.CommonAPI.Infrastructure;

using System.Reflection;

public static class AssemblyReference
{
    public static readonly Assembly Assembly = typeof(AssemblyReference).Assembly;
}