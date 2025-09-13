using Microsoft.AspNetCore.SignalR;
using McpLampada.Services;

namespace McpLampada.Hubs;

public class LampadinaHub : Hub
{
    private readonly ILampadinaService _lampadinaService;

    public LampadinaHub(ILampadinaService lampadinaService)
    {
        _lampadinaService = lampadinaService;
    }

    public override async Task OnConnectedAsync()
    {
        // Invia lo stato attuale quando un client si connette
        var stato = _lampadinaService.GetStato();
        await Clients.Caller.SendAsync("StatoAggiornato", stato);
        await base.OnConnectedAsync();
    }

    // Metodi per ricevere comandi dal client
    public Task Toggle()
    {
        _lampadinaService.Toggle();
        return Task.CompletedTask;
    }

    public async Task CambiaColore(string colore)
    {
        try
        {
            _lampadinaService.CambiaColore(colore);
        }
        catch (Exception ex)
        {
            await Clients.Caller.SendAsync("Errore", ex.Message);
        }
    }

    public async Task RegolareLuminosita(int luminosita)
    {
        try
        {
            _lampadinaService.RegolareLuminosita(luminosita);
        }
        catch (Exception ex)
        {
            await Clients.Caller.SendAsync("Errore", ex.Message);
        }
    }

    public async Task ApplicaPreset(string preset)
    {
        try
        {
            _lampadinaService.ApplicaPreset(preset);
        }
        catch (Exception ex)
        {
            await Clients.Caller.SendAsync("Errore", ex.Message);
        }
    }
}