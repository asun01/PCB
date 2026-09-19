namespace Asun.Domain.Pcb;

public sealed record PcbFeatureReference(
    PcbFeatureId Id,
    string Name,
    PcbFeatureKind Kind,
    int LayerIndex,
    PcbLayerSide Side,
    PcbCoordinate Position,
    double RotationRadians,
    double SizeXmm,
    double SizeYmm)
{
    public bool IsValid =>
        Id.IsValid &&
        !string.IsNullOrWhiteSpace(Name) &&
        Kind != PcbFeatureKind.Unknown &&
        LayerIndex >= 0 &&
        Position.IsFinite &&
        double.IsFinite(RotationRadians) &&
        double.IsFinite(SizeXmm) &&
        double.IsFinite(SizeYmm) &&
        SizeXmm >= 0 &&
        SizeYmm >= 0;
}
