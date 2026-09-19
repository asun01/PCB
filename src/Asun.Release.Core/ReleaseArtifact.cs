namespace Asun.Release.Core;

public sealed record ReleaseArtifact(
    string Path,
    string Sha256,
    long ByteLength)
{
    public bool IsValid=>
        !string.IsNullOrWhiteSpace(Path) &&
        ByteLength>=0 &&
        Sha256.Length==64 &&
        Sha256.All(character=>
            Uri.IsHexDigit(character) &&
            char.ToLowerInvariant(character)==character);
}
