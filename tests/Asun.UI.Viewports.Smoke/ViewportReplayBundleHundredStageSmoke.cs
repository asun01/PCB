using System.Numerics;
using Asun.UI.Viewports;

public static class ViewportReplayBundleHundredStageSmoke
{
    public static void Run(Action<bool, string> assert)
    {
        var inputs = new[]
        {
            new ViewportInputEvent(
                1,
                ViewportInputEventKind.PointerMove,
                new Vector2(10, 20),
                0,
                ViewportMouseButton.Left),
            new ViewportInputEvent(
                2,
                ViewportInputEventKind.Wheel,
                new Vector2(30, 40),
                120,
                ViewportMouseButton.Left)
        };

        var evidence = new[]
        {
            new ViewportRenderEvidenceManifest(
                10,
                20,
                ViewportDirtyFlags.Image,
                2,
                1,
                1,
                1,
                0,
                0,
                0,
                2,
                2,
                0,
                new string('a', 64),
                new string('b', 64),
                new string('c', 64),
                new string('d', 64))
        };

        var audit = new[]
        {
            new ViewportPresentationAuditEvent(
                1,
                "Presented",
                10,
                20,
                ViewportRenderDeliveryStatus.Succeeded,
                2,
                0,
                evidence[0].StableKey)
        };

        var manifest = ViewportReplaySessionBundleRuntime.CreateManifest(
            "hundred-stage",
            DateTimeOffset.UnixEpoch,
            inputs,
            evidence,
            audit);

        var original = ViewportReplaySessionBundleRuntime.Capture(
            manifest,
            inputs,
            evidence,
            audit);

        var round = 0;

        void Check(bool condition, string message)
        {
            round++;
            assert(condition, $"Round {round}: {message}");
        }

        Check(
            original.Manifest.SessionId == "hundred-stage",
            "bundle should preserve session identity.");

        Check(
            original.Inputs.Length == 2,
            "bundle should retain all input events.");

        Check(
            original.Evidence.Length == 1,
            "bundle should retain all evidence manifests.");

        Check(
            original.Audit.Length == 1,
            "bundle should retain all audit events.");

        Check(
            original.Manifest.InputEventCount == 2,
            "manifest input count should be bound.");

        Check(
            original.Manifest.EvidenceManifestCount == 1,
            "manifest evidence count should be bound.");

        Check(
            original.Manifest.AuditEventCount == 1,
            "manifest audit count should be bound.");

        Check(
            ViewportReplaySessionBundleRuntime.Validate(original).Count == 0,
            "original bundle should validate.");

        Check(
            ViewportReplaySessionBundleRuntime.AreEquivalent(original, original),
            "bundle should equal itself.");

        for (var i = 0; i < 10; i++)
        {
            var copy = ViewportReplaySessionBundleRuntime.Capture(
                ViewportReplaySessionBundleRuntime.CreateManifest(
                    original.Manifest.SessionId,
                    original.Manifest.CreatedAtUtc,
                    original.Inputs,
                    original.Evidence,
                    original.Audit),
                original.Inputs,
                original.Evidence,
                original.Audit);

            Check(
                ViewportReplaySessionBundleRuntime.AreEquivalent(
                    original,
                    copy),
                $"deterministic reconstruction {i + 1} should remain equivalent.");
        }

        for (var i = 0; i < 10; i++)
        {
            var changed = original with
            {
                Inputs = original.Inputs
                    .Select((item, index) =>
                        index == 0
                            ? item with { Sequence = item.Sequence + i + 1 }
                            : item)
                    .ToArray()
            };

            var comparison = ViewportReplaySessionBundleRuntime.Compare(
                original,
                changed);

            Check(
                !comparison.IsEquivalent &&
                comparison.Differences.Any(
                    item => item.StartsWith(
                        "Inputs[0].Sequence:",
                        StringComparison.Ordinal)),
                $"input divergence {i + 1} should be localized.");
        }

        for (var i = 0; i < 10; i++)
        {
            var changed = original with
            {
                Evidence = original.Evidence
                    .Select(item =>
                        item with
                        {
                            FrameHash = new string(
                                (char)('e' + i % 10),
                                64)
                        })
                    .ToArray()
            };

            var comparison = ViewportReplaySessionBundleRuntime.Compare(
                original,
                changed);

            Check(
                !comparison.IsEquivalent &&
                comparison.Differences.Any(
                    item => item.StartsWith(
                        "Evidence[0].FrameHash:",
                        StringComparison.Ordinal)),
                $"evidence divergence {i + 1} should be localized.");
        }

        for (var i = 0; i < 10; i++)
        {
            var changed = original with
            {
                Audit = original.Audit
                    .Select(item =>
                        item with
                        {
                            Stage = $"Stage-{i}"
                        })
                    .ToArray()
            };

            var comparison = ViewportReplaySessionBundleRuntime.Compare(
                original,
                changed);

            Check(
                !comparison.IsEquivalent &&
                comparison.Differences.Any(
                    item => item.StartsWith(
                        "Audit[0].Stage:",
                        StringComparison.Ordinal)),
                $"audit divergence {i + 1} should be localized.");
        }

        for (var i = 0; i < 10; i++)
        {
            var changedManifest = original.Manifest with
            {
                SessionId = $"changed-{i}"
            };

            var changed = original with
            {
                Manifest = changedManifest
            };

            var comparison = ViewportReplaySessionBundleRuntime.Compare(
                original,
                changed);

            Check(
                !comparison.IsEquivalent &&
                comparison.Differences.Any(
                    item => item.StartsWith(
                        "Manifest.SessionId:",
                        StringComparison.Ordinal)),
                $"manifest divergence {i + 1} should be localized.");
        }

        var invalidInput = original with
        {
            Inputs = new[]
            {
                original.Inputs[1],
                original.Inputs[0]
            }
        };

        Check(
            ViewportReplaySessionBundleRuntime.Validate(invalidInput).Any(
                item => item.Contains(
                    "input sequence",
                    StringComparison.OrdinalIgnoreCase)),
            "reordered inputs should fail validation.");

        var invalidEvidence = original with
        {
            Evidence = new[]
            {
                original.Evidence[0] with
                {
                    DeferredUnits = original.Evidence[0].PlannedUnits + 1
                }
            }
        };

        Check(
            ViewportReplaySessionBundleRuntime.Validate(invalidEvidence).Any(
                item => item.Contains(
                    "rendered plus deferred",
                    StringComparison.OrdinalIgnoreCase)),
            "invalid evidence counters should fail validation.");

        var invalidAudit = original with
        {
            Audit = new[]
            {
                original.Audit[0],
                original.Audit[0]
            }
        };

        Check(
            ViewportReplaySessionBundleRuntime.Validate(invalidAudit).Any(
                item => item.Contains(
                    "audit sequence",
                    StringComparison.OrdinalIgnoreCase)),
            "duplicate audit sequence should fail validation.");

        var tamperedManifest = original.Manifest with
        {
            SessionHash = new string('f', 64)
        };

        var tampered = original with
        {
            Manifest = tamperedManifest
        };

        Check(
            ViewportReplaySessionBundleRuntime.Validate(tampered).Any(
                item => item.Contains(
                    "session hash",
                    StringComparison.OrdinalIgnoreCase)),
            "tampered session hash should fail validation.");

        for (var i = 0; i < 10; i++)
        {
            var reconstructedManifest =
                ViewportReplaySessionBundleRuntime.CreateManifest(
                    original.Manifest.SessionId,
                    original.Manifest.CreatedAtUtc,
                    original.Inputs,
                    original.Evidence,
                    original.Audit);

            Check(
                reconstructedManifest == original.Manifest,
                $"manifest reconstruction {i + 1} should remain deterministic.");
        }

        assert(
            round == 100,
            $"Hundred-stage replay bundle smoke should execute exactly 100 numbered rounds; actual {round}.");
    }
}
