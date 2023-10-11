using BusinessLayer.Validation;

namespace UserApp.API.Extensions;

public static class ValidatorsExtensions
{
    public static IServiceCollection AddValidators(this IServiceCollection services)
    {
        services.AddScoped<UserValidator>();
        services.AddScoped<UserParametersValidator>();
        services.AddScoped<AccountValidator>();

        return services;
    }
}