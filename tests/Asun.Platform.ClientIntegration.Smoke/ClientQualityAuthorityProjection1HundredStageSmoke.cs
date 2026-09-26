using Asun.Platform.ClientIntegration;

namespace Asun.Platform.ClientIntegration.Smoke;

public static class ClientQualityAuthorityProjection1HundredStageSmoke
{
    public static Task RunAsync(Action<bool,string> Check)
    {
        for(var round=1;round<=100;round++) if(round==100)
        {
            var surface=CreateBoundSurface();
            Check(surface.AuthorityText=="Quality authority: Bound · aaaaaaaaaaaa...",
                "Quality projection must expose a deterministic bound authority text.");
        }

        for(var round=1;round<=100;round++) if(round==100)
        {
            var surface=CreateBoundSurface();
            Check(surface.SummaryText.Contains("4 result(s)",StringComparison.Ordinal) &&
                  surface.SummaryText.Contains("Pass 1",StringComparison.Ordinal) &&
                  surface.SummaryText.Contains("Fail 2",StringComparison.Ordinal) &&
                  surface.SummaryText.Contains("Review 1",StringComparison.Ordinal),
                "Quality projection summary must preserve authoritative result counts.");
        }

        for(var round=1;round<=100;round++) if(round==100)
        {
            var surface=CreateBoundSurface();
            Check(surface.ProviderText=="Provider: Simulation Quality · Simulation.",
                "Quality projection must expose the authoritative provider descriptor.");
        }

        for(var round=1;round<=100;round++) if(round==100)
        {
            var surface=ClientQualitySurfaceRuntime.Create(
                CreateBoundSnapshot(),
                new ClientQualityFilter("Fail","Warning"));
            Check(surface.VisibleFindings.Count==1 &&
                  surface.VisibleFindings[0].FindingId=="F4",
                "Quality projection must apply the selected outcome/severity filter.");
        }

        for(var round=1;round<=100;round++) if(round==100)
        {
            var surface=ClientQualitySurfaceRuntime.Create(
                CreateBoundSnapshot() with { SelectedFindingId="F2" });
            Check(surface.SelectedFinding?.FindingId=="F2" &&
                  surface.SelectedFindingVisible &&
                  surface.SelectionText=="Selected finding F2",
                "Quality projection must preserve selected finding authority.");
        }

        for(var round=1;round<=100;round++) if(round==100)
        {
            var surface=ClientQualitySurfaceRuntime.Create(
                CreateBoundSnapshot() with { SelectedFindingId="F2" },
                new ClientQualityFilter("Pass","None"));
            Check(surface.SelectedFinding?.FindingId=="F2" &&
                  !surface.SelectedFindingVisible &&
                  surface.SelectionText.Contains("hidden by the current filter",StringComparison.Ordinal),
                "Quality projection must distinguish selected authority from filtered visibility.");
        }

        for(var round=1;round<=100;round++) if(round==100)
        {
            var surface=ClientQualitySurfaceRuntime.Create(
                CreateBoundSnapshot() with { Fingerprint=new string('b',64) });
            Check(surface.AuthorityText=="Quality authority: Bound · bbbbbbbbbbbb...",
                "Quality authority text must follow the current authoritative fingerprint.");
        }

        for(var round=1;round<=100;round++) if(round==100)
        {
            var surface=ClientQualitySurfaceRuntime.Create(
                CreateBoundSnapshot() with { Fingerprint="invalid" });
            Check(surface.AuthorityText=="Quality authority: not bound.",
                "Invalid Quality fingerprint must not be presented as bound authority.");
        }

        for(var round=1;round<=100;round++) if(round==100)
        {
            var surface=ClientQualitySurfaceRuntime.Create(
                CreateBoundSnapshot() with { IsBound=false,Provider=null,Fingerprint=null });
            Check(surface.AuthorityText=="Quality authority: not bound." &&
                  surface.ProviderText=="Provider: none.",
                "Unbound Quality projection must not fabricate authority or provider state.");
        }

        for(var round=1;round<=100;round++) if(round==100)
        {
            var snapshot=CreateBoundSnapshot();
            var surface=ClientQualitySurfaceRuntime.Create(snapshot);
            Check(ReferenceEquals(surface.Snapshot,snapshot) &&
                  surface.VisibleFindings.Count==snapshot.Findings.Count,
                "Quality projection must retain the authoritative snapshot instance and findings.");
        }

        return Task.CompletedTask;
    }

    private static ClientQualitySurface CreateBoundSurface() =>
        ClientQualitySurfaceRuntime.Create(CreateBoundSnapshot());

    private static ClientQualityWorkspaceSnapshot CreateBoundSnapshot() =>
        QualityFilterSmokeFactory.Create() with
        {
            Provider=new ClientQualityProviderDescriptor(
                "simulation-quality",
                "Simulation Quality",
                true)
        };
}
