using CSCore;
using CSCore.Codecs.WAV;
using GrpcAudioStreaming;

namespace gRPC_Audio.Services;

public static class Extensions
{
    public static WaveFormat ToWaveFormat(this AudioFormat audioFormat)
    {
        return new WaveFormat(
            // (WaveFormatEncoding)Enum.Parse(typeof(WaveFormatEncoding), audioFormat.Encoding),
            audioFormat.SampleRate,
            audioFormat.BitsPerSample,
            audioFormat.Channels
            // audioFormat.AverageBytesPerSecond,
            // audioFormat.Encoding,
            // audioFormat.Channels,
            // audioFormat.BlockAlign
            );
    }
}