using Application.Interfaces.Services;
using Application.Models.DTOs;
using Application.Services;
using Microsoft.Extensions.DependencyInjection;
using Shared;

namespace Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Services
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IPostService, PostService>();

        // Caches
        services.AddSingleton<Cache<UserDto>>();
        services.AddSingleton<Cache<PostDto>>();

        return services;
    }
}
