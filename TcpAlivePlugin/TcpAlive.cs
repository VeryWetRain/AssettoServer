using AssettoServer.Server;
using AssettoServer.Server.Plugin;
using AssettoServer.Shared.Network.Packets.Shared;
using AssettoServer.Shared.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using System.ServiceModel.Channels;
using System.Threading;

namespace TcpAlivePlugin;

public class TcpAlive : CriticalBackgroundService, IAssettoServerAutostart
{
    private readonly EntryCarManager _entryCarManager;

    public TcpAlive(EntryCarManager entryCarManager, IHostApplicationLifetime applicationLifetime) : base(applicationLifetime)
    {
        _entryCarManager = entryCarManager;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            // every minute
            await Task.Delay(30000,
                stoppingToken);
            try
            {
                _entryCarManager.BroadcastPacket(new ChatMessage { SessionId = 255, Message = "" });
                _entryCarManager.BroadcastPacket(new PulsePacket { });
                Log.Information("sent pulse to all");
            }
            catch (TaskCanceledException)
            {
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error sending chat ping.");
            }
        }
    }
}
