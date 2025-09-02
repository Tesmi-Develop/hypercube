using System.Reflection;
using Hypercube.Utilities.Constants;
using Hypercube.Utilities.Helpers;

namespace Hypercube.Core;

[PublicAPI]
public static class EngineInfo
{
    public static readonly string Name = "Hypercube"; 
    public static readonly string Version = Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? string.Empty;
    public static readonly string Configuration = AssemblyHelper.GetAttribute<AssemblyConfigurationAttribute>()?.Configuration ?? string.Empty;
    public static readonly string WelcomeMessage =
    $"""
    {Ansi.SeroburoMalinovy256}{Ansi.Bold}Name: {Name} ({Configuration}){Ansi.Reset}            
    {Ansi.SeroburoMalinovy256}{Ansi.Bold}Version: {Version}{Ansi.Reset}
    """;
}