using ContainRs.Api.Data;
using ContainRs.Api.Data.Repositories;
using ContainRs.Api.Identity;
using Hangfire;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<IdentityDbContext>(options =>
{
    options
        .UseSqlServer(builder.Configuration.GetConnectionString("IdentityDB"));
});

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options
        .UseSqlServer(builder.Configuration.GetConnectionString("ContainRsDB"));
});

builder.Services.AddScoped<IRepository<Cliente>, ClienteRepository>();
builder.Services.AddScoped<IRepository<PedidoLocacao>, SolicitacaoRepository>();
builder.Services.AddScoped<IRepository<Proposta>, PropostaRepository>();
builder.Services.AddScoped<IRepository<Locacao>, LocacaoRepository>();
builder.Services.AddScoped<IRepository<Conteiner>, ConteinerRepository>();
builder.Services.AddScoped<IRepository<Fatura>, FaturaRepository>();

builder.Services.AddScoped<ICalculadoraPrazosLocacao, CalculadoraPadraoPrazosLocacao>();
builder.Services.AddScoped<IPropostaService, PropostaService>();
builder.Services.AddScoped<IAcessoManager, AcessoManagerWithIdentity>();

builder.Services
    .AddIdentityApiEndpoints<AppUser>(options => options.SignIn.RequireConfirmedEmail = true)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<IdentityDbContext>();

builder.Services.AddAuthorization();
builder.Services.AddHttpContextAccessor();

builder.Services.Configure<IdentityOptions>(options =>
{
    options.ClaimsIdentity.UserIdClaimType = "ClienteId";
});

builder.Services.AddHangfire(config =>
{
    config.UseSqlServerStorage(builder.Configuration.GetConnectionString("ContainRsDB"));
}).AddHangfireServer(options => options.SchedulePollingInterval = TimeSpan.FromSeconds(5));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app
    .MapIdentityEndpoints()
    .MapClientesEndpoints()
    .MapAprovacaoClientesEndpoints()
    .MapSolicitacoesEndpoints()
    .MapPropostasEndpoints()
    .MapLocacoesEndpoints()
    .MapConteineresEndpoints();

app.Services
    .GetRequiredService<IRecurringJobManager>()
    .AddOrUpdate<EmissorDeFaturas>(
        nameof(EmissorDeFaturas),
        job => job.ExecuteAsync(),
        "* * * * *" // executando a cada minuto
    );

app.Run();