using Microsoft.AspNetCore.SignalR;
using McpLampada.Hubs;
using McpLampada.Models;

namespace McpLampada.Services;

public class LampadinaNotificationService : BackgroundService
{
    private readonly IHubContext<LampadinaHub> _hubContext;
    private readonly ILampadinaService _lampadinaService;
    private readonly ILogger<LampadinaNotificationService> _logger;

    public LampadinaNotificationService(
        IHubContext<LampadinaHub> hubContext,
        ILampadinaService lampadinaService,
        ILogger<LampadinaNotificationService> logger)
    {
        _hubContext = hubContext;
        _lampadinaService = lampadinaService;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Sottoscrivi agli eventi di cambiamento stato
        _lampadinaService.StatoCambiato += OnStatoCambiato;

        // Mantieni il servizio in esecuzione
        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(1000, stoppingToken);
        }
    }

    private async void OnStatoCambiato(LampadinaState nuovoStato)
    {
        try
        {
            _logger.LogInformation($"Stato lampadina cambiato: {nuovoStato.GetStatusText()} - {nuovoStato.Colore}");
            await _hubContext.Clients.All.SendAsync("StatoAggiornato", nuovoStato);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Errore nell'invio della notifica di cambio stato");
        }
    }

    public override void Dispose()
    {
        _lampadinaService.StatoCambiato -= OnStatoCambiato;
        base.Dispose();
    }
}