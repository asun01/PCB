using System.Security.Cryptography;

namespace Asun.Device.Contracts;

public sealed record CapturedFrame(
    FrameCaptureMetadata Metadata,
    byte[] Payload,
    string PayloadFingerprint)
{
    public static CapturedFrame Create(
        FrameCaptureMetadata metadata,
        byte[] payload)
    {
        ArgumentNullException.ThrowIfNull(metadata);
        ArgumentNullException.ThrowIfNull(payload);

        if(!metadata.IsValid)
            throw new ArgumentException("Frame metadata is invalid.",nameof(metadata));

        if(payload.Length==0)
            throw new ArgumentException("Frame payload cannot be empty.",nameof(payload));

        var fingerprint=Convert.ToHexString(
            SHA256.HashData(payload))
            .ToLowerInvariant();

        return new CapturedFrame(
            metadata,
            payload.ToArray(),
            fingerprint);
    }
}
