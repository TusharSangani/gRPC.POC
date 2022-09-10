using System.Threading.Tasks;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using gRPC.POC.Service;
using Grpc.Net.Client;
using static gRPC.POC.Service.MyStream;



// See https://aka.ms/new-console-template for more information
Console.WriteLine("Start message streaming test");
using var channel = GrpcChannel.ForAddress("https://localhost:7052");
var client = new MyStreamClient(channel);

ByteString randomData;

using var streamingCall = client.MystreamOperation();

Console.WriteLine("Sending: StartRequest, BlockName: Foo ");
var startRequest = new MyStreamOperationRequest() { Startrequest = new StartRequest() { Blockname = "Foo" } };
await streamingCall.RequestStream.WriteAsync(startRequest);
for (int i = 0; i < 4; i++)
{
    var guidData = Guid.NewGuid().ToString();
    randomData = ByteString.CopyFrom(guidData, System.Text.Encoding.Unicode);
    Console.WriteLine("Sending: Chunk, Payload ..." + guidData);

    var chunkRequest = new MyStreamOperationRequest() { Chunk = new Chunk() { Messagedata = randomData } };
    await streamingCall.RequestStream.WriteAsync(chunkRequest);
}
await streamingCall.RequestStream.CompleteAsync();
var result = await streamingCall.ResponseAsync;
Console.WriteLine("MyStreamOperationResponse");
Console.WriteLine("Press any key to exit...");
Console.ReadKey();