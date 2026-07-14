
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Request.Models;
using Response.Models;
using Service;

namespace Function.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProjectsController: ControllerBase
{
    private readonly ProjectsService _projectsService;

    public ProjectsController(ProjectsService projectService)
    {
        _projectsService = projectService;
    }


    #region GetProjects
    [Authorize]
    [HttpGet]
    public async Task<ActionResult<Response<List<ProjectsResponse>>>> GetProjects()
    {

        Response<List<ProjectsResponse>> response = new Response<List<ProjectsResponse>>();

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

             if(userId == null)
        {
            response.Dados = null;
            response.Message = "Usuário não encontrado.";
            response.Status = false;
           
            return Unauthorized(response);
        }


        List<ProjectsResponse> projects = await _projectsService.GetProjects(userId);
        return Ok(projects);
    }
    #endregion

    #region CreateProject
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> CreateProject(
        ProjectsRequest request)
    {
        ProjectsResponse project = await _projectsService.CreateProject(request);

        return new OkObjectResult(project);
    }
    #endregion

    #region UpdateProject
    [Authorize]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateProject(int id, ProjectsRequest request)
    {
        if (request.Id != 0 && request.Id != id)
        {
            return BadRequest("O id do corpo deve corresponder ao id da rota.");
        }

        Response<List<ProjectsResponse>> response = new();

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if(userId == null)
        {
            response.Dados = null;
            response.Message = "Usuário não encontrado.";
            response.Status = false;
           
            return Unauthorized(response);
        }

        var update = await _projectsService.UpdateProject(id, request, userId);
        if (update is null)
        {
            return NotFound();
        }

        return Ok(update);
    }
    #endregion

    #region DeleteProject
    [Authorize]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProject(int id)
    {
        Response<List<ProjectsResponse>> response = new Response<List<ProjectsResponse>>();

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if(userId == null)
        {
            response.Dados = null;
            response.Message = "Usuário não encontrado.";
            response.Status = false;
           
            return Unauthorized(response);
        }

        var deleted = await _projectsService.DeleteProject(id, userId);

        if (!deleted.Status)
        {
            return BadRequest(deleted);
        }

        return Ok(deleted);
    }
    #endregion

}