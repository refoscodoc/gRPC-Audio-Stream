using System.Net.Http.Headers;
using CSCore.Codecs.WAV;
using Google.Protobuf;
using gRPC_Client.Services.Interfaces;
using GrpcAudioStreaming;

namespace gRPC_Client.Services;

public class AudioSampleSource : IAudioSampleSource, IDisposable
{
    public event EventHandler<AudioSample>? AudioSampleCreated;
    private readonly CancellationTokenSource _cancellationTokenSource;

    private readonly WaveFileReader _waveFileReader;
    public AudioFormat AudioFormat { get; }

    public AudioSampleSource(string file)
    {
        _waveFileReader = new WaveFileReader(file);
        AudioFormat = Extensions.ToAudioFormat(_waveFileReader);
        _cancellationTokenSource = new CancellationTokenSource();
    }
    public Task StartStreaming()
    {
        return Task.Factory.StartNew(() => Stream(_waveFileReader, _cancellationTokenSource.Token),
            TaskCreationOptions.LongRunning);
    }

    public void StopStreaming()
    {
        _cancellationTokenSource.Cancel();
    }
    
    private void Stream(WaveFileReader stream, CancellationToken cancellationToken)
    {
        var buffer = new byte[stream.WaveFormat.BytesPerSecond];
        var streamTimeStart = stream.Position;
        var realTimeStart = DateTime.UtcNow.ToFileTime();

        while (!cancellationToken.IsCancellationRequested)
        {
            var bytesRead = stream.Read(buffer, 0, buffer.Length);

            if (bytesRead == 0)
            {
                // if we have reached the end, reset stream to start
                stream.Position = 0;
                streamTimeStart = stream.Position;
                realTimeStart = DateTime.UtcNow.ToFileTime();
                continue;
            }

            var time = realTimeStart + stream.Position;
            var audioSample = new AudioSample
            {
                Timestamp = time.ToString(),
                Data = ByteString.CopyFrom(buffer)
            };
            OnAudioSampleCreated(audioSample);

            var streamTimePassed = stream.Position - streamTimeStart;
            var realTimePassed = DateTime.UtcNow.ToFileTime() - realTimeStart;
            var timeDifference = Math.Max(0, (streamTimePassed - realTimePassed));
            Thread.Sleep((int)timeDifference);
        }
    }

    protected virtual void OnAudioSampleCreated(AudioSample audioSample)
    {
        AudioSampleCreated?.Invoke(this, audioSample);
    }


    public void Dispose()
    {
        _waveFileReader.Dispose();
    }
}