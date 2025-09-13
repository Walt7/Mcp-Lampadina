# MCP Lampadina .NET

Implementazione .NET del progetto MCP Lampadina con ASP.NET Core, SignalR e server MCP HTTP.

## 🚀 Caratteristiche

- **Server MCP** completo implementato in C#
- **API REST** per controllo remoto
- **SignalR Hub** per aggiornamenti real-time
- **Interfaccia web** integrata con controlli interattivi
- **Gestione stato** thread-safe
- **CORS** configurato per sviluppo

## 📁 Struttura Progetto

```
McpLampada/
├── Controllers/
│   ├── LampadinaController.cs    # API REST
│   └── McpController.cs          # Endpoint MCP HTTP
├── Services/
│   ├── LampadinaService.cs       # Logica business
│   ├── McpServer.cs              # Server MCP JSON-RPC
│   └── LampadinaNotificationService.cs # Notifiche SignalR
├── Models/
│   └── LampadinaState.cs         # Modello stato lampadina
├── Hubs/
│   └── LampadinaHub.cs           # SignalR Hub
└── Program.cs                    # Configurazione e startup
```

## 🔧 Requisiti

- **.NET 8.0** o superiore
- **Browser moderno** per l'interfaccia web

## 🚀 Avvio Rapido

```bash
# Vai nella directory del progetto .NET
cd MCP-Lampadina-NET/McpLampada

# Avvia l'applicazione
dotnet run

# Oppure in modalità watch per sviluppo
dotnet watch run
```

L'applicazione sarà disponibile su:
- **Interfaccia web**: http://localhost:5000
- **API REST**: http://localhost:5000/api/lampadina
- **Server MCP**: http://localhost:5000/mcp
- **SignalR Hub**: http://localhost:5000/lampada-hub

## 📡 API REST

### Ottieni stato
```http
GET /api/lampadina/stato
```

### Accendi/Spegni
```http
POST /api/lampadina/toggle
```

### Cambia colore
```http
POST /api/lampadina/colore
Content-Type: application/json

{
  "colore": "#ff0000"
}
```

### Regola luminosità
```http
POST /api/lampadina/luminosita
Content-Type: application/json

{
  "luminosita": 75
}
```

### Applica preset
```http
POST /api/lampadina/preset
Content-Type: application/json

{
  "preset": "rosso"
}
```

## 🔌 Server MCP

Il server MCP espone gli stessi strumenti della versione Node.js:

### Strumenti Disponibili

- **`lampadina_stato`** - Ottiene stato attuale
- **`lampadina_toggle`** - Accende/spegne
- **`lampadina_colore`** - Cambia colore (parametro: `colore`)
- **`lampadina_luminosita`** - Regola luminosità (parametro: `luminosita`)
- **`lampadina_preset`** - Applica preset (parametro: `preset`)

### Configurazione Claude Code

```bash
# Aggiungi il server MCP .NET a Claude Code
claude mcp add lampadina-net --transport http http://localhost:5000/mcp
```

## 🎨 Preset Colori

- `bianco` - #ffffff
- `rosso` - #ff0000
- `verde` - #00ff00
- `blu` - #0000ff
- `giallo` - #ffff00
- `magenta` - #ff00ff
- `ciano` - #00ffff
- `arancione` - #ffa500

## 🔄 SignalR Real-time

L'applicazione usa SignalR per sincronizzare lo stato in tempo reale:

```javascript
const connection = new signalR.HubConnectionBuilder()
    .withUrl("/lampada-hub")
    .build();

// Ricevi aggiornamenti stato
connection.on("StatoAggiornato", function (stato) {
    console.log("Nuovo stato:", stato);
});

// Invia comandi
connection.invoke("Toggle");
connection.invoke("CambiaColore", "#ff0000");
```

## 🧪 Test

### Test Manuale MCP

```bash
# Test con curl
curl -X POST http://localhost:5000/mcp \
  -H "Content-Type: application/json" \
  -d '{
    "jsonrpc": "2.0",
    "id": 1,
    "method": "tools/call",
    "params": {
      "name": "lampadina_toggle",
      "arguments": {}
    }
  }'
```

### Test API REST

```bash
# Ottieni stato
curl http://localhost:5000/api/lampadina/stato

# Toggle
curl -X POST http://localhost:5000/api/lampadina/toggle

# Cambia colore
curl -X POST http://localhost:5000/api/lampadina/colore \
  -H "Content-Type: application/json" \
  -d '{"colore": "#ff0000"}'
```

## 🔧 Sviluppo

### Dipendenze Principali

- **Microsoft.AspNetCore.SignalR** - Per WebSocket real-time
- **System.Text.Json** - Per serializzazione JSON
- **.NET 8 Minimal APIs** - Per API REST leggere

### Configurazione CORS

Il progetto è configurato con CORS permissivo per sviluppo:

```csharp
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader());
});
```

### Logging

L'applicazione usa il sistema di logging integrato di .NET:

```csharp
_logger.LogInformation($"Richiesta MCP: {method}");
```

## 🆚 Differenze con Versione Node.js

| Caratteristica | Node.js | .NET |
|---|---|---|
| **Runtime** | Node.js + Express | ASP.NET Core |
| **WebSocket** | Socket.IO | SignalR |
| **Linguaggio** | JavaScript | C# |
| **Tipizzazione** | Dinamica | Statica |
| **Performance** | V8 Engine | .NET Runtime |
| **Memoria** | ~50MB | ~25MB |

## 📚 Esempi Utilizzo

### Con Claude Code MCP

```
// Dopo aver configurato il server MCP
"accendi la lampadina"
"cambia colore a rosso"
"imposta luminosità al 50%"
"applica preset blu"
```

### Con JavaScript

```javascript
// Via fetch API
const response = await fetch('/api/lampadina/toggle', {
    method: 'POST'
});
const stato = await response.json();

// Via SignalR
connection.invoke("ApplicaPreset", "verde");
```

## 🐛 Troubleshooting

### Port già in uso
```bash
# Cambia porta in Program.cs o usa variabile ambiente
export ASPNETCORE_URLS="http://localhost:5001"
dotnet run
```

### Errori CORS
Verifica che il CORS sia configurato correttamente per il tuo dominio.

### Problemi SignalR
Controlla la console browser per errori di connessione WebSocket.

## 📄 Licenza

Stesso progetto della versione Node.js, implementato in .NET per confronto e apprendimento.