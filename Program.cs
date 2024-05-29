using Infrastructure.Data.Contexts;
using Infrastructure.Entities;
using Infrastructure.Repositories;
using Infrastructure.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureServices((context, services) =>
    {
        services.AddHttpClient();
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();
        services.AddDbContext<IdentityDataContext>(x => x.UseSqlServer(context.Configuration.GetConnectionString("IdentityServer")));
        services.AddDbContext<FeedbackItemsDataContext>(x => x.UseSqlServer(context.Configuration.GetConnectionString("FeedbackItems")));
        services.AddDefaultIdentity<UserEntity>(x =>
        {
        }).AddEntityFrameworkStores<IdentityDataContext>();

        services.AddSingleton<ReviewTextRepository>();
        services.AddSingleton<RatingRepository>();
        services.AddSingleton<ReviewRepository>();
        
        services.AddSingleton<ReviewCrudService>();
        services.AddSingleton<ReviewService>();
    })
    .Build();

//using (var scope = host.Services.CreateScope())
//{
//    var context = scope.ServiceProvider.GetRequiredService<FeedbackItemsDataContext>();
//    context.Database.Migrate();
//}

host.Run();
