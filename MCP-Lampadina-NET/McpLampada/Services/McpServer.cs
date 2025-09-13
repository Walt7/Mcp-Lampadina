using System.Text.Json;
using System.Text.Json.Nodes;
using McpLampada.Models;

namespace McpLampada.Services;

public class McpServer
{
    private readonly ILampadinaService _lampadinaService;
    private readonly ILogger<McpServer> _logger;

    public McpServer(ILampadinaService lampadinaService, ILogger<McpServer> logger)
    {
        _lampadinaService = lampadinaService;
        _logger = logger;
    }

    public async Task<string> ProcessRequest(string jsonRequest)
    {
        try
        {
            var request = JsonNode.Parse(jsonRequest);
            var method = request?["method"]?.ToString();
            var id = request?["id"];

            _logger.LogInformation($"Richiesta MCP: {method}");

            return method switch
            {
                "initialize" => HandleInitialize(id),
                "tools/list" => HandleToolsList(id),
                "tools/call" => await HandleToolCall(request, id),
                _ => CreateErrorResponse(id, -32601, "Metodo non trovato")
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Errore nell'elaborazione della richiesta MCP");
            return CreateErrorResponse(null, -32603, "Errore interno del server");
        }
    }

    private string HandleInitialize(JsonNode? id)
    {
        var response = new
        {
            jsonrpc = "2.0",
            id = id?.ToString(),
            result = new
            {
                protocolVersion = "2024-11-05",
                capabilities = new
                {
                    tools = new { }
                },
                serverInfo = new
                {
                    name = "mcp-lampadina-net",
                    version = "1.0.0"
                }
            }
        };
        return JsonSerializer.Serialize(response);
    }

    private string HandleToolsList(JsonNode? id)
    {
        var tools = new object[]
        {
            new
            {
                name = "lampadina_stato",
                description = "Ottiene lo stato attuale della lampadina (accesa/spenta, colore, luminosità)",
                inputSchema = new { type = "object", properties = new { } }
            },
            new
            {
                name = "lampadina_toggle",
                description = "Accende o spegne la lampadina",
                inputSchema = new { type = "object", properties = new { } }
            },
            new
            {
                name = "lampadina_colore",
                description = "Cambia il colore della lampadina",
                inputSchema = new
                {
                    type = "object",
                    properties = new
                    {
                        colore = new
                        {
                            type = "string",
                            pattern = "^#[0-9a-fA-F]{6}$",
                            description = "Il colore in formato esadecimale (es: #ff0000 per rosso, #00ff00 per verde)"
                        }
                    },
                    required = new[] { "colore" }
                }
            },
            new
            {
                name = "lampadina_luminosita",
                description = "Regola la luminosità della lampadina",
                inputSchema = new
                {
                    type = "object",
                    properties = new
                    {
                        luminosita = new
                        {
                            type = "number",
                            minimum = 0,
                            maximum = 100,
                            description = "Livello di luminosità da 0 a 100"
                        }
                    },
                    required = new[] { "luminosita" }
                }
            },
            new
            {
                name = "lampadina_preset",
                description = "Applica un preset di colore alla lampadina",
                inputSchema = new
                {
                    type = "object",
                    properties = new
                    {
                        preset = new
                        {
                            type = "string",
                            @enum = new[] { "bianco", "rosso", "verde", "blu", "giallo", "magenta", "ciano", "arancione" },
                            description = "Nome del preset colore"
                        }
                    },
                    required = new[] { "preset" }
                }
            }
        };

        var response = new
        {
            jsonrpc = "2.0",
            id = id?.ToString(),
            result = new { tools }
        };

        return JsonSerializer.Serialize(response);
    }

    private Task<string> HandleToolCall(JsonNode? request, JsonNode? id)
    {
        try
        {
            var toolName = request?["params"]?["name"]?.ToString();
            var arguments = request?["params"]?["arguments"];

            var result = toolName switch
            {
                "lampadina_stato" => HandleLampadinaStato(),
                "lampadina_toggle" => HandleLampadinaToggle(),
                "lampadina_colore" => HandleLampadinaColore(arguments),
                "lampadina_luminosita" => HandleLampadinaLuminosita(arguments),
                "lampadina_preset" => HandleLampadinaPreset(arguments),
                _ => throw new ArgumentException($"Strumento sconosciuto: {toolName}")
            };

            var response = new
            {
                jsonrpc = "2.0",
                id = id?.ToString(),
                result = new
                {
                    content = new[]
                    {
                        new { type = "text", text = result }
                    }
                }
            };

            return Task.FromResult(JsonSerializer.Serialize(response));
        }
        catch (Exception ex)
        {
            return Task.FromResult(CreateErrorResponse(id, -32603, $"Errore nell'esecuzione dello strumento: {ex.Message}"));
        }
    }

    private string HandleLampadinaStato()
    {
        var stato = _lampadinaService.GetStato();
        return $"🔌 Stato della lampadina:\n" +
               $"• Stato: {stato.GetStatusEmoji()} {stato.GetStatusText()}\n" +
               $"• Colore: {stato.Colore}\n" +
               $"• Luminosità: {stato.Luminosita}%\n\n" +
               $"La lampadina è {stato.GetStatusText().ToLower()} con colore {stato.Colore} e luminosità al {stato.Luminosita}%.";
    }

    private string HandleLampadinaToggle()
    {
        var statoIniziale = _lampadinaService.GetStato().Accesa;
        _lampadinaService.Toggle();
        var nuovoStato = _lampadinaService.GetStato();

        var azione = nuovoStato.Accesa ? "accesa" : "spenta";
        return $"✅ Lampadina {azione}\n\n" +
               $"🔌 Stato aggiornato:\n" +
               $"• Stato: {nuovoStato.GetStatusEmoji()} {nuovoStato.GetStatusText()}\n" +
               $"• Colore: {nuovoStato.Colore}\n" +
               $"• Luminosità: {nuovoStato.Luminosita}%";
    }

    private string HandleLampadinaColore(JsonNode? arguments)
    {
        var colore = arguments?["colore"]?.ToString();
        if (string.IsNullOrEmpty(colore))
            throw new ArgumentException("Parametro 'colore' mancante");

        _lampadinaService.CambiaColore(colore);
        var stato = _lampadinaService.GetStato();

        return $"🎨 Colore cambiato a {colore}\n\n" +
               $"🔌 Stato aggiornato:\n" +
               $"• Stato: {stato.GetStatusEmoji()} {stato.GetStatusText()}\n" +
               $"• Colore: {stato.Colore}\n" +
               $"• Luminosità: {stato.Luminosita}%";
    }

    private string HandleLampadinaLuminosita(JsonNode? arguments)
    {
        var luminositaJson = arguments?["luminosita"];
        if (luminositaJson == null)
            throw new ArgumentException("Parametro 'luminosita' mancante");

        var luminosita = (int)luminositaJson.GetValue<double>();
        _lampadinaService.RegolareLuminosita(luminosita);
        var stato = _lampadinaService.GetStato();

        return $"💡 Luminosità regolata al {luminosita}%\n\n" +
               $"🔌 Stato aggiornato:\n" +
               $"• Stato: {stato.GetStatusEmoji()} {stato.GetStatusText()}\n" +
               $"• Colore: {stato.Colore}\n" +
               $"• Luminosità: {stato.Luminosita}%";
    }

    private string HandleLampadinaPreset(JsonNode? arguments)
    {
        var preset = arguments?["preset"]?.ToString();
        if (string.IsNullOrEmpty(preset))
            throw new ArgumentException("Parametro 'preset' mancante");

        _lampadinaService.ApplicaPreset(preset);
        var stato = _lampadinaService.GetStato();

        return $"🎯 Preset \"{preset}\" applicato ({stato.Colore})\n\n" +
               $"🔌 Stato aggiornato:\n" +
               $"• Stato: {stato.GetStatusEmoji()} {stato.GetStatusText()}\n" +
               $"• Colore: {stato.Colore}\n" +
               $"• Luminosità: {stato.Luminosita}%";
    }

    private string CreateErrorResponse(JsonNode? id, int code, string message)
    {
        var response = new
        {
            jsonrpc = "2.0",
            id = id?.ToString(),
            error = new
            {
                code,
                message
            }
        };
        return JsonSerializer.Serialize(response);
    }
}