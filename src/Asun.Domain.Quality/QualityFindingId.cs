namespace Asun.Domain.Quality;

public readonly record struct QualityFindingId(string Value)
{
    public bool IsValid =>
        !string.IsNullOrWhiteSpace(Value);

    public static QualityFindingId Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Finding id cannot be blank.", nameof(value));

        return new QualityFindingId(value.Trim());
    }

    public override string ToString() => Value;
}
