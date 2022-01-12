using GrpcAudioStreaming;

namespace gRPC_Client.Services.Interfaces
{
    public interface IAudioSampleSource
    {
        event EventHandler<AudioSample> AudioSampleCreated;
    
        AudioFormat AudioFormat { get; }
    
        Task StartStreaming();
        void StopStreaming();
    }
}