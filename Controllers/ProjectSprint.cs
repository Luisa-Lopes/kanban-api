using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Request.Models;
using Response.Models;
using Service;

namespace Function.Controllers;

[ApiController]
[Route("api/sprints")]
public class ProjectSprintController : ControllerBase
{
    private readonly ProjectSprintService _projectSprintService;

    public ProjectSprintController(ProjectSprintService projectSprintService)
    {
        _projectSprintService = projectSprintService;
    }

    [Authorize]
    [HttpPost("{projectId}")]
    public async Task<IActionResult> CreateProjectSprint(int projectId, ProjectSprintRequest request)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null)
            return Unauthorized(new Response<ProjectSprintResponse> { Message = "Usuário não encontrado.", Status = false });

        var response = await _projectSprintService.CreateProjectSprint(projectId, request, userId);
        return response.Status ? Ok(response) : StatusCode(response.StatusCode, response);
    }

    [Authorize]
    [HttpPut("{projectId}/{id}")]
    public async Task<IActionResult> UpdateProjectSprint(int projectId, int id, ProjectSprintRequest request)
    {
        if (request.id != 0 && request.id != id)
            return BadRequest("O id do corpo deve corresponder ao id da rota.");

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null)
            return Unauthorized(new Response<ProjectSprintResponse> { Message = "Usuário não encontrado.", Status = false });

        var response = await _projectSprintService.UpdateProjectSprint(projectId, id, request, userId);
        return response.Status ? Ok(response) : StatusCode(response.StatusCode, response);
    }

    [Authorize]
    [HttpDelete("{projectId}/{id}")]
    public async Task<IActionResult> DeleteProjectSprint(int projectId, int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null)
            return Unauthorized(new Response<bool> { Message = "Usuário não encontrado.", Status = false });

        var response = await _projectSprintService.DeleteProjectSprint(projectId, id, userId);
        return response.Status ? Ok(response) : StatusCode(response.StatusCode, response);
    }
}