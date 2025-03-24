using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ContainRs.Contracts;

namespace ContainRs.Financeiro.Faturamento;

public class PropostaAprovada
{
    public Guid IdProposta { get; set; }
    public decimal ValorProposta { get; set; }
}

public class EmissorDeFaturas
{
    private readonly IRepository<Fatura> repoFatura;
    private readonly IEventoManager eventoManager;

    public EmissorDeFaturas(IRepository<Fatura> repoFatura, IEventoManager eventoManager)
    {
        this.repoFatura = repoFatura;
        this.eventoManager = eventoManager;
    }

    public async Task ExecutarAsync()
    {
        var mensagens = await eventoManager
            .RecuperarNaoLidasAsync<PropostaAprovada>(
                "PropostaAprovada", 
                nameof(EmissorDeFaturas));

        foreach(var mensagem in mensagens)
        {
            Fatura fatura = new()
            {
                Id = Guid.NewGuid(),
                DataEmissao = DateTime.Now,
                DataVencimento = DateTime.Now.AddDays(5),
                Numero = "304823908",
                Total = mensagem.Corpo.ValorProposta,
                //LocacaoId = ?? // ACL
            };

            await repoFatura.AddAsync(fatura);

            await eventoManager
                .MarcarComoLidaAsync(mensagem.Id, nameof(EmissorDeFaturas));
        }
    }
}
