using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Response.Models;
using Service;

namespace Function.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class NotificationsController : ControllerBase
{
    private readonly NotificationsService _notificationsService;

    public NotificationsController(NotificationsService notificationsService)
    {
        _notificationsService = notificationsService;
    }

    [HttpGet]
    public async Task<ActionResult<Response<List<NotificationResponse>>>> GetNotifications()
    {
        var userEmail = User.FindFirstValue(ClaimTypes.Email);
        if (string.IsNullOrWhiteSpace(userEmail))
        {
            return Unauthorized(new Response<List<NotificationResponse>>
            {
                Status = false,
                StatusCode = StatusCodes.Status401Unauthorized,
                Message = "E-mail do usuário não encontrado."
            });
        }

        var response = await _notificationsService.GetNotifications(userEmail);
        return Ok(response);
    }
}
