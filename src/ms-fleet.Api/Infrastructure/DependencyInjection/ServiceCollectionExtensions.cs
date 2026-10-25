using ms_fleet.Api.Domain.Model;
using ms_fleet.Api.Application.UseCase;
using ms_fleet.Api.Application.UseCase.Strategy;
using ms_fleet.Api.Domain.Ports.In;
using ms_fleet.Api.Domain.Ports.Out;
using ms_fleet.Api.Infrastructure.Auth;
using ms_fleet.Api.Infrastructure.External;
using ms_fleet.Api.Infrastructure.Grpc;
using ms_fleet.Api.Infrastructure.Persistence.Context;
using ms_fleet.Api.Infrastructure.Persistence.Repository;
using Microsoft.EntityFrameworkCore;

namespace ms_fleet.Api.Infrastructure.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddFleetServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContextPool<FleetContext>(
            (sp, options) => options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"),
                sql => sql.EnableRetryOnFailure(3, TimeSpan.FromSeconds(5), null)),
            poolSize: 32);

        services.AddHttpContextAccessor();
        services.AddGatewayAuthentication();

        // Servidor gRPC de route.v1.RouteService (bus para las vistas de ruta
        // de ms-route). Toda llamada interna debe traer x-internal-api-key.
        services.AddGrpc(options => options.Interceptors.Add<InternalApiKeyInterceptor>());

        services.AddScoped<IBusRepository, BusRepository>();
        services.AddScoped<IDriverAssignmentRepository, DriverAssignmentRepository>();
        services.AddScoped<ICampusReferenceRepository, CampusReferenceRepository>();
        services.AddScoped<ITenantProvider, HttpContextTenantProvider>();

        services.AddScoped<ICreateBusUseCase, CreateBusService>();
        services.AddScoped<IUpdateBusUseCase, UpdateBusService>();
        services.AddScoped<IChangeBusStatusUseCase, ChangeBusStatusService>();
        services.AddScoped<IDeleteBusUseCase, DeleteBusService>();
        services.AddScoped<IListBusesBasicUseCase, ListBusesBasicService>();
        services.AddScoped<IGetBusDetailUseCase, GetBusDetailService>();
        services.AddScoped<IGetBusAssignedToDriverUseCase, GetBusAssignedToDriverService>();
        services.AddScoped<IAssignDriverToBusUseCase, AssignDriverToBusService>();
        services.AddScoped<IUnassignDriverFromBusUseCase, UnassignDriverFromBusService>();

        services.AddScoped<IBusSearchStrategy, PlateSearchStrategy>();
        services.AddScoped<IBusSearchStrategy, NameSearchStrategy>();
        services.AddScoped<SearchBusesService>();

        services.AddScoped<IBrandRepository, BrandRepositoryImpl>();
        services.AddScoped<IModelRepository, ModelRepositoryImpl>();
        services.AddScoped<IListBrandsUseCase, ListBrandsService>();
        services.AddScoped<IListModelsUseCase, ListModelsService>();

        // Los perfiles (existe / nombre) los expone ms-iam a través de gRPC.
        // services.AddHttpClient<IIamService, IamService>(client =>
        //     client.BaseAddress = new Uri(configuration["Iam:BaseUrl"] ?? "http://ms-user-management:8080"));
        services.AddScoped<IIamService, IamGrpcClient>();
        services.AddScoped<IGpsDeviceService, GpsDeviceService>();

        return services;
    }
}
