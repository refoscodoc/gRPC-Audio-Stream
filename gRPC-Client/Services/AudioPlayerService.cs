using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Grpc.Net.Client;
using GrpcAudioStreaming;

namespace gRPC_Audio.Services;

public static class AudioPlayerService
{
    public static async Task AudioApp()
    {
        var channel = GrpcChannel.ForAddress("http://localhost:5064");
        var client = new AudioStream.AudioStreamClient(channel);
        var format = client.GetFormat(new Empty());
        var audioStream = client.GetStream(new Empty());

        Console.WriteLine(format);
        
        using var audioPlayer = new AudioPlayer(format.ToWaveFormat());
        audioPlayer.Play();

        await foreach (var sample in audioStream.ResponseStream.ReadAllAsync())
        {
            audioPlayer.AddSample(sample.Data.ToByteArray());
        }
    }
}