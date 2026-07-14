
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Request.Models;
using Response.Models;
using Service;

namespace Function.Controllers;

[ApiController]
[Route("api/projects/invitations")]

public class ProjectInvitationsController: ControllerBase
{
    private readonly ProjectInvitationsService _projectInvitationsService;

    public ProjectInvitationsController(ProjectInvitationsService projectInvitationsService)
    {
        _projectInvitationsService = projectInvitationsService;
    }


    #region CreateProjectInvitation
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> CreateProjectInvitation(
        ProjectInvitationsRequest request)
    {
        
        Response<ProjectInvitationResponse> response = new();
        
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if(userId == null)
        {
            response.Dados = null;
            response.Message = "Usuário não encontrado.";
            response.Status = false;
           
            return Unauthorized(response);
        }

        var projectInvitation = await _projectInvitationsService.CreateProjectInvitation(request);

        return Ok( projectInvitation);
    }
    #endregion

}