using ContainRs.Financeiro.Faturamento;

namespace ContainRs.Api.Data.Repositories;

public class FaturaRepository : BaseRepository<Fatura>
{
    public FaturaRepository(AppDbContext dbContext) : base(dbContext)
    {
    }
}
