using Asun.Device.Contracts;
using Asun.Device.Impl;
using Asun.Platform.CaptureEvidenceIntegration;
using Asun.Platform.Evidence;
using Asun.Platform.Pipeline;
using Asun.Production.Runtime;

namespace Asun.Platform.CaptureEvidenceIntegration.Smoke;

public sealed record CaptureEvidenceClosureFixture(
    ProductionSessionReport Production,
    CaptureSessionSnapshot Capture,
    IReadOnlyList<ProductionFrameProvenance> Provenance,
    IReadOnlyList<ProductionEvidenceFrameReference> Evidence);

public static class CaptureEvidenceClosureFixtureRuntime
{
    public static CaptureEvidenceClosureFixture Create()
    {
        var production=new ProductionSessionReport(
            Guid.Parse("a1000000-0000-0000-0000-000000000001"),
            "program-fingerprint",
            2,
            new[]
            {
                new ProductionFrameExecution(
                    FrameSequence.Create(1),
                    "1111111111111111111111111111111111111111111111111111111111111111",
                    new PipelineExecutionReport(1,new[]{"Acquire"},"pipeline-one")),
                new ProductionFrameExecution(
                    FrameSequence.Create(2),
                    "2222222222222222222222222222222222222222222222222222222222222222",
                    new PipelineExecutionReport(1,new[]{"Acquire"},"pipeline-two"))
            },
            "production-report-fingerprint");

        var capture=new CaptureSessionSnapshot(
            2,
            FrameSequence.Create(1),
            FrameSequence.Create(2),
            new[]{"1111111111111111111111111111111111111111111111111111111111111111","2222222222222222222222222222222222222222222222222222222222222222"});

        var provenance=new ProductionFrameProvenance[]
        {
            new( FrameSequence.Create(1),5,5,"Gray8",DateTimeOffset.UnixEpoch.AddSeconds(1),"1111111111111111111111111111111111111111111111111111111111111111"),
            new( FrameSequence.Create(2),5,5,"Gray8",DateTimeOffset.UnixEpoch.AddSeconds(2),"2222222222222222222222222222222222222222222222222222222222222222")
        };

        var evidence=new ProductionEvidenceFrameReference[]
        {
            new(
                1,
                new[]
                {
                    EvidenceHandle.Create("capture/1/metadata"),
                    EvidenceHandle.Create("capture/1/raw")
                }),
            new(
                2,
                new[]
                {
                    EvidenceHandle.Create("capture/2/metadata"),
                    EvidenceHandle.Create("capture/2/raw")
                })
        };

        return new CaptureEvidenceClosureFixture(
            production,
            capture,
            provenance,
            evidence);
    }
}
