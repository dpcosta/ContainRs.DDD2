
using System.Text.Json;
using ContainRs.Api.Data;
using Microsoft.EntityFrameworkCore;

namespace ContainRs.Api.Eventos;

public class EventManager : IEventManager
{
    private readonly AppDbContext context;

    public EventManager(AppDbContext context)
    {
        this.context = context;
    }

    public async Task<IEnumerable<T>> GetNotProcessedAsync<T>(string eventType, string readerType)
    {
        return (await context.Outbox
            .FromSqlRaw(@"
                SELECT o.* 
                FROM Outbox o 
                    LEFT JOIN Inbox i on i.OutboxMessageId = o.Id 
                        AND i.TipoLeitor = {0}
                WHERE i.Id IS NULL AND o.TipoEvento = {1}", readerType, eventType
            )
            .ToListAsync())
            .Select(outbox => JsonSerializer.Deserialize<T>(outbox.InfoEvento)!)
            .ToList();
    }

    public async Task MarkAsProcessedAsync(string eventType, string readerType)
    {
        var outbox = await context.Outbox
            .FirstAsync(o => o.TipoEvento.Equals(eventType));

        var inboxMessage = new InboxMessage
        {
            Id = Guid.NewGuid(),
            OutboxMessageId = outbox.Id,
            Evento = outbox,
            TipoLeitor = readerType,
            DataProcessamento = DateTime.Now,
        };
        context.Inbox.Add(inboxMessage);
        await context.SaveChangesAsync();
    }
}
