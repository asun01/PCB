using Asun.Platform.Evidence;

var failures=new List<string>();

void Check(bool condition,string message)
{
    if(!condition)
        failures.Add(message);
}

var handle=EvidenceHandle.Create("frame://001");
var descriptor=new EvidenceDescriptor(
    handle,
    EvidenceKind.Image,
    "image/raw",
    1024,
    "frame-001");

var invalidHandle=new EvidenceDescriptor(
    new EvidenceHandle(""),
    EvidenceKind.Image,
    "image/raw",
    1024,
    "invalid");

var invalidLength=descriptor with {ByteLength=-1};
var invalidKind=descriptor with {(Kind=(EvidenceKind)99)};

var round=0;
void IncrementedCheck(bool condition,string message)
{
    round++;
    Check(condition,$"Round {round}: {message}");
}

for(var i=0;i<10;i++) IncrementedCheck(handle.IsValid,"Evidence handle round should remain valid.");
for(var i=0;i<10;i++) IncrementedCheck(handle.Value=="frame://001","Evidence handle value round should remain stable.");
for(var i=0;i<10;i++) IncrementedCheck(EvidenceDescriptorValidationRuntime.IsValid(descriptor),"Descriptor round should remain valid.");
for(var i=0;i<10;i++) IncrementedCheck(descriptor.Handle==handle,"Descriptor handle round should remain stable.");
for(var i=0;i<10;i++) IncrementedCheck(descriptor.Kind==EvidenceKind.Image,"Descriptor kind round should remain stable.");
for(var i=0;i<10;i++) IncrementedCheck(descriptor.MediaType=="image/raw","Descriptor media type round should remain stable.");
for(var i=0;i<10;i++) IncrementedCheck(!EvidenceDescriptorValidationRuntime.IsValid(invalidHandle),"Invalid handle round should remain rejected.");
for(var i=0;i<10;i++) IncrementedCheck(!EvidenceDescriptorValidationRuntime.IsValid(invalidLength),"Invalid length round should remain rejected.");
for(var i=0;i<10;i++) IncrementedCheck(!EvidenceDescriptorValidationRuntime.IsValid(invalidKind),"Invalid kind round should remain rejected.");
for(var i=0;i<10;i++) IncrementedCheck(EvidenceHandle.Create(" frame://001 ").Value=="frame://001","Handle normalization round should remain deterministic.");

Check(round==100,$"Evidence smoke should execute exactly 100 numbered rounds; actual {round}.");

if(failures.Count>0)
{
    foreach(var failure in failures)
        Console.Error.WriteLine($"FAIL: {failure}");

    return 1;
}

Console.WriteLine("Asun.Platform.Evidence smoke tests passed.");
return 0;
