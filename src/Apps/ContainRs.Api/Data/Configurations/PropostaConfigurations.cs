using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ContainRs.Api.Data.Configurations;

public class PropostaConfigurations : IEntityTypeConfiguration<Proposta>
{
    public void Configure(EntityTypeBuilder<Proposta> builder)
    {
        builder.OwnsOne(p => p.ValorTotal, builder =>
        {
            builder.Property(v => v.Valor)
            .HasColumnType("decimal(18,2)");
        }); 

        builder.OwnsOne(p => p.Situacao, status =>
        {
            status.Property(s => s.Status)
                .HasColumnName("Status")
                .HasConversion<string>();
        });

        builder.HasOne(p => p.Solicitacao)
            .WithMany(s => s.Propostas)
            .HasForeignKey(p => p.SolicitacaoId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
