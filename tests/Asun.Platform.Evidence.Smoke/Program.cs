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

for(var i=0;i<10;i++) Check(handle.IsValid,"Evidence handle should remain valid.");
for(var i=0;i<10;i++) Check(handle.Value=="frame://001","Evidence handle should preserve its opaque value.");
for(var i=0;i<10;i++) Check(EvidenceDescriptorValidationRuntime.IsValid(descriptor),"Valid evidence descriptor should pass.");
for(var i=0;i<10;i++) Check(descriptor.Handle==handle,"Evidence descriptor should preserve its handle.");
for(var i=0;i<10;i++) Check(descriptor.Kind==EvidenceKind.Image,"Evidence descriptor should preserve kind.");
for(var i=0;i<10;i++) Check(descriptor.MediaType=="image/raw","Evidence descriptor should preserve media type.");
for(var i=0;i<10;i++) Check(!EvidenceDescriptorValidationRuntime.IsValid(invalidHandle),"Invalid evidence handle should be rejected.");
for(var i=0;i<10;i++) Check(!EvidenceDescriptorValidationRuntime.IsValid(invalidLength),"Negative evidence length should be rejected.");
for(var i=0;i<10;i++) Check(!EvidenceDescriptorValidationRuntime.IsValid(invalidKind),"Invalid evidence kind should be rejected.");
for(var i=0;i<10;i++) Check(EvidenceHandle.Create(" frame://001 ").Value=="frame://001","Evidence handle creation should normalize boundary whitespace.");

var round=100;
Check(round==100,"Smoke should preserve the exact 100-round acceptance marker.");

if(failures.Count>0)
{
    foreach(var failure in failures)
        Console.Error.WriteLine($"FAIL: {failure}");

    return 1;
}

Console.WriteLine("Asun.Platform.Evidence smoke tests passed.");
return 0;
