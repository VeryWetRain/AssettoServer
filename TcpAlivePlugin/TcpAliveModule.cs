using AssettoServer.Server.Plugin;
using Autofac;

namespace TcpAlivePlugin;

public class TcpAliveModule : AssettoServerModule<TcpAliveConfiguration>
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterType<TcpAlive>().AsSelf().As<IAssettoServerAutostart>().SingleInstance();
    }
}
