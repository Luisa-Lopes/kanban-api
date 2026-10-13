using System.ComponentModel;
using System.Security.Claims;
using Response.Models;
using Service;

namespace AgentTools;

public class ProjectTools
{
    private readonly ProjectsService _projectsService;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ProjectTools(
        ProjectsService projectsService,
        IHttpContextAccessor httpContextAccessor)
    {
        _projectsService = projectsService;
        _httpContextAccessor = httpContextAccessor;
    }

    [Description("Obtém a lista de projetos do usuário autenticado.")]
    public async Task<List<ProjectsResponse>> GetProjects()
    {
        var userId = _httpContextAccessor.HttpContext?
            .User
            .FindFirstValue(ClaimTypes.NameIdentifier);

        if (userId == null)
            throw new UnauthorizedAccessException();

        return await _projectsService.GetProjects(userId);
    }
}