namespace ContainRs.Api.Eventos;

public class OutboxMessage
{
    public Guid Id { get; set; }
    public required string TipoEvento { get; set; }
    public required string InfoEvento { get; set; }
    public DateTime DataCriacao { get; set; }
}
