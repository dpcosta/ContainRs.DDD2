using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ContainRs.Vendas.Locacoes;
using System.Transactions;

namespace ContainRs.Vendas.Propostas;

public class AprovarProposta
{
    public AprovarProposta(Guid idPedido, Guid idProposta)
    {
        IdPedido = idPedido;
        IdProposta = idProposta;
    }

    public Guid IdPedido { get; }
    public Guid IdProposta { get; }

    public void Executar()
    {
        var proposta = await repoProposta
                .GetFirstAsync(
                    p => p.Id == propostaId && p.SolicitacaoId == id,
                    p => p.Id);
        if (proposta is null) return Results.NotFound();

        proposta.Situacao = SituacaoProposta.Aceita;

        // criar locação a partir da proposta aceita
        var locacao = new Locacao()
        {
            PropostaId = proposta.Id,
            DataInicio = DateTime.Now,
            DataPrevistaEntrega = proposta.Solicitacao.DataInicioOperacao.AddDays(-proposta.Solicitacao.DisponibilidadePrevia),
            DataTermino = proposta.Solicitacao.DataInicioOperacao.AddDays(proposta.Solicitacao.DuracaoPrevistaLocacao)
        };

        using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

        await repoProposta.UpdateAsync(proposta);
        await repoLocacao.AddAsync(locacao);

        scope.Complete();
    }
}
