using Microsoft.EntityFrameworkCore;
using ProjectManager.Data;
using Response.Models;
using Tables.Models;

namespace Service;

public class NotificationsService
{
    private readonly AppDbContext _dbContext;

    public NotificationsService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Response<List<NotificationResponse>>> GetNotifications(string userEmail)
    {
        var invitations = await (
            from invitation in _dbContext.ProjectInvitation.AsNoTracking()
            join project in _dbContext.Projects.AsNoTracking()
                on invitation.ProjectId equals project.Id
            where invitation.Email == userEmail && invitation.Status == InvitationStatus.Pending
            orderby invitation.CreatedAt descending
            select new NotificationResponse
            {
                InvitationId = invitation.Id,
                ProjectId = project.Id,
                ProjectName = project.Name,
                Role = invitation.Role,
                CreatedAt = invitation.CreatedAt
            }).ToListAsync();

        return new Response<List<NotificationResponse>>
        {
            Dados = invitations,
            Message = invitations.Count == 0
                ? "Nenhum convite pendente encontrado."
                : "Notificações encontradas com sucesso.",
            StatusCode = StatusCodes.Status200OK
        };
    }
}
