using Asun.UI.Viewports;

public static class ViewportReplayPresentationEvidenceGateHundredStageSmoke
{
    public static void Run(Action<bool, string> assert)
    {
        static (
            ViewportRenderEvidenceManifest[] Evidence,
            ViewportPresentationAuditEvent[] Audit)
            CreateEvidence(int variant = 0)
        {
            var evidence =
                new ViewportRenderEvidenceManifest[]
                {
                    new(
                        1,
                        1,
                        ViewportDirtyFlags.Image,
                        2 + variant,
                        1,
                        1,
                        1,
                        0,
                        0,
                        0,
                        2 + variant,
                        2 + variant,
                        0,
                        new string('a', 64),
                        new string('b', 64),
                        new string('c', 64),
                        new string('d', 64)),
                    new(
                        2,
                        2,
                        ViewportDirtyFlags.Roi,
                        3,
                        1,
                        0,
                        2,
                        1,
                        0,
                        0,
                        3,
                        3,
                        0,
                        new string('e', 64),
                        new string('f', 64),
                        new string('g', 64),
                        new string('h', 64))
                };

            var audit =
                new ViewportPresentationAuditEvent[]
                {
                    new(
                        1,
                        "Completed",
                        1,
                        1,
                        ViewportRenderDeliveryStatus.Succeeded,
                        2 + variant,
                        0,
                        evidence[0].StableKey),
                    new(
                        2,
                        "Presented",
                        2,
                        2,
                        ViewportRenderDeliveryStatus.Succeeded,
                        3,
                        0,
                        evidence[1].StableKey)
                };

            return (evidence, audit);
        }

        var round = 0;

        void Check(bool condition, string message)
        {
            round++;
            assert(condition, $"Round {round}: {message}");
        }

        for (var i = 0; i < 10; i++)
        {
            var expected = CreateEvidence();
            var actual = CreateEvidence();
            var result =
                ViewportReplayPresentationEvidenceGateRuntime.Verify(
                    expected.Evidence,
                    actual.Evidence,
                    expected.Audit,
                    actual.Audit);

            Check(
                result.IsEquivalent &&
                ViewportReplayPresentationEvidenceGateRuntime.IsEquivalent(result),
                $"clean evidence gate {i + 1} should pass.");
        }

        for (var i = 0; i < 10; i++)
        {
            var expected = CreateEvidence();
            var actual = CreateEvidence(i + 1);
            var result =
                ViewportReplayPresentationEvidenceGateRuntime.Verify(
                    expected.Evidence,
                    actual.Evidence,
                    expected.Audit,
                    actual.Audit);

            Check(
                !result.IsEquivalent &&
                !result.EvidenceMatches &&
                result.AuditMatches &&
                result.OrderingMatches,
                $"evidence mutation gate {i + 1} should isolate evidence.");
        }

        for (var i = 0; i < 10; i++)
        {
            var expected = CreateEvidence();
            var actual = CreateEvidence();

            actual.Audit[1] =
                actual.Audit[1] with
                {
                    Stage = $"Presented-{i}"
                };

            var result =
                ViewportReplayPresentationEvidenceGateRuntime.Verify(
                    expected.Evidence,
                    actual.Evidence,
                    expected.Audit,
                    actual.Audit);

            Check(
                !result.IsEquivalent &&
                result.EvidenceMatches &&
                !result.AuditMatches &&
                result.OrderingMatches,
                $"audit mutation gate {i + 1} should isolate audit.");
        }

        for (var i = 0; i < 10; i++)
        {
            var expected = CreateEvidence();
            var actual = CreateEvidence();

            var result =
                ViewportReplayPresentationEvidenceGateRuntime.Verify(
                    expected.Evidence[..1],
                    actual.Evidence,
                    expected.Audit,
                    actual.Audit);

            Check(
                !result.IsEquivalent &&
                !result.EvidenceMatches &&
                result.AuditMatches,
                $"evidence count mismatch {i + 1} should be detected.");
        }

        for (var i = 0; i < 10; i++)
        {
            var expected = CreateEvidence();
            var actual = CreateEvidence();

            var result =
                ViewportReplayPresentationEvidenceGateRuntime.Verify(
                    expected.Evidence,
                    actual.Evidence,
                    expected.Audit[..1],
                    actual.Audit);

            Check(
                !result.IsEquivalent &&
                result.EvidenceMatches &&
                !result.AuditMatches,
                $"audit count mismatch {i + 1} should be detected.");
        }

        for (var i = 0; i < 10; i++)
        {
            var expected = CreateEvidence();
            var actual = CreateEvidence();

            actual.Evidence[1] =
                actual.Evidence[1] with
                {
                    Generation = actual.Evidence[0].Generation
                };

            actual.Audit[1] =
                actual.Audit[1] with
                {
                    Sequence = actual.Audit[0].Sequence
                };

            var result =
                ViewportReplayPresentationEvidenceGateRuntime.Verify(
                    expected.Evidence,
                    actual.Evidence,
                    expected.Audit,
                    actual.Audit);

            Check(
                !result.IsEquivalent &&
                !result.OrderingMatches,
                $"ordering mutation {i + 1} should fail the gate.");
        }

        for (var i = 0; i < 10; i++)
        {
            var expected = CreateEvidence();
            var result =
                ViewportReplayPresentationEvidenceGateRuntime.Verify(
                    expected.Evidence,
                    expected.Evidence,
                    expected.Audit,
                    expected.Audit);

            Check(
                result.Differences.Count == 0 &&
                result.IsEquivalent,
                $"self verification {i + 1} should be exact.");
        }

        for (var i = 0; i < 10; i++)
        {
            var expected = CreateEvidence();
            var emptyEvidence =
                Array.Empty<ViewportRenderEvidenceManifest>();
            var emptyAudit =
                Array.Empty<ViewportPresentationAuditEvent>();

            var result =
                ViewportReplayPresentationEvidenceGateRuntime.Verify(
                    emptyEvidence,
                    emptyEvidence,
                    emptyAudit,
                    emptyAudit);

            Check(
                result.IsEquivalent &&
                result.EvidenceMatches &&
                result.AuditMatches &&
                result.OrderingMatches,
                $"empty evidence gate {i + 1} should pass.");
        }

        for (var i = 0; i < 10; i++)
        {
            var expected = CreateEvidence();
            var actual = CreateEvidence();

            actual.Evidence[0] =
                actual.Evidence[0] with
                {
                    ReplayHash = new string((char)('z' - i), 64)
                };

            var result =
                ViewportReplayPresentationEvidenceGateRuntime.Verify(
                    expected.Evidence,
                    actual.Evidence,
                    expected.Audit,
                    actual.Audit);

            Check(
                !result.IsEquivalent &&
                !result.EvidenceMatches &&
                result.Differences.Any(
                    item => item.Contains(
                        "ReplayHash",
                        StringComparison.Ordinal)),
                $"replay evidence hash {i + 1} should be visible.");
        }

        for (var i = 0; i < 10; i++)
        {
            var expected = CreateEvidence();
            var actual = CreateEvidence();

            actual.Audit[0] =
                actual.Audit[0] with
                {
                    EvidenceKey = $"missing-{i}"
                };

            var result =
                ViewportReplayPresentationEvidenceGateRuntime.Verify(
                    expected.Evidence,
                    actual.Evidence,
                    expected.Audit,
                    actual.Audit);

            Check(
                !result.IsEquivalent &&
                !result.AuditMatches &&
                result.Differences.Any(
                    item => item.Contains(
                        "EvidenceKey",
                        StringComparison.Ordinal)),
                $"audit evidence linkage {i + 1} should be visible.");
        }

        assert(
            round == 100,
            $"Presentation evidence gate smoke should execute exactly 100 numbered rounds; actual {round}.");
    }
}
