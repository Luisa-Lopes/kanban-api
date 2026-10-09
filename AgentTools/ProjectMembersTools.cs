using System.ComponentModel;
using System.Security.Claims;
using Response.Models;
using Service;
using Tables.Models;

namespace AgentTools;

public class ProjectMembersTools
{
    private readonly ProjectMembersService _projectMembersService;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ProjectsService _projectsService;

    public ProjectMembersTools(
        ProjectsService projectsService,
        ProjectMembersService projectMembersService,
        IHttpContextAccessor httpContextAccessor)
    {
         _projectsService = projectsService;
        _projectMembersService = projectMembersService;
        _httpContextAccessor = httpContextAccessor;
    }

     [Description("Obtém os membros de um projeto pelo nome. " +
        "Use quando o usuário perguntar quem são os membros de um projeto.")]
    public async Task<Response<List<ProjectMembersResponse>>> GetProjectMembersByName(
        [Description("Nome do projeto informado pelo usuário")]
        string projectName)
    {
        var userId = _httpContextAccessor.HttpContext?
            .User
            .FindFirstValue(ClaimTypes.NameIdentifier);

        if (userId == null)
            throw new UnauthorizedAccessException();

        // Busca somente os projetos acessíveis ao usuário.
        var projects = await _projectsService.GetProjects(userId);

        var matchingProjects = projects
            .Where(p => p.Name.Contains(
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

        // Busca os membros usando o ID encontrado.
        return await _projectMembersService.GetProjectMembers(
            projectId, userId);
    }
}