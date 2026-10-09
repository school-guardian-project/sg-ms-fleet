using ms_fleet.Api.Domain.Model;
using ms_fleet.Api.Application.UseCase;
using ms_fleet.Api.Application.UseCase.Strategy;
using ms_fleet.Api.Domain.Ports.In;
using ms_fleet.Api.Domain.Ports.Out;
using ms_fleet.Api.Infrastructure.External;
using ms_fleet.Api.Infrastructure.Persistence.Context;
using ms_fleet.Api.Infrastructure.Persistence.Repository;
using ms_fleet.Api.Infrastructure.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace ms_fleet.Api.Infrastructure.DependencyInjection;

public static class ServiceCollectionExtensions
{
    // Mismo default literal que usa ms-iam cuando no se define JWT_SECRET.
    private const string DefaultJwtSecret = "your-256-bit-base64-encoded-secret-min-32-chars";

    public static IServiceCollection AddFleetServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<FleetContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        services.AddHttpContextAccessor();
        services.AddJwtAuthentication(configuration);

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

        // Los perfiles (existe / nombre) los expone ms-user-management en api/profiles,
        // sin token: ms-iam exige JWT y respondía 401, dejando vacío el conductor.
        services.AddHttpClient<IIamService, IamService>(client =>
            client.BaseAddress = new Uri(configuration["Iam:BaseUrl"] ?? "http://ms-user-management:8080"));
        services.AddScoped<IGpsDeviceService, GpsDeviceService>();

        return services;
    }

    /// <summary>
    /// Autentica el JWT (HS256) emitido por ms-iam cuando la petición lo trae.
    /// Deliberadamente sin UseAuthorization ni [Authorize]: sin token o con token
    /// inválido la petición continúa como anónima (ms-route llama internamente sin token).
    /// Secreto: env JWT_SECRET > config "Jwt:Secret" > default literal de ms-iam.
    /// </summary>
    public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        var secret = Environment.GetEnvironmentVariable("JWT_SECRET")
            ?? configuration["Jwt:Secret"]
            ?? DefaultJwtSecret;

        services.AddAuthentication(options =>
        {
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret)),
                ValidateLifetime = true,
                ClockSkew = TimeSpan.FromMinutes(1)
            };
        });

        return services;
    }
}
