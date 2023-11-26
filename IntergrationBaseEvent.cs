namespace EventBus.Messages;
public record IntegrationBaseEvent()
{
    public DateTime CreationDate { get; } = DateTime.Now
}