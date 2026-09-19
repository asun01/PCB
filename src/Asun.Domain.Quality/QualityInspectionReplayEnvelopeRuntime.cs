namespace Asun.Domain.Quality;

public static class QualityInspectionReplayEnvelopeRuntime
{
    public static QualityInspectionReplayEnvelope Create(
        QualityInspectionReplayBundle bundle)
    {
        ArgumentNullException.ThrowIfNull(bundle);

        var fingerprint =
            QualityInspectionReplayBundleFingerprintRuntime
                .CreateFingerprint(bundle);

        return new QualityInspectionReplayEnvelope(
            bundle,
            fingerprint);
    }
}
