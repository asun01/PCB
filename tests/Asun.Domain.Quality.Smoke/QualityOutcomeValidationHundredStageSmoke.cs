using Asun.Domain.Quality;

public static class QualityOutcomeValidationHundredStageSmoke
{
    public static void Run(Action<bool, string> assert)
    {
        var round = 0;

        void Check(bool condition, string message)
        {
            round++;
            assert(condition, $"Round {round}: {message}");
        }

        var pass = QualityOutcome.Pass;
        var fail = QualityOutcome.Fail;
        var review = QualityOutcome.Review;
        var info = QualityOutcome.Informational;
        var unknown = QualityOutcome.Unknown;

        for (var i = 0; i < 10; i++)
            Check(QualityOutcomeValidationRuntime.IsValid(pass), $"pass outcome round {i + 1} should be valid.");

        for (var i = 0; i < 10; i++)
            Check(QualityOutcomeValidationRuntime.IsValid(fail), $"fail outcome round {i + 1} should be valid.");

        for (var i = 0; i < 10; i++)
            Check(QualityOutcomeValidationRuntime.IsValid(review), $"review outcome round {i + 1} should be valid.");

        for (var i = 0; i < 10; i++)
            Check(QualityOutcomeValidationRuntime.IsValid(info), $"informational outcome round {i + 1} should be valid.");

        for (var i = 0; i < 10; i++)
            Check(!QualityOutcomeValidationRuntime.IsValid(unknown), $"unknown outcome round {i + 1} should be invalid.");

        for (var i = 0; i < 10; i++)
            Check(QualityOutcomeValidationRuntime.Validate(pass).Count == 0, $"valid outcome validation round {i + 1} should have no errors.");

        for (var i = 0; i < 10; i++)
            Check(QualityOutcomeValidationRuntime.Validate(unknown).Count > 0, $"unknown outcome validation round {i + 1} should produce evidence.");

        for (var i = 0; i < 10; i++)
            Check((int)pass == 1 && (int)fail == 2, $"outcome numeric stability round {i + 1} should remain deterministic.");

        for (var i = 0; i < 10; i++)
            Check((int)review == 3 && (int)info == 4, $"additional outcome numeric stability round {i + 1} should remain deterministic.");

        for (var i = 0; i < 10; i++)
            Check(pass != fail && fail != review && review != info, $"outcome identity round {i + 1} should remain distinct.");

        assert(round == 100, $"Quality outcome smoke should execute exactly 100 numbered rounds; actual {round}.");
    }
}
