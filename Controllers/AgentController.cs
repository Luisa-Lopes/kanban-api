using Microsoft.AspNetCore.Mvc;

namespace Function.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AgentController : ControllerBase
{
    private readonly IAgentService _agentService;

    public AgentController(IAgentService agentService)
    {
        _agentService = agentService;
    }

    [HttpPost("chat")]
    public async Task<IActionResult> Chat([FromBody] string message)
    {
        var response = await _agentService.SendMessageAsync(message);

        return Ok(response);
    }
}