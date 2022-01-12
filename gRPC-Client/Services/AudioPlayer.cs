using System.Diagnostics;
using CSCore;
using CSCore.DMO;
using CSCore.SoundOut;
using CSCore.Streams;

namespace gRPC_Audio.Services;

public class AudioPlayer : IDisposable
{
    private readonly ISoundOut _wavePlayer;
    private readonly WriteableBufferingSource _bufferedWaveProvider;

    public AudioPlayer(WaveFormat waveFormat)
    {
        _wavePlayer = new WaveOut();
        _bufferedWaveProvider = new WriteableBufferingSource(waveFormat, 5000);
        _wavePlayer.Initialize(_bufferedWaveProvider);
    }

    public void AddSample(byte[] sample)
    {
        try
        {
            // _bufferedWaveProvider.AddSamples(sample, 0, sample.Length);
            _bufferedWaveProvider.Write(sample, 0, sample.Length);
        }
        catch (Exception ex)
        {
            Trace.TraceError($"Adding samples failed: {ex}");
        }
    }

    public void Play()
    {
        _wavePlayer.Play();
    }

    public void Stop()
    {
        _wavePlayer.Stop();
    }

    public void Dispose()
    {
        _wavePlayer.Stop();
        _wavePlayer.Dispose();
    }
}