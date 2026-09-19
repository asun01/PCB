namespace Asun.Domain.Quality;

public readonly record struct QualityEvidenceKey(string Value)
{
    public bool IsValid =>
        !string.IsNullOrWhiteSpace(Value);

    public static QualityEvidenceKey Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Evidence key cannot be blank.", nameof(value));

        return new QualityEvidenceKey(value.Trim());
    }

    public override string ToString() => Value;
}
