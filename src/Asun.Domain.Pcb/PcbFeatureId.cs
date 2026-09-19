namespace Asun.Domain.Pcb;

public readonly record struct PcbFeatureId(string Value)
{
    public bool IsValid =>
        !string.IsNullOrWhiteSpace(Value);

    public static PcbFeatureId Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Feature id cannot be blank.", nameof(value));

        return new PcbFeatureId(value.Trim());
    }

    public override string ToString() => Value;
}
