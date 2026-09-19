namespace Asun.Program.Core;

public sealed record ProgramParameter(
    string Key,
    string Value)
{
    public bool IsValid=>
        !string.IsNullOrWhiteSpace(Key);
}
