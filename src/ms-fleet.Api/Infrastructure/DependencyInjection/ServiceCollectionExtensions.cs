using ms_fleet.Api.Application.UseCase;
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
        services.AddScoped<IGetBusUseCase, GetBusService>();
        services.AddScoped<IListBusesUseCase, ListBusesService>();
        services.AddScoped<IAssignDriverToBusUseCase, AssignDriverToBusService>();
        services.AddScoped<IUnassignDriverFromBusUseCase, UnassignDriverFromBusService>();

        services.AddHttpClient<IIamService, IamService>();
        services.AddHttpClient<IGpsDeviceService, GpsDeviceService>();

        return services;
    }
}
