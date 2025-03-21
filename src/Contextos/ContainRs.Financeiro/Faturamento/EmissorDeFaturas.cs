using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ContainRs.Contracts;
using Microsoft.Extensions.Logging;

namespace ContainRs.Financeiro.Faturamento;


public class PropostaAprovadaDto
{
    public Guid IdProposta { get; set; }
    public decimal ValorProposta { get; set; }
}

public class EmissorDeFaturas
{
    private readonly ILogger<EmissorDeFaturas> logger;
    private readonly IRepository<Fatura> repoFatura;

    public EmissorDeFaturas(ILogger<EmissorDeFaturas> logger, IRepository<Fatura> repoFatura)
    {
        this.logger = logger;
        this.repoFatura = repoFatura;
    }

    public async Task ExecuteAsync()
    {
        // SELECT..
        logger.LogInformation("Executando o emissor de faturas...");

        // ler as mensagens de saída
        var propostas = Enumerable.Empty<PropostaAprovadaDto>();

        foreach (var proposta in propostas)
        {
            var fatura = new Fatura()
            {
                Id = Guid.NewGuid(),
                DataEmissao = DateTime.Now,
                DataVencimento = DateTime.Now.AddDays(5),
                Total = proposta.ValorProposta,
                LocacaoId = proposta.IdProposta, // ACL
                Numero = "3232090"
            };
            await repoFatura.AddAsync(fatura);
            // marcar mensagem como lida
        }
    }
}
