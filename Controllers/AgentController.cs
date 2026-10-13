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
    public async Task<IActionResult> Chat([FromBody] ChatRequest request)
    {
         if (string.IsNullOrWhiteSpace(request.Message))
        {
            return BadRequest("A mensagem não pode ser vazia.");
        }

        var response = await _agentService.SendMessageAsync(request.Message);

        return Ok(response);
    }
}