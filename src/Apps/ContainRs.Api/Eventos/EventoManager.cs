
using System.Text.Json;
using ContainRs.Api.Data;
using Microsoft.EntityFrameworkCore;

namespace ContainRs.Api.Eventos;

public class EventoManager : IEventoManager
{
    private readonly AppDbContext context;

    public EventoManager(AppDbContext context)
    {
        this.context = context;
    }

    public async Task MarcarComoLidaAsync(Guid idEvento, string tipoLeitor)
    {
        var outbox = await context.Outbox
            .FirstOrDefaultAsync(o => o.Id == idEvento);
        if (outbox is null) return;

        var inbox = new InboxMessage
        {
            Id = Guid.NewGuid(),
            OutboxMessageId = outbox.Id,
            Evento = outbox,
            TipoLeitor = tipoLeitor,
            DataProcessamento = DateTime.Now
        };

        await context.Inbox.AddAsync(inbox);
        await context.SaveChangesAsync();
    }

    public async Task<IEnumerable<Mensagem<T>>> RecuperarNaoLidasAsync<T>(string tipoEvento, string tipoLeitor)
    {
        var mensagens = await context.Outbox
            .FromSqlRaw(@"
                SELECT o.* 
                FROM Outbox o 
                    LEFT JOIN Inbox i on i.OutboxMessageId = o.Id 
                        AND i.TipoLeitor = {0}
                WHERE i.Id IS NULL AND o.TipoEvento = {1}", tipoLeitor, tipoEvento
            )
            .ToListAsync();

        var saida = mensagens
            .Select(outbox =>
            {
                var obj = JsonSerializer.Deserialize<T>(outbox.InfoEvento);
                return new Mensagem<T>() { Id = outbox.Id, Corpo = obj! };
            }).ToList();

        return saida;
    }
}
