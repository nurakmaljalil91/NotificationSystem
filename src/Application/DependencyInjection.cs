using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Application.Common.Behaviours;
using Application.Common.Models;
using Application.Common.Interfaces;
using Application.Common.Services;
using Application.Notifications.Commands;
using Application.Notifications.Models;
using Application.Notifications.Queries;
using Domain.Common;
using FluentValidation;
using Mediator;
using Microsoft.Extensions.DependencyInjection;

namespace Application;

/// <summary>
/// Provides extension methods for registering application services in the dependency injection container.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Registers application-specific services to the provided <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The service collection to add services to.</param>
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));
        services.AddScoped<IMediator, Mediator.Mediator>();
        services.AddScoped<IRequestHandler<GetNotificationsQuery, BaseResponse<PaginatedEnumerable<NotificationItemDto>>>,
            GetNotificationsQueryHandler>();
        services.AddScoped<IRequestHandler<MarkNotificationReadCommand, BaseResponse<NotificationItemDto>>,
            MarkNotificationReadCommandHandler>();
        services.AddSingleton<INotificationRealtimePublisher, NullNotificationRealtimePublisher>();
        return services;
    }
}
