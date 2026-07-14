
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using ProjectManager.Data;
using ProjectManager.Exceptions;
using Request.Models;
using Response.Models;
using Tables.Models;

namespace Service;


public class ProjectMembersService
{
    
    private readonly AppDbContext _dbContext;

    public ProjectMembersService (AppDbContext dbContext)
    {
       
        _dbContext = dbContext;
    }

    public async Task<ProjectMembers> GetProjectMember(string userId, int projectId)
        {
            var member = await _dbContext.ProjectMembers
                .FirstOrDefaultAsync(x =>
                    x.ProjectId == projectId &&
                    x.UserId == userId);

            if (member == null)
                throw new Exception("Usuário não participa do projeto.");

            return member;
        }


   public async Task<Response<List<ProjectMembersResponse>>> GetProjectMembers(int projectId, string userId)
    {
        Response<List<ProjectMembersResponse>> response = new ();

        await ThisProjectExist(projectId);


        List <ProjectMembersResponse> projectMembers = await _dbContext.ProjectMembers.Include(x => x.User)
        .Where(x => x.ProjectId == projectId)
        .Select(x => new ProjectMembersResponse
        {
            Id = x.Id,
            ProjectId = x.ProjectId,
            User = new UserResponse { 
                FirstName = x.User.FirstName,
                LastName = x.User.LastName,
                Email = x.User.Email!,
                Id = x.User.Id,
            },
            JoinedAt = x.JoinedAt,
            Role = x.Role
        })
        .ToListAsync();

        var userHasAccess = projectMembers.Any(f => f.User.Id == userId);

        await ValidateMemberPermission(userId, projectId);

        response.Dados = projectMembers;
        response.Message = "Membros do projeto encontrado com sucesso.";
           
        return response;
           
    }

    public async Task<Response<ProjectMembersResponse>> CreateProjectOwner(ProjectMembersRequest request)
        {
            Response<ProjectMembersResponse> response = new();

            var userInfo = await _dbContext.Users
                .FirstOrDefaultAsync(x => x.Email == request.Email);

            if (userInfo == null)
            {
                response.Message = "Usuário não encontrado.";
                response.StatusCode = StatusCodes.Status404NotFound;
                return response;
            }

            var projectMember = new ProjectMembers
            {
                ProjectId = request.ProjectId,
                UserId = userInfo.Id,
                InvitationSent = DateTime.Now,
                JoinedAt = DateTime.Now,
                Role = ProjectRole.Owner
            };

            _dbContext.ProjectMembers.Add(projectMember);
            await _dbContext.SaveChangesAsync();

            response.Dados = new ProjectMembersResponse
            {
                Id = projectMember.Id,
                ProjectId = projectMember.ProjectId,
                User = new UserResponse
                {
                    FirstName = userInfo.FirstName,
                    LastName = userInfo.LastName,
                    Email = userInfo.Email!
                },
                JoinedAt = projectMember.JoinedAt,
                Role = projectMember.Role
            };

            response.Message = "Projeto criado com sucesso!";
            response.StatusCode = StatusCodes.Status200OK;

            return response;
            
    }

    public async Task<Response<ProjectMembersResponse>> AddProjectMember(
        int projectId,
        string userId,
        ProjectRole role)
    {
        Response<ProjectMembersResponse> response = new();

        var user = await _dbContext.Users
            .FirstOrDefaultAsync(x => x.Id == userId);

        if (user == null)
            throw new Exception("Usuário não encontrado.");

        var memberExists = await _dbContext.ProjectMembers
            .AnyAsync(x => x.ProjectId == projectId && x.UserId == userId);

        if (memberExists)
            throw new Exception("O usuário já faz parte deste projeto.");

        var projectMember = new ProjectMembers
        {
            ProjectId = projectId,
            UserId = userId,
            JoinedAt = DateTime.Now,
            InvitationSent = DateTime.Now,
            Role = role
        };

        _dbContext.ProjectMembers.Add(projectMember);
        await _dbContext.SaveChangesAsync();

        response.Dados = new ProjectMembersResponse
        {
            Id = projectMember.Id,
            ProjectId = projectMember.ProjectId,
            JoinedAt = projectMember.JoinedAt,
            Role = projectMember.Role,
            User = new UserResponse
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email!
            }
        };

        response.Message = "Membro adicionado com sucesso!";
        response.StatusCode = StatusCodes.Status200OK;

        return response;
    }



   public async Task ValidateMemberPermission(string userId, int projectId)
    {
        var member = await GetProjectMember(userId, projectId);

        if (member.Role != ProjectRole.Owner &&
            member.Role != ProjectRole.Admin)
        {
            throw new ForbiddenException("Você não possui permissão de edição do projeto.");
        }
    }

     public async Task ThisProjectExist (int projectId)
    {

        Response<bool> response = new ();

        var project = await _dbContext.Projects
            .FirstOrDefaultAsync(x => x.Id == projectId);

        if (project == null)
        {
        
            throw new NotFoundException( "Projeto não encontrado.");   
        }

    }

}


