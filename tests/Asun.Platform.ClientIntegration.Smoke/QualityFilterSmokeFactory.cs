
using Asun.Platform.ClientIntegration;

namespace Asun.Platform.ClientIntegration.Smoke;

internal static class QualityFilterSmokeFactory
{
    public static ClientQualityWorkspaceSnapshot Create() =>
        new(
            Guid.Parse("85000000-0000-0000-0000-000000000001"),
            1,
            4,
            1,
            1,
            1,
            4,
            new string('a',64),
            true,
            new[]
            {
                new ClientQualityFindingDisplayItem("F1","R1","Pass","None","pass",1),
                new ClientQualityFindingDisplayItem("F2","R2","Fail","Critical","fail",1),
                new ClientQualityFindingDisplayItem("F3","R3","Review","Warning","review",2),
                new ClientQualityFindingDisplayItem("F4","R4","Fail","Warning","fail-warning",0)
            });
}
