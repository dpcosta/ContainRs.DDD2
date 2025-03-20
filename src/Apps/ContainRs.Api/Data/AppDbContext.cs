using System.Text.Json;
using ContainRs.Api.Eventos;
using ContainRs.DDD;
using ContainRs.Engenharia.Conteineres;
using Microsoft.EntityFrameworkCore;

namespace ContainRs.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Cliente> Clientes { get; set; }
    public DbSet<PedidoLocacao> Pedidos { get; set; }
    public DbSet<Proposta> Propostas { get; set; }
    public DbSet<Locacao> Locacoes { get; set; }
    public DbSet<Conteiner> Conteineres { get; set; }
    public DbSet<Fatura> Faturas { get; set; }
    public DbSet<OutboxMessage> Outbox { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var domainEvents = ChangeTracker
            .Entries<IAgreggateRoot>()
            .Select(entry => entry.Entity)
            .SelectMany(entity =>
            {
                var events = entity.Events;
                entity.RemoverEventos();
                return events;
            })
            .ToList();

        var outboxMessages = domainEvents
            .Select(@event => new OutboxMessage
            {
                Id = Guid.NewGuid(),
                TipoEvento = @event.GetType().Name,
                InfoEvento = JsonSerializer.Serialize(@event),
                DataCriacao = DateTime.Now,
            })
            .ToList();

        Outbox.AddRange(outboxMessages);

        return base.SaveChangesAsync(cancellationToken);
    }
}
