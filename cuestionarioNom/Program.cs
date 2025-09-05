using AutoMapper;
using cuestionarioNom.Data;
using cuestionarioNom.Infrastructure.Mapping;
using cuestionarioNom.Services;
using cuestionarioNom.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

var mapperConfig = new MapperConfiguration(cfg =>
{
    cfg.AddProfile<MappingProfile>();
});
var mapper = mapperConfig.CreateMapper();

builder.Services.AddDbContext<ApplicationDbContext>(opt =>
    opt.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sql => sql.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null) // resiliencia
              .CommandTimeout(60)
    )
);


builder.Services.AddAutoMapper(typeof(MappingProfile));
builder.Services.AddScoped<IScoringService, ScoringService>();
builder.Services.AddScoped<IImportService, ImportService>();
builder.Services.AddScoped<IReportService, DummyReportService>(); // opcional (stub)

builder.Services.AddSingleton(mapper);
builder.Services.AddSingleton<AutoMapper.IConfigurationProvider>(mapperConfig);

builder.Services.AddControllersWithViews();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Questionnaires}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

// Stubs para compilar si aún no implementas import/report
public class DummyImportService : IImportService
{
    public Task<int> ImportAsync(cuestionarioNom.Models.Dtos.ImportQuestionnaireDto dto) => Task.FromResult(0);
}
public class DummyReportService : IReportService
{
    public Task<byte[]> ExportSurveySummaryAsync(Guid surveyId) => Task.FromResult(Array.Empty<byte>());
}
