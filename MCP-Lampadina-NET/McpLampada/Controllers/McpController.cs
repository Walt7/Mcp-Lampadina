using Microsoft.AspNetCore.Mvc;
using McpLampada.Services;
using System.Text;

namespace McpLampada.Controllers;

[ApiController]
[Route("mcp")]
public class McpController : ControllerBase
{
    private readonly McpServer _mcpServer;
    private readonly ILogger<McpController> _logger;

    public McpController(McpServer mcpServer, ILogger<McpController> logger)
    {
        _mcpServer = mcpServer;
        _logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> HandleMcpRequest()
    {
        try
        {
            using var reader = new StreamReader(Request.Body, Encoding.UTF8);
            var jsonRequest = await reader.ReadToEndAsync();

            _logger.LogInformation($"Ricevuta richiesta MCP: {jsonRequest}");

            var response = await _mcpServer.ProcessRequest(jsonRequest);

            _logger.LogInformation($"Risposta MCP: {response}");

            return Content(response, "application/json");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Errore nell'elaborazione della richiesta MCP");
            return StatusCode(500, new { error = "Errore interno del server" });
        }
    }

    [HttpOptions]
    public IActionResult HandlePreflight()
    {
        return Ok();
    }
}