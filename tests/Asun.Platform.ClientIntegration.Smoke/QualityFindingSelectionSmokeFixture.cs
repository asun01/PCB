namespace Asun.Platform.ClientIntegration.Smoke;

internal static class QualityFindingSelectionSmokeFixture
{
    public static IReadOnlyList<ClientQualityFindingDisplayItem> Create() =>
        new[]
        {
            new ClientQualityFindingDisplayItem("F1","R1","Pass","None","pass",1),
            new ClientQualityFindingDisplayItem("F2","R2","Fail","Critical","fail",2),
            new ClientQualityFindingDisplayItem("F3","R3","Review","Warning","review",0)
        };
}
