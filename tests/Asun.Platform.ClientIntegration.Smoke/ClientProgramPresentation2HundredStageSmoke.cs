using Asun.Program.Core;

namespace Asun.Platform.ClientIntegration.Smoke;

public static class ClientProgramPresentation2HundredStageSmoke
{
    public static Task RunAsync(Action<bool,string> Check)
    {
        var round=0;
        for(var iteration0=0;iteration0<10;iteration0++)
        {
            round++;
            var program=new InspectionProgram(
                Guid.Parse("82000000-0000-0000-0000-000000000001"),
                "Display Program",
                new Version(2,1,0),
                new[]
                {
                    new ProgramStep(Guid.Parse("82000000-0000-0000-0000-000000000102"),2,ProgramStepKind.Measure,"Measure",Array.Empty<ProgramParameter>()),
                    new ProgramStep(Guid.Parse("82000000-0000-0000-0000-000000000101"),1,ProgramStepKind.Acquire,"Acquire",Array.Empty<ProgramParameter>())
                });
            var items=ClientProgramPresentationRuntime.CreateItems(program);
            Check(items.Select(item=>item.Order).SequenceEqual(new[]{1,2}) &&
                  items[0].Kind==ProgramStepKind.Acquire.ToString(),
                  "Program presentation must normalize steps by canonical order");
        }
        for(var iteration1=0;iteration1<10;iteration1++)
        {
            round++;
            var program=new InspectionProgram(
                Guid.Parse("82000000-0000-0000-0000-000000000001"),
                "Display Program",
                new Version(2,1,0),
                new[]
                {
                    new ProgramStep(Guid.Parse("82000000-0000-0000-0000-000000000102"),2,ProgramStepKind.Measure,"Measure",Array.Empty<ProgramParameter>()),
                    new ProgramStep(Guid.Parse("82000000-0000-0000-0000-000000000101"),1,ProgramStepKind.Acquire,"Acquire",Array.Empty<ProgramParameter>())
                });
            var items=ClientProgramPresentationRuntime.CreateItems(program);
            Check(items.Select(item=>item.Order).SequenceEqual(new[]{1,2}) &&
                  items[0].Kind==ProgramStepKind.Acquire.ToString(),
                  "Program presentation must normalize steps by canonical order");
        }
        for(var iteration2=0;iteration2<10;iteration2++)
        {
            round++;
            var program=new InspectionProgram(
                Guid.Parse("82000000-0000-0000-0000-000000000001"),
                "Display Program",
                new Version(2,1,0),
                new[]
                {
                    new ProgramStep(Guid.Parse("82000000-0000-0000-0000-000000000102"),2,ProgramStepKind.Measure,"Measure",Array.Empty<ProgramParameter>()),
                    new ProgramStep(Guid.Parse("82000000-0000-0000-0000-000000000101"),1,ProgramStepKind.Acquire,"Acquire",Array.Empty<ProgramParameter>())
                });
            var items=ClientProgramPresentationRuntime.CreateItems(program);
            Check(items.Select(item=>item.Order).SequenceEqual(new[]{1,2}) &&
                  items[0].Kind==ProgramStepKind.Acquire.ToString(),
                  "Program presentation must normalize steps by canonical order");
        }
        for(var iteration3=0;iteration3<10;iteration3++)
        {
            round++;
            var program=new InspectionProgram(
                Guid.Parse("82000000-0000-0000-0000-000000000001"),
                "Display Program",
                new Version(2,1,0),
                new[]
                {
                    new ProgramStep(Guid.Parse("82000000-0000-0000-0000-000000000102"),2,ProgramStepKind.Measure,"Measure",Array.Empty<ProgramParameter>()),
                    new ProgramStep(Guid.Parse("82000000-0000-0000-0000-000000000101"),1,ProgramStepKind.Acquire,"Acquire",Array.Empty<ProgramParameter>())
                });
            var items=ClientProgramPresentationRuntime.CreateItems(program);
            Check(items.Select(item=>item.Order).SequenceEqual(new[]{1,2}) &&
                  items[0].Kind==ProgramStepKind.Acquire.ToString(),
                  "Program presentation must normalize steps by canonical order");
        }
        for(var iteration4=0;iteration4<10;iteration4++)
        {
            round++;
            var program=new InspectionProgram(
                Guid.Parse("82000000-0000-0000-0000-000000000001"),
                "Display Program",
                new Version(2,1,0),
                new[]
                {
                    new ProgramStep(Guid.Parse("82000000-0000-0000-0000-000000000102"),2,ProgramStepKind.Measure,"Measure",Array.Empty<ProgramParameter>()),
                    new ProgramStep(Guid.Parse("82000000-0000-0000-0000-000000000101"),1,ProgramStepKind.Acquire,"Acquire",Array.Empty<ProgramParameter>())
                });
            var items=ClientProgramPresentationRuntime.CreateItems(program);
            Check(items.Select(item=>item.Order).SequenceEqual(new[]{1,2}) &&
                  items[0].Kind==ProgramStepKind.Acquire.ToString(),
                  "Program presentation must normalize steps by canonical order");
        }
        for(var iteration5=0;iteration5<10;iteration5++)
        {
            round++;
            var program=new InspectionProgram(
                Guid.Parse("82000000-0000-0000-0000-000000000001"),
                "Display Program",
                new Version(2,1,0),
                new[]
                {
                    new ProgramStep(Guid.Parse("82000000-0000-0000-0000-000000000102"),2,ProgramStepKind.Measure,"Measure",Array.Empty<ProgramParameter>()),
                    new ProgramStep(Guid.Parse("82000000-0000-0000-0000-000000000101"),1,ProgramStepKind.Acquire,"Acquire",Array.Empty<ProgramParameter>())
                });
            var items=ClientProgramPresentationRuntime.CreateItems(program);
            Check(items.Select(item=>item.Order).SequenceEqual(new[]{1,2}) &&
                  items[0].Kind==ProgramStepKind.Acquire.ToString(),
                  "Program presentation must normalize steps by canonical order");
        }
        for(var iteration6=0;iteration6<10;iteration6++)
        {
            round++;
            var program=new InspectionProgram(
                Guid.Parse("82000000-0000-0000-0000-000000000001"),
                "Display Program",
                new Version(2,1,0),
                new[]
                {
                    new ProgramStep(Guid.Parse("82000000-0000-0000-0000-000000000102"),2,ProgramStepKind.Measure,"Measure",Array.Empty<ProgramParameter>()),
                    new ProgramStep(Guid.Parse("82000000-0000-0000-0000-000000000101"),1,ProgramStepKind.Acquire,"Acquire",Array.Empty<ProgramParameter>())
                });
            var items=ClientProgramPresentationRuntime.CreateItems(program);
            Check(items.Select(item=>item.Order).SequenceEqual(new[]{1,2}) &&
                  items[0].Kind==ProgramStepKind.Acquire.ToString(),
                  "Program presentation must normalize steps by canonical order");
        }
        for(var iteration7=0;iteration7<10;iteration7++)
        {
            round++;
            var program=new InspectionProgram(
                Guid.Parse("82000000-0000-0000-0000-000000000001"),
                "Display Program",
                new Version(2,1,0),
                new[]
                {
                    new ProgramStep(Guid.Parse("82000000-0000-0000-0000-000000000102"),2,ProgramStepKind.Measure,"Measure",Array.Empty<ProgramParameter>()),
                    new ProgramStep(Guid.Parse("82000000-0000-0000-0000-000000000101"),1,ProgramStepKind.Acquire,"Acquire",Array.Empty<ProgramParameter>())
                });
            var items=ClientProgramPresentationRuntime.CreateItems(program);
            Check(items.Select(item=>item.Order).SequenceEqual(new[]{1,2}) &&
                  items[0].Kind==ProgramStepKind.Acquire.ToString(),
                  "Program presentation must normalize steps by canonical order");
        }
        for(var iteration8=0;iteration8<10;iteration8++)
        {
            round++;
            var program=new InspectionProgram(
                Guid.Parse("82000000-0000-0000-0000-000000000001"),
                "Display Program",
                new Version(2,1,0),
                new[]
                {
                    new ProgramStep(Guid.Parse("82000000-0000-0000-0000-000000000102"),2,ProgramStepKind.Measure,"Measure",Array.Empty<ProgramParameter>()),
                    new ProgramStep(Guid.Parse("82000000-0000-0000-0000-000000000101"),1,ProgramStepKind.Acquire,"Acquire",Array.Empty<ProgramParameter>())
                });
            var items=ClientProgramPresentationRuntime.CreateItems(program);
            Check(items.Select(item=>item.Order).SequenceEqual(new[]{1,2}) &&
                  items[0].Kind==ProgramStepKind.Acquire.ToString(),
                  "Program presentation must normalize steps by canonical order");
        }
        for(var iteration9=0;iteration9<10;iteration9++)
        {
            round++;
            var program=new InspectionProgram(
                Guid.Parse("82000000-0000-0000-0000-000000000001"),
                "Display Program",
                new Version(2,1,0),
                new[]
                {
                    new ProgramStep(Guid.Parse("82000000-0000-0000-0000-000000000102"),2,ProgramStepKind.Measure,"Measure",Array.Empty<ProgramParameter>()),
                    new ProgramStep(Guid.Parse("82000000-0000-0000-0000-000000000101"),1,ProgramStepKind.Acquire,"Acquire",Array.Empty<ProgramParameter>())
                });
            var items=ClientProgramPresentationRuntime.CreateItems(program);
            Check(items.Select(item=>item.Order).SequenceEqual(new[]{1,2}) &&
                  items[0].Kind==ProgramStepKind.Acquire.ToString(),
                  "Program presentation must normalize steps by canonical order");
        }
        if(round==100)
            return Task.CompletedTask;

        throw new InvalidOperationException("acceptance matrix must execute exactly 100 rounds");
    }
}
