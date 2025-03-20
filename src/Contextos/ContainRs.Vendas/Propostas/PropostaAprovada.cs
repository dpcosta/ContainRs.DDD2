using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ContainRs.DDD;

namespace ContainRs.Vendas.Propostas;

public class PropostaAprovada : IDomainEvent
{
    public PropostaAprovada(Guid idProposta, decimal valorProposta)
    {
        IdProposta = idProposta;
        ValorProposta = valorProposta;
    }

    public Guid IdProposta { get; }
    public decimal ValorProposta { get; }
}
