namespace Asun.Platform.Evidence;

public readonly record struct EvidenceHandle(string Value)
{
    public bool IsValid =>
        !string.IsNullOrWhiteSpace(Value);

    public static EvidenceHandle Create(string value)
    {
        if(string.IsNullOrWhiteSpace(value))
            throw new ArgumentException(
                "Evidence handle cannot be blank.",
                nameof(value));

        return new EvidenceHandle(value.Trim());
    }

    public override string ToString()=>Value;
}
