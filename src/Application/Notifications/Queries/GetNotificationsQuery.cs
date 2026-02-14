using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Notifications.Models;
using Domain.Common;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace Application.Notifications.Queries;

/// <summary>
/// Retrieves notifications for a recipient with pagination.
/// </summary>
public sealed class GetNotificationsQuery : PaginatedRequest, IRequest<BaseResponse<PaginatedEnumerable<NotificationItemDto>>>
{
    /// <summary>
    /// Gets or sets the recipient identifier.
    /// </summary>
    public string RecipientId { get; set; } = string.Empty;
}

/// <summary>
/// Handles notification retrieval for a recipient.
/// </summary>
public sealed class GetNotificationsQueryHandler
    : IRequestHandler<GetNotificationsQuery, BaseResponse<PaginatedEnumerable<NotificationItemDto>>>
{
    private readonly IApplicationDbContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetNotificationsQueryHandler"/> class.
    /// </summary>
    public GetNotificationsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc />
    public async Task<BaseResponse<PaginatedEnumerable<NotificationItemDto>>> Handle(
        GetNotificationsQuery request,
        CancellationToken cancellationToken)
    {
        var recipientId = request.RecipientId?.Trim();
        if (string.IsNullOrWhiteSpace(recipientId))
        {
            return BaseResponse<PaginatedEnumerable<NotificationItemDto>>.Fail("RecipientId is required.");
        }

        var page = request.Page <= 0 ? 1 : request.Page;
        var pageSize = request.Total <= 0 ? 10 : request.Total;

        var baseQuery = _context.NotificationRecipients
            .AsNoTracking()
            .Where(item => item.RecipientId == recipientId && item.InAppEnabled)
            .Include(item => item.Notification)
            .OrderByDescending(item => item.Notification.CreatedDate);

        var totalCount = await baseQuery.CountAsync(cancellationToken);

        var items = await baseQuery
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(item => NotificationItemDto.FromEntities(item.Notification, item))
            .ToListAsync(cancellationToken);

        var paginated = new PaginatedEnumerable<NotificationItemDto>(
            items,
            totalCount,
            page,
            pageSize);

        return BaseResponse<PaginatedEnumerable<NotificationItemDto>>.Ok(
            paginated,
            $"Successfully retrieved {paginated.Items?.Count() ?? 0} notifications.");
    }
}
