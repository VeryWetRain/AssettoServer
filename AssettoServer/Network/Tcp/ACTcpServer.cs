using System;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using AssettoServer.Server.Configuration;
using AssettoServer.Shared.Services;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace AssettoServer.Network.Tcp;

public class ACTcpServer : CriticalBackgroundService
{
    private readonly ACServerConfiguration _configuration;
    private readonly Func<TcpClient, ACTcpClient> _acTcpClientFactory;

    public ACTcpServer(Func<TcpClient, ACTcpClient> acTcpClientFactory, ACServerConfiguration configuration, IHostApplicationLifetime applicationLifetime) : base(applicationLifetime)
    {
        _acTcpClientFactory = acTcpClientFactory;
        _configuration = configuration;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        Log.Information("Starting TCP server on port {TcpPort}", _configuration.Server.TcpPort);
        using var listener = new TcpListener(IPAddress.Any, _configuration.Server.TcpPort);
        listener.Start();

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                TcpClient tcpClient = await listener.AcceptTcpClientAsync(stoppingToken);

                // Set keepalive because some routers will drop idle connections
                //tcpClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);
                //SetSocketKeepAliveValues(tcpClient, 60000, 1000);

                ACTcpClient acClient = _acTcpClientFactory(tcpClient);
                await acClient.StartAsync();
            }
            catch (OperationCanceledException) { }
            catch (Exception ex)
            {
                Log.Error(ex, "Something went wrong while trying to accept TCP connection");
            }
        }
    }

    private static void SetSocketKeepAliveValues(TcpClient tcpc, int KeepAliveTime, int KeepAliveInterval)
    {
        //KeepAliveTime: default value is 2hr
        //KeepAliveInterval: default value is 1s and Detect 5 times

        uint dummy = 0; //lenth = 4
        byte[] inOptionValues = new byte[System.Runtime.InteropServices.Marshal.SizeOf(dummy) * 3]; //size = lenth * 3 = 12
        bool OnOff = true;

        BitConverter.GetBytes((uint)(OnOff ? 1 : 0)).CopyTo(inOptionValues, 0);
        BitConverter.GetBytes((uint)KeepAliveTime).CopyTo(inOptionValues, Marshal.SizeOf(dummy));
        BitConverter.GetBytes((uint)KeepAliveInterval).CopyTo(inOptionValues, Marshal.SizeOf(dummy) * 2);
        // of course there are other ways to marshal up this byte array, this is just one way
        // call WSAIoctl via IOControl

        // .net 3.5 type
        tcpc.Client.IOControl(IOControlCode.KeepAliveValues, inOptionValues, null);
    }
}
