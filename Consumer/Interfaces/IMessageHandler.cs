using Shared.Models;

namespace Consumer.Interfaces;
public interface IMessageHandler
{
    Task HandleMessage(Message message, CancellationToken cancellationToken);
}
