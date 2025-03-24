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

    public EmissorDeFaturas(IRepository<Fatura> repoFatura)
    {
        this.repoFatura = repoFatura;
    }

    public async Task ExecutarAsync()
    {
        var mensagens = new List<PropostaAprovada>();

        foreach(var mensagem in mensagens)
        {
            Fatura fatura = new()
            {
                Id = Guid.NewGuid(),
                DataEmissao = DateTime.Now,
                DataVencimento = DateTime.Now.AddDays(5),
                Numero = "304823908",
                Total = mensagem.ValorProposta,
                //LocacaoId = ?? // ACL
            };

            // persistir a fatura
            await repoFatura.AddAsync(fatura);

            // marcar msg como lida
        }
    }
}
