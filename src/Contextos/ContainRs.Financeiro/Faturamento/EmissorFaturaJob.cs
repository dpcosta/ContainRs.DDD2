using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
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

public class EmissorFaturaJob
{
    public const string INBOX_ID = "emissor-fatura";
    private readonly ILogger<EmissorFaturaJob> _logger;
    private readonly IRepository<Fatura> repoFatura;
    private readonly IEventManager eventManager;

    public EmissorFaturaJob(ILogger<EmissorFaturaJob> logger, IRepository<Fatura> repoFatura, IEventManager eventManager)
    {
        _logger = logger;
        this.repoFatura = repoFatura;
        this.eventManager = eventManager;
    }

    public async Task ExecutarAsync()
    {
        _logger.LogInformation("Executando o emissor de fatura...");

        var mensagens = await eventManager
            .GetNotProcessedAsync<PropostaAprovadaDto>("PropostaAprovada", nameof(EmissorFaturaJob));

        foreach (var proposta in mensagens)
        {
            // gerar fatura a partir da proposta
            var fatura = new Fatura()
            {
                Id = Guid.NewGuid(),
                Numero = $"{DateTime.Now.Year}000000001",
                DataEmissao = DateTime.Now,
                DataVencimento = DateTime.Now.AddDays(5),
                //LocacaoId = ? recuperar via ACL
                Total = proposta.ValorProposta,
            };
            await repoFatura.AddAsync(fatura);
        }
    }
}
