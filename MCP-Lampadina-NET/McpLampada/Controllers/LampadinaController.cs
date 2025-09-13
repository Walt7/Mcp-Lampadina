using Microsoft.AspNetCore.Mvc;
using McpLampada.Services;
using McpLampada.Models;

namespace McpLampada.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LampadinaController : ControllerBase
{
    private readonly ILampadinaService _lampadinaService;

    public LampadinaController(ILampadinaService lampadinaService)
    {
        _lampadinaService = lampadinaService;
    }

    [HttpGet("stato")]
    public ActionResult<LampadinaState> GetStato()
    {
        return Ok(_lampadinaService.GetStato());
    }

    [HttpPost("toggle")]
    public ActionResult<LampadinaState> Toggle()
    {
        _lampadinaService.Toggle();
        return Ok(_lampadinaService.GetStato());
    }

    [HttpPost("colore")]
    public ActionResult<LampadinaState> CambiaColore([FromBody] ColoreRequest request)
    {
        try
        {
            _lampadinaService.CambiaColore(request.Colore);
            return Ok(_lampadinaService.GetStato());
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPost("luminosita")]
    public ActionResult<LampadinaState> RegolareLuminosita([FromBody] LuminositaRequest request)
    {
        try
        {
            _lampadinaService.RegolareLuminosita(request.Luminosita);
            return Ok(_lampadinaService.GetStato());
        }
        catch (ArgumentOutOfRangeException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPost("preset")]
    public ActionResult<LampadinaState> ApplicaPreset([FromBody] PresetRequest request)
    {
        try
        {
            _lampadinaService.ApplicaPreset(request.Preset);
            return Ok(_lampadinaService.GetStato());
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}

public record ColoreRequest(string Colore);
public record LuminositaRequest(int Luminosita);
public record PresetRequest(string Preset);