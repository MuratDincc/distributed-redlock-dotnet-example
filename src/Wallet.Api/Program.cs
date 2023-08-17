using Quartz;
using Quartz.AspNetCore;
using Serilog.Sinks.Elasticsearch;
using Serilog;
using Wallet.Api.Jobs;
using Serilog.Events;
using Serilog.Filters;
using Serilog.Formatting.Elasticsearch;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(builder.Configuration)
                .MinimumLevel.Override("Microsoft", LogEventLevel.Error)
                .MinimumLevel.Override("System", LogEventLevel.Error)
                .Enrich.FromLogContext()
                .Filter.ByExcluding(Matching.FromSource("Microsoft.AspNetCore.StaticFiles"))
                .WriteTo.Async(writeTo => writeTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri(builder.Configuration["ElasticConfiguration:uri"]))
                {
                    TypeName = null,
                    AutoRegisterTemplate = true,
                    IndexFormat = $"Wallet.Api-{DateTime.UtcNow:yyyy-MM}",
                    BatchAction = ElasticOpType.Create,
                    CustomFormatter = new ElasticsearchJsonFormatter(),
                    OverwriteTemplate = true,
                    DetectElasticsearchVersion = true,
                    AutoRegisterTemplateVersion = AutoRegisterTemplateVersion.ESv7
                }))
               .CreateLogger();

builder.Logging.ClearProviders();
builder.Host.UseSerilog(Log.Logger, true);

builder.Services.AddQuartz(q =>
{
    q.UseMicrosoftDependencyInjectionJobFactory();

    var recurringPaymentJobKey = new JobKey("RecurringPaymentJob");

    q.AddJob<RecurringPaymentJob>(opts => opts.WithIdentity(recurringPaymentJobKey));

    q.AddTrigger(opts => opts
        .ForJob(recurringPaymentJobKey)
        .WithIdentity("RecurringPaymentJob-trigger")
        .WithSimpleSchedule(x => x.WithIntervalInMinutes(1).RepeatForever())
    );

    var bonusCalculatorJobKey = new JobKey("BonusCalculatorJob");

    q.AddJob<BonusCalculatorJob>(opts => opts.WithIdentity(bonusCalculatorJobKey));

    q.AddTrigger(opts => opts
        .ForJob(bonusCalculatorJobKey)
        .WithIdentity("BonusCalculatorJob-trigger")
        .WithSimpleSchedule(x => x.WithIntervalInMinutes(3).RepeatForever())
    );
});

builder.Services.AddQuartzServer(options =>
{
    options.WaitForJobsToComplete = true;
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.UseRouting();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();