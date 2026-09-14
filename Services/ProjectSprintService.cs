using Microsoft.EntityFrameworkCore;
using ProjectManager.Data;
using ProjectManager.Exceptions;
using Request.Models;
using Response.Models;
using Tables.Models;

namespace Service;

public class ProjectSprintService
{
    private readonly AppDbContext _dbContext;
    private readonly ProjectMembersService _projectMembersService;

    public ProjectSprintService(
        AppDbContext dbContext,
        ProjectMembersService projectMembersService)
    {
        _dbContext = dbContext;
        _projectMembersService = projectMembersService;
    }

    public async Task<Response<List<ProjectSprintResponse>>> GetProjectSprints(int projectId, string userId)
    {
        await _projectMembersService.GetProjectMember(userId, projectId);

        var sprintEntities = await _dbContext.ProjectSprint
            .Where(x => x.ProjectId == projectId)
            .OrderBy(x => x.StartDate)
            .ToListAsync();

        var sprints = sprintEntities.Select(ToResponse).ToList();

        return new Response<List<ProjectSprintResponse>>
        {
            Dados = sprints,
            Message = "Sprints do projeto encontradas com sucesso."
        };
    }

    public async Task<Response<ProjectSprintResponse>> GetProjectSprint(int projectId, int id, string userId)
    {
        await _projectMembersService.GetProjectMember(userId, projectId);

        var sprint = await _dbContext.ProjectSprint
            .FirstOrDefaultAsync(x => x.Id == id && x.ProjectId == projectId);

        if (sprint == null)
            throw new NotFoundException("Sprint não encontrada.");

        return new Response<ProjectSprintResponse>
        {
            Dados = ToResponse(sprint),
            Message = "Sprint encontrada com sucesso."
        };
    }

    public async Task<Response<ProjectSprintResponse>> CreateProjectSprint(int projectId, ProjectSprintRequest request, string userId)
    {
        await _projectMembersService.ValidateSprintTaskPermission(userId, projectId);

        var sprint = new ProjectSprint
        {
            ProjectId = projectId,
            Title = request.Title,
            Description = request.Description,
            StartDate = request.StartDate,
            EndDate = request.EndDate
        };

        _dbContext.ProjectSprint.Add(sprint);
        await _dbContext.SaveChangesAsync();

        return new Response<ProjectSprintResponse>
        {
            Dados = ToResponse(sprint),
            Message = "Sprint criada com sucesso."
        };
    }

    public async Task<Response<ProjectSprintResponse>> UpdateProjectSprint(int projectId, int id, ProjectSprintRequest request, string userId)
    {
        await _projectMembersService.ValidateSprintTaskPermission(userId, projectId);

        var sprint = await _dbContext.ProjectSprint
            .FirstOrDefaultAsync(x => x.Id == id && x.ProjectId == projectId);

        if (sprint == null)
            throw new NotFoundException("Sprint não encontrada.");

        sprint.Title = request.Title;
        sprint.Description = request.Description;
        sprint.StartDate = request.StartDate;
        sprint.EndDate = request.EndDate;

        await _dbContext.SaveChangesAsync();

        return new Response<ProjectSprintResponse>
        {
            Dados = ToResponse(sprint),
            Message = "Sprint atualizada com sucesso."
        };
    }

    public async Task<Response<bool>> DeleteProjectSprint(int projectId, int id, string userId)
    {
        await _projectMembersService.ValidateSprintTaskPermission(userId, projectId);

        var sprint = await _dbContext.ProjectSprint
            .FirstOrDefaultAsync(x => x.Id == id && x.ProjectId == projectId);

        if (sprint == null)
            throw new NotFoundException("Sprint não encontrada.");

        _dbContext.ProjectSprint.Remove(sprint);
        await _dbContext.SaveChangesAsync();

        return new Response<bool>
        {
            Dados = true,
            Message = "Sprint removida com sucesso."
        };
    }

    private static ProjectSprintResponse ToResponse(ProjectSprint sprint) => new()
    {
        id = sprint.Id,
        ProjectId = sprint.ProjectId,
        Title = sprint.Title,
        Description = sprint.Description,
        StartDate = sprint.StartDate,
        EndDate = sprint.EndDate
    };
}