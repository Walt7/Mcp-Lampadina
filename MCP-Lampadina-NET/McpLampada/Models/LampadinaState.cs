namespace McpLampada.Models;

public class LampadinaState
{
    public bool Accesa { get; set; } = false;
    public string Colore { get; set; } = "#ffffff";
    public int Luminosita { get; set; } = 100;

    public string GetStatusEmoji() => Accesa ? "🟢" : "🔴";
    public string GetStatusText() => Accesa ? "Accesa" : "Spenta";
}