
 
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Google.Protobuf.WellKnownTypes;
using gRPC_Audio.Services;
using Grpc.Core;
using Grpc.Net.Client;

namespace GrpcAudioStreaming.Client
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var channel = GrpcChannel.ForAddress("http://localhost:5064");
            var client = new AudioStream.AudioStreamClient(channel);
            var format = client.GetFormat(new Empty());
            var audioStream = client.GetStream(new Empty());

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                using var audioPlayerFfmpg = new AudioPlayerFfmpeg(format.ToWaveFormat());
                audioPlayerFfmpg.Play();
                
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
//
// var builder = WebApplication.CreateBuilder(args);
//
// // Add services to the container.
// builder.Services.AddRazorPages();
// builder.Services.AddScoped<AudioPlayerService>();
//
//
// var app = builder.Build();
//
// // Configure the HTTP request pipeline.
// if (!app.Environment.IsDevelopment())
// {
//     app.UseExceptionHandler("/Error");
//     // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
//     app.UseHsts();
// }
//
// app.UseHttpsRedirection();
// app.UseStaticFiles();
//
// app.UseRouting();
//
// app.UseAuthorization();
//
// app.MapRazorPages();
//
// app.Run();