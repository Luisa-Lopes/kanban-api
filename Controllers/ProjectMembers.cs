
using System.Reflection.Metadata.Ecma335;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Request.Models;
using Response.Models;
using Service;

namespace Function.Controllers;

[ApiController]
[Route("api/project/{projectId}/members")]

public class ProjectMembersController: ControllerBase
{
    private readonly ProjectMembersService _projectsMembersService;

    public ProjectMembersController(ProjectMembersService projectsMembersService)
    {
        _projectsMembersService = projectsMembersService;
    }


    #region GetProjectMembers
    
    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetProjectMembers(int projectId)
    {
        Response<List<ProjectMembersResponse>> response = new();

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if(userId == null)
        {
            response.Dados = null;
            response.Message = "Usuário não encontrado.";
            response.Status = false;
           
            return Unauthorized(response);
        }

        response =
            await _projectsMembersService.GetProjectMembers(
                projectId,
                userId
            );

        return response.Status
            ? Ok(response)
            : StatusCode(response.StatusCode, response);
    }
    #endregion

}