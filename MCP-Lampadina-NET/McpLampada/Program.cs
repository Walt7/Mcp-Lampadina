using McpLampada.Services;
using McpLampada.Hubs;

var builder = WebApplication.CreateBuilder(args);

// Servizi
builder.Services.AddControllers();
builder.Services.AddSignalR();
builder.Services.AddSingleton<ILampadinaService, LampadinaService>();
builder.Services.AddSingleton<McpServer>();

// CORS per permettere connessioni da qualsiasi origine
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

// Configura il servizio per notificare i client via SignalR
builder.Services.AddHostedService<LampadinaNotificationService>();

var app = builder.Build();

// Configura pipeline
app.UseCors();
app.UseStaticFiles();
app.UseRouting();

app.MapControllers();
app.MapHub<LampadinaHub>("/lampada-hub");

// Serve la pagina principale
app.MapGet("/", () => Results.Content(GetIndexHtml(), "text/html"));

app.Run();

static string GetIndexHtml()
{
    return """
<!DOCTYPE html>
<html lang="it">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Controllo Lampadina MCP - .NET</title>
    <style>
        body {
            font-family: Arial, sans-serif;
            max-width: 800px;
            margin: 50px auto;
            padding: 20px;
            background: #f0f0f0;
        }
        .container {
            background: white;
            padding: 30px;
            border-radius: 10px;
            box-shadow: 0 4px 6px rgba(0,0,0,0.1);
        }
        .lampada {
            text-align: center;
            margin: 30px 0;
        }
        .bulb {
            width: 150px;
            height: 150px;
            border-radius: 50%;
            margin: 0 auto 20px;
            border: 3px solid #333;
            transition: all 0.3s ease;
            display: flex;
            align-items: center;
            justify-content: center;
            font-size: 48px;
        }
        .controlli {
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
            gap: 20px;
            margin: 30px 0;
        }
        button, select, input[type="range"] {
            padding: 10px;
            border: 1px solid #ddd;
            border-radius: 5px;
            font-size: 16px;
        }
        button {
            background: #007cba;
            color: white;
            cursor: pointer;
            transition: background 0.3s;
        }
        button:hover { background: #005a87; }
        .stato {
            background: #f8f9fa;
            padding: 15px;
            border-radius: 5px;
            margin: 20px 0;
        }
        .preset-buttons {
            display: grid;
            grid-template-columns: repeat(4, 1fr);
            gap: 10px;
            margin: 20px 0;
        }
        .preset-btn {
            padding: 8px;
            font-size: 14px;
        }
    </style>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/microsoft-signalr/8.0.0/signalr.min.js"></script>
</head>
<body>
    <div class="container">
        <h1>🔌 Controllo Lampadina MCP (.NET)</h1>

        <div class="lampada">
            <div id="bulb" class="bulb">💡</div>
            <div id="status">Connessione in corso...</div>
        </div>

        <div class="stato">
            <h3>Stato Attuale:</h3>
            <div id="statoDettagli">Caricamento...</div>
        </div>

        <div class="controlli">
            <button onclick="toggle()">Accendi/Spegni</button>

            <div>
                <input type="color" id="colorePicker" onchange="cambiaColore(this.value)">
                <label for="colorePicker">Scegli Colore</label>
            </div>

            <div>
                <input type="range" id="luminosita" min="0" max="100" value="100"
                       oninput="regolareLuminosita(this.value)">
                <label for="luminosita">Luminosità: <span id="luminositaValue">100</span>%</label>
            </div>
        </div>

        <div class="preset-buttons">
            <button class="preset-btn" onclick="applicaPreset('bianco')">⚪ Bianco</button>
            <button class="preset-btn" onclick="applicaPreset('rosso')">🔴 Rosso</button>
            <button class="preset-btn" onclick="applicaPreset('verde')">🟢 Verde</button>
            <button class="preset-btn" onclick="applicaPreset('blu')">🔵 Blu</button>
            <button class="preset-btn" onclick="applicaPreset('giallo')">🟡 Giallo</button>
            <button class="preset-btn" onclick="applicaPreset('magenta')">🟣 Magenta</button>
            <button class="preset-btn" onclick="applicaPreset('ciano')">🔵 Ciano</button>
            <button class="preset-btn" onclick="applicaPreset('arancione')">🟠 Arancione</button>
        </div>
    </div>

    <script>
        const connection = new signalR.HubConnectionBuilder()
            .withUrl("/lampada-hub")
            .build();

        connection.start().then(function () {
            console.log("Connesso a SignalR Hub");
            document.getElementById("status").textContent = "Connesso";
        }).catch(function (err) {
            console.error("Errore connessione SignalR:", err);
            document.getElementById("status").textContent = "Errore connessione";
        });

        connection.on("StatoAggiornato", function (stato) {
            aggiornaUI(stato);
        });

        connection.on("Errore", function (messaggio) {
            alert("Errore: " + messaggio);
        });

        function aggiornaUI(stato) {
            const bulb = document.getElementById("bulb");
            const status = document.getElementById("status");
            const statoDettagli = document.getElementById("statoDettagli");
            const colorePicker = document.getElementById("colorePicker");
            const luminositaSlider = document.getElementById("luminosita");
            const luminositaValue = document.getElementById("luminositaValue");

            // Aggiorna visualizzazione lampadina
            if (stato.accesa) {
                bulb.style.backgroundColor = stato.colore;
                bulb.style.opacity = stato.luminosita / 100;
                bulb.textContent = "💡";
                status.textContent = "🟢 Accesa";
            } else {
                bulb.style.backgroundColor = "#333";
                bulb.style.opacity = "0.3";
                bulb.textContent = "💡";
                status.textContent = "🔴 Spenta";
            }

            // Aggiorna controlli
            colorePicker.value = stato.colore;
            luminositaSlider.value = stato.luminosita;
            luminositaValue.textContent = stato.luminosita;

            // Aggiorna stato dettagli
            statoDettagli.innerHTML = `
                <strong>Stato:</strong> ${stato.accesa ? 'Accesa' : 'Spenta'}<br>
                <strong>Colore:</strong> ${stato.colore}<br>
                <strong>Luminosità:</strong> ${stato.luminosita}%
            `;
        }

        function toggle() {
            connection.invoke("Toggle");
        }

        function cambiaColore(colore) {
            connection.invoke("CambiaColore", colore);
        }

        function regolareLuminosita(valore) {
            document.getElementById("luminositaValue").textContent = valore;
            connection.invoke("RegolareLuminosita", parseInt(valore));
        }

        function applicaPreset(preset) {
            connection.invoke("ApplicaPreset", preset);
        }
    </script>
</body>
</html>
""";
}
