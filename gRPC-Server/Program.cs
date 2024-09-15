using System.Net;
using gRPC_Client.Services;
using gRPC_Client.Services.Interfaces;
using Microsoft.AspNetCore.Server.Kestrel.Core;

var builder = WebApplication.CreateBuilder(args);

// builder.WebHost.ConfigureKestrel(options =>
// {
//     options.Listen(IPAddress.Any, 7273, listenOptions =>
//     {
//         listenOptions.Protocols = HttpProtocols.Http2;
//         // listenOptions.UseHttps("<path to .pfx file>",
//         //     "<certificate password>");
//     });
// });

builder.Services.AddCors();

builder.Services.AddGrpc();
builder.Services.AddSingleton<IAudioSampleSource>(new AudioSampleSource(@"./Wav/audio.wav"));

var app = builder.Build();

app.UseCors();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

// app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// app.UseAuthorization();

app.MapGrpcService<AudioStreamService>();
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

// app.UseEndpoints(endpoints =>
// {
//     endpoints.MapGrpcService<AudioStreamService>();
//
//     endpoints.MapGet("/", async context =>
//     {
//         await context.Response.WriteAsync("Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");
//     });
// });

app.Run();