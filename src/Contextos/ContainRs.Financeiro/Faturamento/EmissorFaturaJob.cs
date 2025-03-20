using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace ContainRs.Financeiro.Faturamento;

public class EmissorFaturaJob
{
    private readonly ILogger<EmissorFaturaJob> _logger;
    public const string INBOX_ID = "emissor-fatura";

    public EmissorFaturaJob(ILogger<EmissorFaturaJob> logger)
    {
        _logger = logger;
    }

    public Task ExecutarAsync()
    {
        _logger.LogInformation("Executando o emissor de fatura...");
        
        return Task.CompletedTask;
    }
}
