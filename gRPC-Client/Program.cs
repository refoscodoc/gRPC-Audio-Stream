using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Google.Protobuf.WellKnownTypes;
using gRPC_Audio.Services;
using Grpc.Core;
using Grpc.Net.Client;
using Grpc.Net.Client.Web;

namespace GrpcAudioStreaming.Client
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var httpHandler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
            };
            
            var channel = GrpcChannel.ForAddress(
                "https://localhost:7167/",
                new GrpcChannelOptions
                {
                    HttpHandler = new GrpcWebHandler(httpHandler)
                });
            var client = new AudioStream.AudioStreamClient(channel);
            var format = client.GetFormat(new Empty());
            var audioStream = client.GetStream(new Empty());

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                Console.WriteLine("Linux Platform Detected: " + format);
                using var audioPlayerFfmpg = new AudioPlayerFfmpeg(format.ToWaveFormat());
                audioPlayerFfmpg.Play();

                Console.WriteLine("Streaming Starting:");

                await foreach (var sample in audioStream.ResponseStream.ReadAllAsync())
                {
                    audioPlayerFfmpg.AddSample(sample.Data.ToByteArray());
                }
            }
            else
            {
                using var audioPlayer = new AudioPlayer(format.ToWaveFormat());
                audioPlayer.Play(); 
                
                await foreach (var sample in audioStream.ResponseStream.ReadAllAsync())
                {
                    audioPlayer.AddSample(sample.Data.ToByteArray());
                }
            }
        }
    }
}
