using Microsoft.EntityFrameworkCore;
using ProjectManager.Data;
using ProjectManager.Exceptions;
using Request.Models;
using Response.Models;
using Tables.Models;

namespace Service;

public class ProjectTaskService
{
    private readonly AppDbContext _dbContext;
    private readonly ProjectMembersService _projectMembersService;

    public ProjectTaskService(AppDbContext dbContext, ProjectMembersService projectMembersService)
    {
        _dbContext = dbContext;
        _projectMembersService = projectMembersService;
    }

    public async Task<Response<List<ProjectTaskResponse>>> GetProjectTasks(int sprintId, string userId)
    {
        var sprint = await GetSprint(sprintId);
        await _projectMembersService.GetProjectMember(userId, sprint.ProjectId);

        var taskEntities = await _dbContext.ProjectTasks
            .Where(x => x.ProjectSprintId == sprintId)
            .OrderBy(x => x.Id)
            .ToListAsync();

        var tasks = taskEntities.Select(ToResponse).ToList();

        return new Response<List<ProjectTaskResponse>>
        {
            Dados = tasks,
            Message = "Tarefas da sprint encontradas com sucesso."
        };
    }

    public async Task<Response<ProjectTaskResponse>> GetProjectTask(int sprintId, int id, string userId)
    {
        var sprint = await GetSprint(sprintId);
        await _projectMembersService.GetProjectMember(userId, sprint.ProjectId);

        var task = await _dbContext.ProjectTasks
            .FirstOrDefaultAsync(x => x.Id == id && x.ProjectSprintId == sprintId);

        if (task == null)
            throw new NotFoundException("Tarefa não encontrada.");

        return new Response<ProjectTaskResponse>
        {
            Dados = ToResponse(task),
            Message = "Tarefa encontrada com sucesso."
        };
    }

    public async Task<Response<ProjectTaskResponse>> CreateProjectTask(int sprintId, ProjectTasksRequest request, string userId)
    {
        var sprint = await GetSprint(sprintId);
        await _projectMembersService.ValidateSprintTaskPermission(userId, sprint.ProjectId);

        var task = new ProjectTasks
        {
            ProjectSprintId = sprintId,
            Title = request.Title,
            Description = request.Description,
            Status = request.Status
        };

        _dbContext.ProjectTasks.Add(task);
        await _dbContext.SaveChangesAsync();

        return new Response<ProjectTaskResponse>
        {
            Dados = ToResponse(task),
            Message = "Tarefa criada com sucesso."
        };
    }

    public async Task<Response<ProjectTaskResponse>> UpdateProjectTask(int sprintId, int id, ProjectTasksRequest request, string userId)
    {
        var sprint = await GetSprint(sprintId);
        await _projectMembersService.ValidateSprintTaskPermission(userId, sprint.ProjectId);

        var task = await _dbContext.ProjectTasks
            .FirstOrDefaultAsync(x => x.Id == id && x.ProjectSprintId == sprintId);

        if (task == null)
            throw new NotFoundException("Tarefa não encontrada.");

        task.Title = request.Title;
        task.Description = request.Description;
        task.Status = request.Status;

        await _dbContext.SaveChangesAsync();

        return new Response<ProjectTaskResponse>
        {
            Dados = ToResponse(task),
            Message = "Tarefa atualizada com sucesso."
        };
    }

    public async Task<Response<bool>> DeleteProjectTask(int sprintId, int id, string userId)
    {
        var sprint = await GetSprint(sprintId);
        await _projectMembersService.ValidateSprintTaskPermission(userId, sprint.ProjectId);

        var task = await _dbContext.ProjectTasks
            .FirstOrDefaultAsync(x => x.Id == id && x.ProjectSprintId == sprintId);

        if (task == null)
            throw new KeyNotFoundException("Tarefa não encontrada.");

        _dbContext.ProjectTasks.Remove(task);
        await _dbContext.SaveChangesAsync();

        return new Response<bool>
        {
            Dados = true,
            Message = "Tarefa removida com sucesso."
        };
    }

    private async Task<ProjectSprint> GetSprint(int sprintId)
    {
        var sprint = await _dbContext.ProjectSprint.FindAsync(sprintId);

        if (sprint == null)
            throw new NotFoundException("Sprint não encontrada.");

        return sprint;
    }

    private static ProjectTaskResponse ToResponse(ProjectTasks task) => new()
    {
        Id = task.Id,
        ProjectSprintId = task.ProjectSprintId,
        Title = task.Title,
        Description = task.Description,
        Status = task.Status
    };
}