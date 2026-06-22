
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
    [HttpGet]
    public async Task<ActionResult<List<ProjectsResponse>>> GetProjects()
    {
        List<ProjectsResponse> projects = await _projectsService.GetProjects();
        return Ok(projects);
    }
    #endregion

     #region CreateProject
    [HttpPost]
    public async Task<IActionResult> CreateProject(
        ProjectsRequest request)
    {
        ProjectsResponse project = await _projectsService.CreateProject(request);

        return new OkObjectResult(project);
    }
    #endregion

    #region UpdateProject
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateProject(int id, ProjectsRequest request)
    {
        if (request.id != 0 && request.id != id)
        {
            return BadRequest("O id do corpo deve corresponder ao id da rota.");
        }

        var update = await _projectsService.UpdateProject(id, request);
        if (update is null)
        {
            return NotFound();
        }

        return Ok(update);
    }
    #endregion

    #region DeleteProject
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProject(int id)
    {
        bool deleted = await _projectsService.DeleteProject(id);
        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }
    #endregion

}