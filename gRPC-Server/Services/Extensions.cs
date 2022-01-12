using CSCore;
using CSCore.Codecs.WAV;
using GrpcAudioStreaming;

namespace gRPC_Client.Services;

public class Extensions
{
    public static AudioFormat ToAudioFormat(WaveFileReader waveFormat)
    {
        return new AudioFormat
        {
            AverageBytesPerSecond = waveFormat.WaveFormat.BytesPerSecond,
            BitsPerSample = waveFormat.WaveFormat.BitsPerSample,
            BlockAlign = waveFormat.WaveFormat.BlockAlign,
            Channels = waveFormat.WaveFormat.Channels,
            ExtraSize = waveFormat.WaveFormat.ExtraSize,
            SampleRate = waveFormat.WaveFormat.SampleRate,
            
            // In CSCore the wave format is a separate class - AudioEncoding.
            // So let's assume it's always WAV for now
            // Encoding = waveFormat.Encoding.ToString()
        };
    }
}