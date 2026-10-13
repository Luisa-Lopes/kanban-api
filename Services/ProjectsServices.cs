

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

    private readonly ProjectMembersService _projectsMembersService;

    public ProjectsService (AppDbContext dbContext,ProjectMembersService projectsMembersService)
    {
         _dbContext = dbContext;
         _projectsMembersService = projectsMembersService;
        
    }


    public async Task<List<ProjectsResponse>> GetProjects(string memberId)
    {

        return await _dbContext.Projects
            .Where(p => p.Members.Any(m => m.UserId == memberId))
            .Select(r => new ProjectsResponse
            {
                id = r.Id,
                Name = r.Name,
                Description = r.Description,
                StartDate = r.StartDate,
                EstimatedDate = r.EstimatedDate,
                EndDate = r.EndDate,
                progress = r.Sprints.SelectMany(s => s.Tasks).Count() == 0
                    ? 0
                    : (int)Math.Round(
                        (double)r.Sprints.SelectMany(s => s.Tasks)
                            .Count(t => t.Status == TaskStatus.Done)
                        / r.Sprints.SelectMany(s => s.Tasks).Count() * 100),
                status = r.Sprints.Any(s => s.EndDate < DateTime.UtcNow && s.Tasks.Any(t => t.Status != TaskStatus.Done))
                    ? ProjectStatus.InProgress
                    : r.Sprints.All(s => s.Tasks.All(t => t.Status == TaskStatus.Done))
                        ? ProjectStatus.Completed
                        : ProjectStatus.NotStarted,
                Tasks = r.Sprints.SelectMany(s => s.Tasks).Where(t => t.AssigneeId == memberId).Select(t => new ProjectTaskResponse
                {
                    Id = t.Id,
                    ProjectSprintId = t.ProjectSprintId,
                    Title = t.Title,
                    Description = t.Description,
                    Status = t.Status
                }).ToList()

            })
            .ToListAsync();
    }

    public async Task<Response<ProjectResponse>> GetProject(int projectId)
    {
         Response<ProjectResponse> response = new();
        try
        {
            var metrics = new ProjectMetricsResponse();

            var project = await _dbContext.Projects
                .Include(f => f.Members)
                .ThenInclude(pm => pm.User)
                .Include(p => p.Sprints)
                .ThenInclude(s => s.Tasks)
                .FirstOrDefaultAsync(p => p.Id == projectId);

            if (project != null)
            {
                response.Dados = new ProjectResponse
                {
                    id = project.Id,
                    Name = project.Name,
                    Description = project.Description,
                    StartDate = project.StartDate,
                    EstimatedDate = project.EstimatedDate,
                    EndDate = project.EndDate,
                    Members = project.Members.Select(m => new ProjectMembersResponse
                    {
                        Id = m.Id,
                        ProjectId = m.ProjectId,
                        User = new UserResponse
                        {
                            Id = m.User.Id,
                            FirstName = m.User.FirstName,
                            LastName = m.User.LastName,
                            Email = m.User.Email!
                        },
                        JoinedAt = m.JoinedAt,
                        Role = m.Role
                    }).ToList(),
                    Sprints = project.Sprints.Select(s =>
                    {
                        if (s.StartDate < DateTime.Now && s.EndDate < DateTime.Now &&
                            s.Tasks.Any(t => t.Status != TaskStatus.Done))
                        {
                            metrics.overdueTasks++;
                        }

                        return new ProjectSprintResponse
                        {
                            id = s.Id,
                            ProjectId = s.ProjectId,
                            Title = s.Title,
                            Description = s.Description,
                            StartDate = s.StartDate,
                            EndDate = s.EndDate,
                            Tasks = s.Tasks.Select(t =>
                            {
                                metrics.totalTasks++;
                                if (t.Status == TaskStatus.Done)
                                    metrics.completedTasks++;
                                else if (t.Status == TaskStatus.InProgress)
                                    metrics.inProgressTasks++;

                                return new ProjectTaskResponse
                                {
                                    Id = t.Id,
                                    ProjectSprintId = t.ProjectSprintId,
                                    Title = t.Title,
                                    Description = t.Description,
                                    Status = t.Status,
                                    AssigneeId = t.AssigneeId
                                };
                            }).ToList()
                        };
                    }).ToList(),
                    Metrics = metrics
                };
            }
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

    public async Task<Response<ProjectsResumedResponse>> CreateProject(
        ProjectsRequest request, string userEmail)
    {

        Response<ProjectsResumedResponse> response = new();

        try
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

                var projectMember = new ProjectMembersRequest
                {
                    ProjectId = project.Id,
                    Email = userEmail,
                    Role = ProjectRole.Owner
                };

               await _projectsMembersService.CreateProjectOwner(projectMember);


                response.Dados =  new ProjectsResumedResponse
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
        catch (Exception ex)
        {
            response.Status = false;
            response.StatusCode = StatusCodes.Status417ExpectationFailed;
            response.Message = ex.InnerException?.Message ?? ex.Message;

            return response;
        }
    }

    public async Task<Response<ProjectsResumedResponse>> UpdateProject(int id, ProjectsRequest request, string userId)
    {
       
        Response<ProjectsResumedResponse> response = new ();

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
        response.Dados =  new ProjectsResumedResponse
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