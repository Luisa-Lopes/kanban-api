using System.ComponentModel;
using System.Security.Claims;
using Response.Models;
using Service;

namespace AgentTools;

public class ProjectSprintTools
{
    private readonly ProjectSprintService _projectSprintService;
    private readonly ProjectsService _projectsService;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ProjectSprintTools(
        ProjectSprintService projectSprintService,
        ProjectsService projectsService,
        IHttpContextAccessor httpContextAccessor)
    {
        _projectSprintService = projectSprintService;
        _projectsService = projectsService;
        _httpContextAccessor = httpContextAccessor;
    }

    [Description("Obtém a lista de sprints de um projeto pelo nome. " +
        "Use quando o usuário perguntar quais sprints existem em um projeto.")]
    public async Task<Response<List<ProjectSprintResponse>>> GetProjectSprints(
        [Description("Nome do projeto informado pelo usuário")]
        string projectName)
    {
        var userId = _httpContextAccessor.HttpContext?
            .User
            .FindFirstValue(ClaimTypes.NameIdentifier);

        if (userId == null)
            throw new UnauthorizedAccessException();

        var projects = await _projectsService.GetProjects(userId);

        var matchingProjects = projects
            .Where(project => project.Name.Contains(
                projectName,
                StringComparison.OrdinalIgnoreCase))
            .ToList();

        if (matchingProjects.Count == 0)
            throw new InvalidOperationException(
                $"Nenhum projeto encontrado com o nome '{projectName}'.");

        if (matchingProjects.Count > 1)
            throw new InvalidOperationException(
                "Existem vários projetos com esse nome. " +
                "Peça ao usuário para especificar qual deles deseja consultar.");

        var projectId = matchingProjects[0].id;

        return await _projectSprintService.GetProjectSprints(projectId, userId);
    }
}
