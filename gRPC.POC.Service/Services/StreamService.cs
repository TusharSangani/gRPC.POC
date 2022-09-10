using Google.Protobuf;
using Grpc.Core;

namespace gRPC.POC.Service.Services
{
    public class StreamService : MyStream.MyStreamBase
    {
        private readonly ILogger<StreamService> _logger;
        public StreamService(ILogger<StreamService> logger)
        {
            _logger = logger;
        }

        public override async Task<MyStreamOperationResponse> MystreamOperation(IAsyncStreamReader<MyStreamOperationRequest> requestStream, ServerCallContext context)
        {
            try
            {
                while (await requestStream.MoveNext()
                  && !context.CancellationToken.IsCancellationRequested)
                {
                    // read incoming message 
                    var current = requestStream.Current;
                    if (current.RequestCase == MyStreamOperationRequest.RequestOneofCase.Startrequest)
                    {
                        // TusharSangani: Haven't used string.format or  $ for combining string as it
                        //can slow down the logging.
                        _logger.LogInformation("Received: StartRequest, BlockName:" + requestStream.Current.Startrequest.Blockname);
                    }
                    else if (current.RequestCase == MyStreamOperationRequest.RequestOneofCase.Chunk)
                    {
                        _logger.LogInformation("Received: Chunk, Payload ..." + await (new ByteArrayContent(requestStream.Current.Chunk.Messagedata.ToByteArray())).ReadAsStringAsync());
                    }                    
                }
            }
            catch (RpcException ex) when (ex.StatusCode == StatusCode.Cancelled)
            {
                _logger.LogInformation("Operation Cancelled");
            }

            _logger.LogInformation("Operation Complete.");          

            return new MyStreamOperationResponse();
        }

       
    }
}
