
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectManager.Exceptions;
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

        var projectInvitation = await _projectInvitationsService.CreateProjectInvitation(request, userId);

         return projectInvitation.Status ? Ok(projectInvitation) : StatusCode(response.StatusCode,projectInvitation);
    }
    #endregion

    [Authorize]
    [HttpPut("{invitationId:int}")]
    public async Task<IActionResult> RespondToProjectInvitation(
        int invitationId,
        RespondToProjectInvitationRequest request)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var userEmail = User.FindFirstValue(ClaimTypes.Email);

        if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(userEmail))
        {
            return Unauthorized(new Response<ProjectInvitationResponse>
            {
                Status = false,
                StatusCode = StatusCodes.Status401Unauthorized,
                Message = "Usuário não encontrado."
            });
        }

        if (request.Status is not InvitationStatus.Accepted and not InvitationStatus.Cancelled)
        {
            throw new BadRequestException("O status deve ser Accepted para aceitar ou Cancelled para recusar o convite.");
        }

        var response = await _projectInvitationsService.RespondToInvitation(
            invitationId,
            userId,
            userEmail,
            request.Status);

        return Ok(response);
    }

}
