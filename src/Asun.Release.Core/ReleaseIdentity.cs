namespace Asun.Release.Core;

public sealed record ReleaseIdentity(
    string ProductName,
    Version Version,
    string Channel)
{
    public bool IsValid=>
        !string.IsNullOrWhiteSpace(ProductName) &&
        Version is not null &&
        !string.IsNullOrWhiteSpace(Channel);
}
