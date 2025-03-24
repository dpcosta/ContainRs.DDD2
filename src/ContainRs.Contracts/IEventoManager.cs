using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContainRs.Contracts;

public class Mensagem<T>
{
    public Guid Id { get; set; }
    public required T Corpo { get; set; }
}

public interface IEventoManager
{
    Task<IEnumerable<Mensagem<T>>> RecuperarNaoLidasAsync<T>(string tipoEvento, string tipoLeitor);
    Task MarcarComoLidaAsync(Guid idEvento, string tipoLeitor);
}