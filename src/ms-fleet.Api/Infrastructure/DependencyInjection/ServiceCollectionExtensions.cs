using ms_fleet.Api.Domain.Model;
using ms_fleet.Api.Application.UseCase;
using ms_fleet.Api.Application.UseCase.Strategy;
using ms_fleet.Api.Domain.Ports.In;
using ms_fleet.Api.Domain.Ports.Out;
using ms_fleet.Api.Infrastructure.External;
using ms_fleet.Api.Infrastructure.Persistence.Context;
using ms_fleet.Api.Infrastructure.Persistence.Repository;
using Microsoft.EntityFrameworkCore;

namespace ms_fleet.Api.Infrastructure.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddFleetServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<FleetContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<IBusRepository, BusRepository>();
        services.AddScoped<IDriverAssignmentRepository, DriverAssignmentRepository>();

        services.AddScoped<ICreateBusUseCase, CreateBusService>();
        services.AddScoped<IUpdateBusUseCase, UpdateBusService>();
        services.AddScoped<IChangeBusStatusUseCase, ChangeBusStatusService>();
        services.AddScoped<IDeleteBusUseCase, DeleteBusService>();
        services.AddScoped<IListBusesBasicUseCase, ListBusesBasicService>();
        services.AddScoped<IGetBusDetailUseCase, GetBusDetailService>();
        services.AddScoped<IAssignDriverToBusUseCase, AssignDriverToBusService>();
        services.AddScoped<IUnassignDriverFromBusUseCase, UnassignDriverFromBusService>();

        services.AddScoped<IBusSearchStrategy, PlateSearchStrategy>();
        services.AddScoped<IBusSearchStrategy, NameSearchStrategy>();
        services.AddScoped<SearchBusesService>();

        // ponytail: sin BaseAddress las llamas relativas a ms-iam revientan en tiempo de ejecución.
        // Si ms-iam pasa a exponer los perfiles detrás de Kong, apuntar a la URL del gateway.
        services.AddHttpClient<IIamService, IamService>(client =>
            client.BaseAddress = new Uri(configuration["Iam:BaseUrl"] ?? "http://ms-iam:8080"));
        services.AddHttpClient<IGpsDeviceService, GpsDeviceService>();

        return services;
    }
}
