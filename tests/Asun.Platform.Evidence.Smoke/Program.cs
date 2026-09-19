using Asun.Platform.Evidence;

var failures=new List<string>();
var round=0;

void Check(bool condition,string message)
{
    round++;
    if(!condition)
        failures.Add($"Round {round}: {message}");
}

var handle=EvidenceHandle.Create("frame://001");
var descriptor=new EvidenceDescriptor(handle,EvidenceKind.Image,"image/raw",1024,"frame-001");
var invalidHandle=new EvidenceDescriptor(new EvidenceHandle(""),EvidenceKind.Image,"image/raw",1024,"invalid");
var invalidLength=descriptor with {ByteLength=-1};
var invalidKind=descriptor with {Kind=(EvidenceKind)99};
var fingerprint=EvidenceDescriptorFingerprintRuntime.CreateFingerprint(descriptor);

for(var i=0;i<10;i++) Check(handle.IsValid,"Evidence handle should remain valid.");
for(var i=0;i<10;i++) Check(handle.Value=="frame://001","Evidence handle value should remain stable.");
for(var i=0;i<10;i++) Check(EvidenceDescriptorValidationRuntime.IsValid(descriptor),"Descriptor should remain valid.");
for(var i=0;i<10;i++) Check(descriptor.Handle==handle,"Descriptor handle should remain stable.");
for(var i=0;i<10;i++) Check(descriptor.Kind==EvidenceKind.Image,"Descriptor kind should remain stable.");
for(var i=0;i<10;i++) Check(descriptor.MediaType=="image/raw","Descriptor media type should remain stable.");
for(var i=0;i<10;i++) Check(!EvidenceDescriptorValidationRuntime.IsValid(invalidHandle),"Invalid handle should remain rejected.");
for(var i=0;i<10;i++) Check(!EvidenceDescriptorValidationRuntime.IsValid(invalidLength),"Negative length should remain rejected.");
for(var i=0;i<10;i++) Check(!EvidenceDescriptorValidationRuntime.IsValid(invalidKind),"Invalid kind should remain rejected.");
for(var i=0;i<10;i++) Check(EvidenceHandle.Create(" frame://001 ").Value=="frame://001","Handle normalization should remain deterministic.");

if(round!=100)
    failures.Add($"Evidence descriptor smoke should execute exactly 100 numbered rounds; actual {round}.");

var missing=await EvidenceCatalogRuntime.GetValidatedAsync(
    new StubEvidenceCatalog(descriptor),
    EvidenceHandle.Create("frame://missing"));
if(missing is not null) failures.Add("Missing evidence lookup should remain empty.");

var found=await EvidenceCatalogRuntime.GetValidatedAsync(
    new StubEvidenceCatalog(descriptor),
    handle);
if(found!=descriptor) failures.Add("Validated evidence lookup should return the descriptor.");

using var cancellation=new CancellationTokenSource();
cancellation.Cancel();
var cancelledObserved=false;
try
{
    await EvidenceCatalogRuntime.GetValidatedAsync(
        new BlockingEvidenceCatalog(),
        handle,
        cancellation.Token);
}
catch(OperationCanceledException)
{
    cancelledObserved=true;
}

if(!cancelledObserved) failures.Add("Evidence catalog lookup should honor cancellation.");


EvidenceCatalogSnapshotHundredStageSmoke.Run((condition,message)=>
    Check(condition,message));

if(failures.Count>0)
{
    foreach(var failure in failures)
        Console.Error.WriteLine($"FAIL: {failure}");

    return 1;
}

Console.WriteLine("Asun.Platform.Evidence smoke tests passed.");

file sealed class StubEvidenceCatalog : IEvidenceCatalog
{
    private readonly EvidenceDescriptor _descriptor;

    public StubEvidenceCatalog(EvidenceDescriptor descriptor)=>
        _descriptor=descriptor;

    public ValueTask<EvidenceDescriptor?> GetAsync(
        EvidenceHandle handle,
        CancellationToken cancellationToken=default)=>
        ValueTask.FromResult<EvidenceDescriptor?>(
            handle==_descriptor.Handle ? _descriptor : null);
}

file sealed class BlockingEvidenceCatalog : IEvidenceCatalog
{
    public async ValueTask<EvidenceDescriptor?> GetAsync(
        EvidenceHandle handle,
        CancellationToken cancellationToken=default)
    {
        await Task.Delay(Timeout.InfiniteTimeSpan,cancellationToken);
        return null;
    }
}
