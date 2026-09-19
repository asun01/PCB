namespace Asun.Domain.Quality;

public static class QualityInspectionReplayEnvelopeValidationRuntime
{
    public static IReadOnlyList<string> Validate(
        QualityInspectionReplayEnvelope envelope)
    {
        ArgumentNullException.ThrowIfNull(envelope);

        var errors = new List<string>();

        if (envelope.Bundle is null)
        {
            errors.Add("Replay envelope bundle cannot be null.");
            return errors;
        }

        errors.AddRange(
            QualityInspectionReplayBundleValidationRuntime
                .Validate(envelope.Bundle));

        errors.AddRange(
            QualityInspectionReplayBundleFingerprintValidationRuntime
                .ValidateFingerprint(envelope.BundleFingerprint));

        if (errors.Count == 0)
        {
            var expected =
                QualityInspectionReplayBundleFingerprintRuntime
                    .CreateFingerprint(envelope.Bundle);

            if (!string.Equals(
                    expected,
                    envelope.BundleFingerprint,
                    StringComparison.Ordinal))
            {
                errors.Add(
                    "Replay envelope fingerprint does not match the bundle.");
            }
        }

        return errors;
    }

    public static bool IsValid(
        QualityInspectionReplayEnvelope envelope) =>
        Validate(envelope).Count == 0;
}
