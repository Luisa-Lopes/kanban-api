
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using ProjectManager.Data;
using ProjectManager.Exceptions;
using Request.Models;
using Response.Models;
using Tables.Models;

namespace Service;


public class ProjectInvitationsService

{

    private readonly AppDbContext _dbContext;

    private readonly ProjectMembersService _projectMemberService;

    public ProjectInvitationsService (AppDbContext dbContext, ProjectMembersService projectMembersService)
    {
         _dbContext = dbContext;
        _projectMemberService = projectMembersService;
    }

    public async Task InvitationExist (int projectId, string email)
    {
        

        var invitationExists = await _dbContext.ProjectInvitation
            .AnyAsync(x =>
                x.ProjectId == projectId &&
                x.Email == email &&
                x.Status == InvitationStatus.Pending);

        if (invitationExists)
        {
            throw new Exception(
            "Já existe um convite pendente.");
        }
    }

    public async Task<Response<List<ProjectInvitationResponse>>> GetInvitations(
        int projectId,
        string userId)
    {
        await _projectMemberService.ThisProjectExist(projectId);
        await _projectMemberService.ValidateMemberPermission(userId, projectId);

        var invitations = await _dbContext.ProjectInvitation
            .AsNoTracking()
            .Where(invitation => invitation.ProjectId == projectId)
            .OrderByDescending(invitation => invitation.CreatedAt)
            .Select(invitation => new ProjectInvitationResponse
            {
                Id = invitation.Id,
                ProjectId = invitation.ProjectId,
                Email = invitation.Email,
                Role = invitation.Role,
                Status = invitation.Status,
                InvitesBy = invitation.InvitesBy,
                CreatedAt = invitation.CreatedAt
            })
            .ToListAsync();

        return new Response<List<ProjectInvitationResponse>>
        {
            Dados = invitations,
            Message = "Convites do projeto encontrados com sucesso.",
            StatusCode = StatusCodes.Status200OK
        };
    }

    public async Task<Response<ProjectInvitationResponse>> CreateProjectInvitation (ProjectInvitationsRequest request, string invitesBy)
    {
        Response<ProjectInvitationResponse> response = new ();

        try
        {
            

            await _projectMemberService.ThisProjectExist(request.ProjectId);

            await InvitationExist(request.ProjectId, request.Email);

            await _projectMemberService.ValidateMemberPermission(invitesBy,request.ProjectId);


            var projectInvitation = new ProjectInvitations
            {
                ProjectId = request.ProjectId,
                Email = request.Email,
                Role = request.Role,
                Token = Guid.NewGuid(),
                Status = InvitationStatus.Pending,
                InvitesBy = invitesBy,
                CreatedAt = DateTime.UtcNow,
            };

            _dbContext.ProjectInvitation.Add(projectInvitation);

            await _dbContext.SaveChangesAsync();


            response.Dados = new ProjectInvitationResponse
            {
                Id = projectInvitation.Id,
                ProjectId = projectInvitation.ProjectId,
                Email = projectInvitation.Email,
                Role = projectInvitation.Role,
                Status = projectInvitation.Status,
                InvitesBy = projectInvitation.InvitesBy,
                CreatedAt = projectInvitation.CreatedAt
            };

            response.Message = "Convite Adicionado";
            response.StatusCode = StatusCodes.Status200OK;

            return response;

        }
        catch (Exception ex)
        {
            response.Status = false;
            response.StatusCode = StatusCodes.Status417ExpectationFailed;
            response.Message = ex.Message;

            return response;   
        }
    }

    public async Task<Response<ProjectInvitationResponse>> RespondToInvitation(
        int invitationId,
        string userId,
        string userEmail,
        InvitationStatus status)
    {
        var invitation = await _dbContext.ProjectInvitation
            .FirstOrDefaultAsync(x => x.Id == invitationId);

        if (invitation == null)
            throw new NotFoundException("Convite não encontrado.");

        if (!string.Equals(invitation.Email, userEmail, StringComparison.OrdinalIgnoreCase))
            throw new ForbiddenException("Este convite foi enviado para outro usuário.");

        if (invitation.Status != InvitationStatus.Pending)
            throw new ConflictException("Este convite já foi respondido ou não está mais pendente.");

        if (status == InvitationStatus.Accepted)
        {
            var alreadyMember = await _dbContext.ProjectMembers
                .AnyAsync(x => x.ProjectId == invitation.ProjectId && x.UserId == userId);

            if (alreadyMember)
                throw new ConflictException("O usuário já participa deste projeto.");

            _dbContext.ProjectMembers.Add(new ProjectMembers
            {
                ProjectId = invitation.ProjectId,
                UserId = userId,
                InvitationSent = invitation.CreatedAt,
                JoinedAt = DateTime.UtcNow,
                Role = invitation.Role
            });
        }

        invitation.Status = status;
        await _dbContext.SaveChangesAsync();

        return new Response<ProjectInvitationResponse>
        {
            Dados = new ProjectInvitationResponse
            {
                Id = invitation.Id,
                ProjectId = invitation.ProjectId,
                Email = invitation.Email,
                Role = invitation.Role,
                Status = invitation.Status,
                InvitesBy = invitation.InvitesBy,
                CreatedAt = invitation.CreatedAt
            },
            Message = status == InvitationStatus.Accepted
                ? "Convite aceito com sucesso."
                : "Convite recusado com sucesso.",
            StatusCode = StatusCodes.Status200OK
        };
    }

}


