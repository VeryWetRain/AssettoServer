using FluentValidation;
using JetBrains.Annotations;

namespace TcpAlivePlugin;

// Use FluentValidation to validate plugin configuration
[UsedImplicitly]
public class TcpAliveConfigurationValidator : AbstractValidator<TcpAliveConfiguration>
{
    public TcpAliveConfigurationValidator()
    {

    }
}
