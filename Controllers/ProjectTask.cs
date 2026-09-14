using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Request.Models;
using Response.Models;
using Service;

namespace Function.Controllers;

[ApiController]
[Route("api/sprint/{sprintId}/tasks")]
public class ProjectTaskController : ControllerBase
{
    private readonly ProjectTaskService _projectTaskService;

    public ProjectTaskController(ProjectTaskService projectTaskService)
    {
        _projectTaskService = projectTaskService;
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetProjectTasks(int sprintId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null)
            return Unauthorized(new Response<List<ProjectTaskResponse>> { Message = "Usuário não encontrado.", Status = false });

        var response = await _projectTaskService.GetProjectTasks(sprintId, userId);
        return response.Status ? Ok(response) : StatusCode(response.StatusCode, response);
    }

    [Authorize]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetProjectTask(int sprintId, int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null)
            return Unauthorized(new Response<ProjectTaskResponse> { Message = "Usuário não encontrado.", Status = false });

        var response = await _projectTaskService.GetProjectTask(sprintId, id, userId);
        return response.Status ? Ok(response) : StatusCode(response.StatusCode, response);
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> CreateProjectTask(int sprintId, ProjectTasksRequest request)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null)
            return Unauthorized(new Response<ProjectTaskResponse> { Message = "Usuário não encontrado.", Status = false });

        var response = await _projectTaskService.CreateProjectTask(sprintId, request, userId);
        return response.Status ? Ok(response) : StatusCode(response.StatusCode, response);
    }

    [Authorize]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateProjectTask(int sprintId, int id, ProjectTasksRequest request)
    {
        if (request.Id != 0 && request.Id != id)
            return BadRequest("O id do corpo deve corresponder ao id da rota.");

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null)
            return Unauthorized(new Response<ProjectTaskResponse> { Message = "Usuário não encontrado.", Status = false });

        var response = await _projectTaskService.UpdateProjectTask(sprintId, id, request, userId);
        return response.Status ? Ok(response) : StatusCode(response.StatusCode, response);
    }

    [Authorize]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProjectTask(int sprintId, int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null)
            return Unauthorized(new Response<bool> { Message = "Usuário não encontrado.", Status = false });

        var response = await _projectTaskService.DeleteProjectTask(sprintId, id, userId);
        return response.Status ? Ok(response) : StatusCode(response.StatusCode, response);
    }
}