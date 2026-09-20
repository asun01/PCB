using Asun.Domain.Pcb;
using Asun.Platform.MetrologyProductionIntegration;

namespace Asun.Platform.PcbExecutionIntegration.Smoke;

public static class PcbExecutionMeasurementComponentBinding3HundredStageSmoke
{
    public static Task RunAsync(Action<bool,string> Check)
    {
        var round=0;
        for(var group1=0;group1<10;group1++)
        {
            for(var i1=0;i1<10;i1++)
            {
                round++;
                var fixture=PcbMeasurementComponentFixtureRuntime.Create();
                var binding=fixture.MeasurementBindings.ToArray();
                binding[i1%2]=binding[i1%2] with
                {
                    Designator=i1%2==0 ? "UNKNOWN" : "C1"
                };
                Check(
                    !PcbExecutionMeasurementComponentBindingRuntime.IsValid(
                        fixture.ExecutionSnapshot,fixture.Assembly,binding) &&
                    (group1<9 || round==100),
                    "measurement designator drift must be rejected");
            }
        }
        for(var group2=0;group2<10;group2++)
        {
            for(var i2=0;i2<10;i2++)
            {
                round++;
                var fixture=PcbMeasurementComponentFixtureRuntime.Create();
                var binding=fixture.MeasurementBindings.ToArray();
                binding[i2%2]=binding[i2%2] with
                {
                    Designator=i2%2==0 ? "UNKNOWN" : "C1"
                };
                Check(
                    !PcbExecutionMeasurementComponentBindingRuntime.IsValid(
                        fixture.ExecutionSnapshot,fixture.Assembly,binding) &&
                    (group2<9 || round==100),
                    "measurement designator drift must be rejected");
            }
        }
        for(var group3=0;group3<10;group3++)
        {
            for(var i3=0;i3<10;i3++)
            {
                round++;
                var fixture=PcbMeasurementComponentFixtureRuntime.Create();
                var binding=fixture.MeasurementBindings.ToArray();
                binding[i3%2]=binding[i3%2] with
                {
                    Designator=i3%2==0 ? "UNKNOWN" : "C1"
                };
                Check(
                    !PcbExecutionMeasurementComponentBindingRuntime.IsValid(
                        fixture.ExecutionSnapshot,fixture.Assembly,binding) &&
                    (group3<9 || round==100),
                    "measurement designator drift must be rejected");
            }
        }
        for(var group4=0;group4<10;group4++)
        {
            for(var i4=0;i4<10;i4++)
            {
                round++;
                var fixture=PcbMeasurementComponentFixtureRuntime.Create();
                var binding=fixture.MeasurementBindings.ToArray();
                binding[i4%2]=binding[i4%2] with
                {
                    Designator=i4%2==0 ? "UNKNOWN" : "C1"
                };
                Check(
                    !PcbExecutionMeasurementComponentBindingRuntime.IsValid(
                        fixture.ExecutionSnapshot,fixture.Assembly,binding) &&
                    (group4<9 || round==100),
                    "measurement designator drift must be rejected");
            }
        }
        for(var group5=0;group5<10;group5++)
        {
            for(var i5=0;i5<10;i5++)
            {
                round++;
                var fixture=PcbMeasurementComponentFixtureRuntime.Create();
                var binding=fixture.MeasurementBindings.ToArray();
                binding[i5%2]=binding[i5%2] with
                {
                    Designator=i5%2==0 ? "UNKNOWN" : "C1"
                };
                Check(
                    !PcbExecutionMeasurementComponentBindingRuntime.IsValid(
                        fixture.ExecutionSnapshot,fixture.Assembly,binding) &&
                    (group5<9 || round==100),
                    "measurement designator drift must be rejected");
            }
        }
        for(var group6=0;group6<10;group6++)
        {
            for(var i6=0;i6<10;i6++)
            {
                round++;
                var fixture=PcbMeasurementComponentFixtureRuntime.Create();
                var binding=fixture.MeasurementBindings.ToArray();
                binding[i6%2]=binding[i6%2] with
                {
                    Designator=i6%2==0 ? "UNKNOWN" : "C1"
                };
                Check(
                    !PcbExecutionMeasurementComponentBindingRuntime.IsValid(
                        fixture.ExecutionSnapshot,fixture.Assembly,binding) &&
                    (group6<9 || round==100),
                    "measurement designator drift must be rejected");
            }
        }
        for(var group7=0;group7<10;group7++)
        {
            for(var i7=0;i7<10;i7++)
            {
                round++;
                var fixture=PcbMeasurementComponentFixtureRuntime.Create();
                var binding=fixture.MeasurementBindings.ToArray();
                binding[i7%2]=binding[i7%2] with
                {
                    Designator=i7%2==0 ? "UNKNOWN" : "C1"
                };
                Check(
                    !PcbExecutionMeasurementComponentBindingRuntime.IsValid(
                        fixture.ExecutionSnapshot,fixture.Assembly,binding) &&
                    (group7<9 || round==100),
                    "measurement designator drift must be rejected");
            }
        }
        for(var group8=0;group8<10;group8++)
        {
            for(var i8=0;i8<10;i8++)
            {
                round++;
                var fixture=PcbMeasurementComponentFixtureRuntime.Create();
                var binding=fixture.MeasurementBindings.ToArray();
                binding[i8%2]=binding[i8%2] with
                {
                    Designator=i8%2==0 ? "UNKNOWN" : "C1"
                };
                Check(
                    !PcbExecutionMeasurementComponentBindingRuntime.IsValid(
                        fixture.ExecutionSnapshot,fixture.Assembly,binding) &&
                    (group8<9 || round==100),
                    "measurement designator drift must be rejected");
            }
        }
        for(var group9=0;group9<10;group9++)
        {
            for(var i9=0;i9<10;i9++)
            {
                round++;
                var fixture=PcbMeasurementComponentFixtureRuntime.Create();
                var binding=fixture.MeasurementBindings.ToArray();
                binding[i9%2]=binding[i9%2] with
                {
                    Designator=i9%2==0 ? "UNKNOWN" : "C1"
                };
                Check(
                    !PcbExecutionMeasurementComponentBindingRuntime.IsValid(
                        fixture.ExecutionSnapshot,fixture.Assembly,binding) &&
                    (group9<9 || round==100),
                    "measurement designator drift must be rejected");
            }
        }
        for(var group10=0;group10<10;group10++)
        {
            for(var i10=0;i10<10;i10++)
            {
                round++;
                var fixture=PcbMeasurementComponentFixtureRuntime.Create();
                var binding=fixture.MeasurementBindings.ToArray();
                binding[i10%2]=binding[i10%2] with
                {
                    Designator=i10%2==0 ? "UNKNOWN" : "C1"
                };
                Check(
                    !PcbExecutionMeasurementComponentBindingRuntime.IsValid(
                        fixture.ExecutionSnapshot,fixture.Assembly,binding) &&
                    (group10<9 || round==100),
                    "measurement designator drift must be rejected");
            }
        }
        return Task.CompletedTask;
    }
}
