using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace ContainRs.Financeiro.Faturamento;

public class EmissorDeFaturas
{
    private readonly ILogger<EmissorDeFaturas> logger;

    public EmissorDeFaturas(ILogger<EmissorDeFaturas> logger)
    {
        this.logger = logger;
    }

    public async Task ExecuteAsync()
    {
        // SELECT..
        logger.LogInformation("Executando o emissor de faturas...");

        return Task.CompletedTask;
    }
}
