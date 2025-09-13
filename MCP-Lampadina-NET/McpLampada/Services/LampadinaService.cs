using McpLampada.Models;

namespace McpLampada.Services;

public interface ILampadinaService
{
    LampadinaState GetStato();
    void Toggle();
    void CambiaColore(string colore);
    void RegolareLuminosita(int luminosita);
    void ApplicaPreset(string preset);
    event Action<LampadinaState>? StatoCambiato;
}

public class LampadinaService : ILampadinaService
{
    private readonly LampadinaState _stato = new();
    private readonly Dictionary<string, string> _presets = new()
    {
        ["bianco"] = "#ffffff",
        ["rosso"] = "#ff0000",
        ["verde"] = "#00ff00",
        ["blu"] = "#0000ff",
        ["giallo"] = "#ffff00",
        ["magenta"] = "#ff00ff",
        ["ciano"] = "#00ffff",
        ["arancione"] = "#ffa500"
    };

    public event Action<LampadinaState>? StatoCambiato;

    public LampadinaState GetStato() => _stato;

    public void Toggle()
    {
        _stato.Accesa = !_stato.Accesa;
        NotificaCambiamento();
    }

    public void CambiaColore(string colore)
    {
        if (IsValidHexColor(colore))
        {
            _stato.Colore = colore.ToLowerInvariant();
            NotificaCambiamento();
        }
        else
        {
            throw new ArgumentException($"Colore non valido: {colore}");
        }
    }

    public void RegolareLuminosita(int luminosita)
    {
        if (luminosita >= 0 && luminosita <= 100)
        {
            _stato.Luminosita = luminosita;
            NotificaCambiamento();
        }
        else
        {
            throw new ArgumentOutOfRangeException(nameof(luminosita), "La luminosità deve essere tra 0 e 100");
        }
    }

    public void ApplicaPreset(string preset)
    {
        var presetLower = preset.ToLowerInvariant();
        if (_presets.TryGetValue(presetLower, out var colore))
        {
            _stato.Colore = colore;
            NotificaCambiamento();
        }
        else
        {
            throw new ArgumentException($"Preset non esistente: {preset}. Disponibili: {string.Join(", ", _presets.Keys)}");
        }
    }

    private void NotificaCambiamento()
    {
        StatoCambiato?.Invoke(_stato);
    }

    private static bool IsValidHexColor(string colore)
    {
        return colore.Length == 7 &&
               colore[0] == '#' &&
               colore.Skip(1).All(c => char.IsAsciiHexDigit(c));
    }
}