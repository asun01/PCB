using System.Numerics;
using Asun.UI.Viewports;

public static class ViewportThreeHundredRoundSmoke
{
    public static void Run(Action<bool, string> assert)
    {
        var round = 0;

        void Check(bool condition, string message)
        {
            round++;
            assert(
                condition,
                $"300-round check #{round:000}: {message}");
        }

        var imageSize = new Vector2(2000, 1200);
        var viewportSize = new Vector2(800, 500);
        var baseTransform = ViewportTransform.Fit(
            imageSize,
            viewportSize);

        // Rounds 1-60: coordinate round-trips across a deterministic grid.
        for (var y = 0; y < 10; y++)
        {
            for (var x = 0; x < 6; x++)
            {
                var point = new Vector2(
                    25 + x * 150,
                    35 + y * 110);

                var viewportPoint = baseTransform.ImageToViewport(point);
                var imagePoint = baseTransform.ViewportToImage(viewportPoint);

                Check(
                    Vector2.Distance(point, imagePoint) < 1e-4f &&
                    baseTransform.ContainsViewportPoint(viewportPoint),
                    "image/viewport transform should round-trip a finite grid point.");
            }
        }

        // Rounds 61-100: zoom profiles and anchored zooms.
        for (var index = 0; index < 40; index++)
        {
            var anchor = new Vector2(
                40 + (index % 10) * 70,
                40 + (index / 10) * 80);

            var factor = 0.8 + index * 0.02;
            var transformed = baseTransform.WithZoomFactor(
                factor,
                anchor);

            var anchoredImagePoint =
                baseTransform.ViewportToImage(anchor);

            var roundTripViewport =
                transformed.ImageToViewport(anchoredImagePoint);

            Check(
                Vector2.Distance(anchor, roundTripViewport) < 1e-3f &&
                transformed.Scale > 0,
                "anchored zoom should preserve the image point under the cursor.");
        }

        // Rounds 101-140: ROI geometry validity and containment.
        for (var index = 0; index < 40; index++)
        {
            var center = new Vector2(
                100 + index * 25,
                120 + (index % 5) * 30);

            var size = new Vector2(
                20 + index,
                15 + index * 0.5f);

            var rectangle = RoiGeometry.CreateRectangle(center, size);
            var ellipse = RoiGeometry.CreateEllipse(center, size);
            var rotated = RoiGeometry.CreateRotatedRectangle(
                center,
                size,
                (index % 8) * 0.1f);

            Check(
                rectangle.IsValid &&
                rectangle.Contains(center) &&
                ellipse.IsValid &&
                ellipse.Contains(center) &&
                rotated.IsValid &&
                rotated.Contains(center),
                "rectangle, ellipse, and rotated rectangle ROI primitives should remain valid and contain their centers.");
        }

        // Rounds 141-160: polygon validity and vertex-preserving translations.
        for (var index = 0; index < 20; index++)
        {
            var offset = new Vector2(
                index * 3,
                index * 2);

            var polygon = RoiGeometry.CreatePolygon(
                new[]
                {
                    new Vector2(10, 10) + offset,
                    new Vector2(50, 10) + offset,
                    new Vector2(30, 60) + offset
                });

            var moved = polygon.Translate(new Vector2(5, 7));

            Check(
                polygon.IsValid &&
                moved.IsValid &&
                moved.Contains(polygon.Center + new Vector2(5, 7)) &&
                moved.Vertices.Count == 3,
                "polygon translation should preserve topology and move its center consistently.");
        }

        // Rounds 161-180: MiniMap mapping.
        for (var index = 0; index < 20; index++)
        {
            var main = baseTransform.CenterOnImagePoint(
                new Vector2(
                    200 + index * 50,
                    150 + index * 20));

            var snapshot = ViewportMiniMapRuntime.CreateSnapshot(
                main,
                new Vector2(200, 120));

            var imagePoint =
                new Vector2(
                    100 + index * 40,
                    120 + index * 15);

            var miniPoint =
                snapshot.MiniMapTransform.ImageToViewport(imagePoint);

            var recovered =
                ViewportMiniMapRuntime.MiniMapToImage(
                    snapshot,
                    miniPoint);

            Check(
                Vector2.Distance(imagePoint, recovered) < 1e-3f,
                "MiniMap image mapping should round-trip deterministically.");
        }

        // Rounds 181-220: input replay determinism.
        for (var index = 0; index < 40; index++)
        {
            var replay = new ViewportInputReplayRuntime();

            replay.Record(
                ViewportInputEventKind.PointerDown,
                new Vector2(100 + index, 80 + index),
                button: ViewportMouseButton.Middle,
                sequence: 1);

            replay.Record(
                ViewportInputEventKind.PointerMove,
                new Vector2(120 + index, 90 + index),
                button: ViewportMouseButton.Middle,
                sequence: 2);

            replay.Record(
                ViewportInputEventKind.PointerUp,
                new Vector2(120 + index, 90 + index),
                button: ViewportMouseButton.Middle,
                sequence: 3);

            var firstRuntime = new RoiViewportRuntime(
                imageSize,
                viewportSize);

            var firstInput =
                new ViewportCompositeInputRuntime<string>(firstRuntime);

            var firstResults = replay.Replay(firstInput);

            var secondRuntime = new RoiViewportRuntime(
                imageSize,
                viewportSize);

            var secondInput =
                new ViewportCompositeInputRuntime<string>(secondRuntime);

            replay.Replay(secondInput);

            Check(
                firstResults.Count == 3 &&
                firstRuntime.Transform == secondRuntime.Transform &&
                replay.EvidenceHash().Length == 64,
                "identical input traces should produce identical interaction state and evidence.");
        }

        // Rounds 221-240: workflow validation.
        for (var index = 0; index < 20; index++)
        {
            var commands = new[]
            {
                ViewportWorkflowCommand.Fit(),
                ViewportWorkflowCommand.Pan(
                    new Vector2(index, index + 1)),
                ViewportWorkflowCommand.Zoom(
                    1 + index * 0.01,
                    0.05,
                    64,
                    new Vector2(200, 150))
            };

            var validation =
                ViewportWorkflowValidator.Validate(commands);

            Check(
                validation.IsValid &&
                validation.ErrorCount == 0 &&
                ViewportInvariantRuntime.ValidateWorkflow(validation).Count == 0,
                "valid deterministic workflow batches should remain structurally valid.");
        }

        // Rounds 241-260: Evidence hashing stability.
        for (var index = 0; index < 20; index++)
        {
            var text = $"render-round|{index}|generation={index + 10}|units={index * 3}";
            var first = ViewportRenderEvidenceRuntime.ComputeTextHash(text);
            var second = ViewportRenderEvidenceRuntime.ComputeTextHash(text);
            var changed = ViewportRenderEvidenceRuntime.ComputeTextHash(
                text + "|changed");

            Check(
                first.Length == 64 &&
                first == second &&
                first != changed,
                "evidence hashing should be deterministic and sensitive to logical trace changes.");
        }

        // Rounds 261-280: Zoom profile and transform guard validation.
        for (var index = 0; index < 20; index++)
        {
            var profile = new ViewportZoomProfile(
                0.05,
                64,
                1.05 + index * 0.002,
                0.8 + index * 0.005);

            var errors =
                ViewportInvariantRuntime.ValidateZoomProfile(profile);

            var scale =
                ViewportZoomProfileRuntime.StepScale(
                    1,
                    120,
                    profile);

            Check(
                errors.Count == 0 &&
                scale > 1 &&
                scale <= profile.MaximumScale,
                "validated zoom profiles should produce bounded positive wheel-step scales.");
        }

        // Rounds 281-300: deterministic scene/evidence/invariant composition.
        for (var index = 0; index < 20; index++)
        {
            var viewport = new RoiViewportRuntime(
                imageSize,
                viewportSize);

            var id = viewport.Document.Add(
                RoiGeometry.CreateRectangle(
                    new Vector2(
                        150 + index * 10,
                        120 + index * 4),
                    new Vector2(50, 30)));

            viewport.Document.Select(id);

            var snapshot = viewport.CreateSnapshot();
            var scene = ViewportSceneRuntime.Build(snapshot);
            var sceneErrors =
                ViewportInvariantRuntime.ValidateSceneSnapshot(scene);

            var evidence =
                ViewportRenderEvidenceRuntime.ComputeTextHash(
                    string.Join(
                        "
",
                        scene.Commands.Select(command =>
                            command.ToString())));

            Check(
                scene.Commands.Any(command => command.RoiId == id) &&
                sceneErrors.Count == 0 &&
                evidence.Length == 64,
                "a deterministic ROI scene should remain valid and fingerprintable.");
        }

        assert(
            round == 300,
            "the verification matrix must execute exactly 300 numbered rounds.");
    }
}
