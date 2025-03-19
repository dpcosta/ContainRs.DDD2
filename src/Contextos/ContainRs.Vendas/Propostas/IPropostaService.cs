using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ContainRs.Vendas.Locacoes;
using System.Transactions;
using ContainRs.Contracts;

namespace ContainRs.Vendas.Propostas;

public interface IPropostaService
{
    Task<Proposta?> AprovarAsync(AprovarProposta comando);
    Task<Proposta?> ComentarAsync(ComentarProposta comando);
}

public class PropostaService : IPropostaService
{
    private readonly IRepository<Proposta> repoProposta;
    private readonly IRepository<Locacao> repoLocacao;
    private readonly ICalculadoraPrazosLocacao calculadora;

    public PropostaService(IRepository<Proposta> repoProposta, IRepository<Locacao> repoLocacao, ICalculadoraPrazosLocacao calculadora)
    {
        this.repoProposta = repoProposta;
        this.repoLocacao = repoLocacao;
        this.calculadora = calculadora;
    }

    public async Task<Proposta?> AprovarAsync(AprovarProposta comando)
    {
        var proposta = await repoProposta
                .GetFirstAsync(
                    p => p.Id == comando.IdProposta && p.SolicitacaoId == comando.IdPedido,
                    p => p.Id);
        if (proposta is null) return null;

        if (proposta.Aprovar())
        {
            // criar locação a partir da proposta aceita
            var locacao = new Locacao()
            {
                PropostaId = proposta.Id,
                DataInicio = DateTime.Now,
                DataPrevistaEntrega = calculadora
                    .CalculaDataPrevistaParaEntrega(proposta),
                DataTermino = calculadora
                    .CalculaDataPrevistaParaTermino(proposta)
            };

            using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            await repoProposta.UpdateAsync(proposta);
            await repoLocacao.AddAsync(locacao);

            scope.Complete();
        }

        return proposta;
    }

    public async Task<Proposta?> ComentarAsync(ComentarProposta comando)
    {
        var proposta = await repoProposta
                .GetFirstAsync(
                    p => p.Id == comando.IdProposta && p.SolicitacaoId == comando.IdPedido,
                    p => p.Id);
        if (proposta is null) return null;

        
        proposta.AddComentario(new Comentario()
        {
            Id = Guid.NewGuid(),
            Data = DateTime.Now,
            Usuario = comando.Pessoa,
            Texto = comando.Mensagem
        });

        await repoProposta.UpdateAsync(proposta);
        return proposta;
    }
}
