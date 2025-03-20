namespace ContainRs.Api.Eventos;

public class InboxMessage
{
    public Guid Id { get; set; }
    public required string TipoLeitor { get; set; }
    public Guid OutboxMessageId { get; set; }
    public required OutboxMessage Evento { get; set; }
    public DateTime DataProcessamento { get; set; }
    public string? Erro { get; set; }
}
