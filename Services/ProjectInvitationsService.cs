
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using ProjectManager.Data;
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

}


