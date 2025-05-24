using System.Data;

using CodeGenerator.Application.Cqrs.Commands;

using Microsoft.Extensions.DependencyInjection;

namespace CodeGenerator.Application.DependencyInjection;

internal static class ApplicationServiceRegistration
{
    /// <summary>
    /// Registers application services: MediatR, Dapper repositories, and DB connection factory.
    /// </summary>
    public static IServiceCollection AddApplicationLayer(
            this IServiceCollection services,
            Func<IServiceProvider, IDbConnection> connectionFactory)
    {
        // Register IDbConnection
        _ = services.AddTransient(sp => connectionFactory(sp));

        // Register MediatR for all Handlers
        _ = services.AddMediatR(x => x.RegisterServicesFromAssembly(typeof(GetAllDtosQuery).Assembly));

        return services;
    }
}