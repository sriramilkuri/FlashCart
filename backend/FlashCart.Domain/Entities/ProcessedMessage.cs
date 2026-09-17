namespace FlashCart.Domain.Entities;

public class ProcessedMessage
{
    public int Id { get; set; }

    public Guid EventId { get; set; }

    public DateTime ProcessedAt { get; set; }
    public string ConsumerName { get; set; } = string.Empty;
}