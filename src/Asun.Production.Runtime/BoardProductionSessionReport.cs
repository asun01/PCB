namespace Asun.Production.Runtime;

public sealed record BoardProductionSessionReport(
    string AssemblyFingerprint,
    ProductionSessionReport ProductionReport,
    string Fingerprint);
