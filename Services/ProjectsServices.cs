

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using ProjectManager.Data;
using ProjectManager.Exceptions;
using Request.Models;
using Response.Models;
using Tables.Models;

namespace Service;


public class ProjectsService
{
    
    private readonly AppDbContext _dbContext;

    public ProjectsService (AppDbContext dbContext)
    {
         _dbContext = dbContext;
        
    }


    public async Task<List<ProjectsResponse>> GetProjects(string memberId)
    {
        return await _dbContext.Projects
            .Include(f => f.Members)
            .Where(p => p.Members.Any(m => m.UserId == memberId))
            .Select(r => new ProjectsResponse
            {
                id = r.Id,
                Name = r.Name,
                Description = r.Description,
                StartDate = r.StartDate,
                EstimatedDate = r.EstimatedDate,
                EndDate = r.EndDate
            })
            .ToListAsync();
    }

    public async Task<ProjectsResponse> CreateProject(
        ProjectsRequest request)
    {
        var project = new Projects
        {
            Name = request.Name,
            Description = request.Description,
            StartDate = request.StartDate,
            EstimatedDate = request.EstimatedDate,
            EndDate = request.EndDate

        };

        _dbContext.Projects.Add(project);

        await _dbContext.SaveChangesAsync();

        return new ProjectsResponse
        {
            id = project.Id,
            Name = project.Name,
            Description = project.Description,
            StartDate = project.StartDate,
            EstimatedDate = project.EstimatedDate,
            EndDate = project.EndDate
        };
    }

    public async Task<Response<ProjectsResponse>> UpdateProject(int id, ProjectsRequest request, string userId)
    {
       
        Response<ProjectsResponse> response = new ();

        var project = await _dbContext.Projects.FindAsync(id);

        if(project == null)
        {
            response.Message = "Projeto não existe!";
            response.Status = false;
            return response;
        }

        await ValidateMemberPermission(userId, id);

    
        project.Name = request.Name;
        project.Description = request.Description;
        project.StartDate = request.StartDate;
        project.EstimatedDate = request.EstimatedDate;
        project.EndDate = request.EndDate;

        await _dbContext.SaveChangesAsync();

        response.Message ="Projeto Editado com Sucesso!";
        response.Dados =  new ProjectsResponse
        {
            id = project.Id,
            Name = project.Name,
            Description = project.Description,
            StartDate = project.StartDate,
            EstimatedDate = project.EstimatedDate,
            EndDate = project.EndDate
        };

        return response;
    }

    public async Task <Response<bool>> DeleteProject(int id, string userId)
    {
        Response<bool> response = new Response<bool>();

        var project = await _dbContext.Projects.FindAsync(id);

        if (project is null)
        {
            response.Status = false;
            response.Dados = false;
            response.Message = "Projeto não encontrado.";
            return response;
        }

        await ValidateMemberPermission(userId, id);

        _dbContext.Projects.Remove(project);
        await _dbContext.SaveChangesAsync();

        response.Message = "Usuário não pode editar o projeto!";
        response.Dados = true;    
        return response;
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

         public async Task ValidateMemberPermission(string userId, int projectId)
    {
        var member = await GetProjectMember(userId, projectId);

        if (member.Role != ProjectRole.Owner &&
            member.Role != ProjectRole.Admin)
        {
            throw new ForbiddenException("Você não possui permissão de edição do projeto.");
        }
    }

}