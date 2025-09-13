# 🔌 MCP Lampadina

Un progetto dimostrativo che implementa una lampadina virtuale controllabile tramite **Model Context Protocol (MCP)** per integrazione con Claude Code.

**🚀 Disponibile in due implementazioni complete:**
- **Node.js** - Versione originale con Express e Socket.IO
- **.NET** - Versione moderna con ASP.NET Core e SignalR

## ✨ Caratteristiche

- 🌐 **Interfaccia Web**: Interfaccia grafica reattiva per controllare la lampadina
- 🎨 **Controllo Colore**: Cambia il colore della lampadina con picker o preset
- 💡 **Controllo Luminosità**: Regola l'intensità luminosa da 0 a 100%
- 🔄 **Aggiornamenti Real-time**: WebSocket per sincronizzazione live
- 🤖 **Integrazione MCP**: Server MCP per controllo tramite Claude Code
- 📡 **API REST**: Endpoint per integrazioni esterne

## 🚀 Installazione

### Versione Node.js
```bash
# Clona il repository
git clone <url-repository>
cd mcp-lampadina

# Installa le dipendenze
npm install
```

### Versione .NET
```bash
# Vai nella cartella .NET
cd MCP-Lampadina-NET/McpLampada

# Ripristina le dipendenze (automatico)
dotnet restore
```

## 🎮 Utilizzo

### 1. Avvia il Server Web

#### Node.js
```bash
npm start
```
La lampadina sarà disponibile su:
- **Interfaccia Web**: http://localhost:3000
- **WebSocket**: ws://localhost:8080

#### .NET
```bash
dotnet run
```
La lampadina sarà disponibile su:
- **Interfaccia Web**: http://localhost:5000 (o porta dinamica)
- **SignalR Hub**: http://localhost:5000/lampada-hub

### 2. Configurazione MCP per Claude Code

#### Opzione A: Configurazione locale (stdio)
Aggiungi questa configurazione al tuo `claude_desktop_config.json`:

```json
{
  "mcpServers": {
    "lampadina": {
      "command": "node",
      "args": ["C:\\path\\to\\mcp-lampadina\\mcp-server.js"],
      "cwd": "C:\\path\\to\\mcp-lampadina"
    }
  }
}
```

#### Opzione B: Configurazione HTTP con claude mcp add

**Node.js:**
```bash
# Prima avvia entrambi i server
npm run all

# Poi usa il comando claude mcp add
claude mcp add lampadina --transport http http://localhost:3001/mcp
```

**.NET:**
```bash
# Prima avvia il server .NET
cd MCP-Lampadina-NET/McpLampada
dotnet run

# Poi usa il comando claude mcp add
claude mcp add lampadina-net --transport http http://localhost:5000/mcp
```

### 3. Usa Claude Code per Controllare la Lampadina

Ora puoi usare comandi come:
- "Accendi la lampadina"
- "Cambia il colore a rosso"
- "Regola la luminosità al 50%"
- "Applica il preset blu"
- "Mostrami lo stato della lampadina"

## 🛠️ API Endpoints

### GET `/api/lampadina`
Ottiene lo stato attuale della lampadina.

```json
{
  "success": true,
  "data": {
    "accesa": true,
    "colore": "#ff0000",
    "luminosita": 75
  }
}
```

### POST `/api/lampadina/toggle`
Accende o spegne la lampadina.

### POST `/api/lampadina/colore`
```json
{
  "colore": "#00ff00"
}
```

### POST `/api/lampadina/luminosita`
```json
{
  "luminosita": 80
}
```

## 🤖 Strumenti MCP Disponibili

| Strumento | Descrizione |
|-----------|-------------|
| `lampadina_stato` | Ottiene lo stato attuale |
| `lampadina_toggle` | Accende/spegne |
| `lampadina_colore` | Cambia colore (formato hex) |
| `lampadina_luminosita` | Regola luminosità (0-100) |
| `lampadina_preset` | Applica preset colore |

### Preset Colori Disponibili
- `bianco` (#ffffff)
- `rosso` (#ff0000)
- `verde` (#00ff00)
- `blu` (#0000ff)
- `giallo` (#ffff00)
- `magenta` (#ff00ff)
- `ciano` (#00ffff)
- `arancione` (#ffa500)

## 🔧 Sviluppo

```bash
# Modalità sviluppo con hot reload
npm run dev

# Avvia solo il server MCP (stdio)
npm run mcp

# Avvia solo il server MCP HTTP
npm run mcp-http

# Avvia entrambi i server (web + MCP HTTP)
npm run all
```

## 📁 Struttura Progetto

### Versione Node.js
```
mcp-lampadina/
├── server.js              # Server web principale
├── mcp-server.js          # Server MCP (stdio)
├── mcp-http-server.js     # Server MCP HTTP
├── package.json           # Configurazione Node.js
├── public/                # File statici web
│   ├── index.html         # Interfaccia utente
│   └── script.js          # Logica frontend
├── CLAUDE.md             # Configurazione Claude Code
└── README.md             # Documentazione
```

### Versione .NET
```
MCP-Lampadina-NET/
└── McpLampada/
    ├── Controllers/       # API REST e MCP endpoints
    ├── Services/          # Business logic e MCP server
    ├── Models/            # Data models
    ├── Hubs/              # SignalR hubs
    ├── Program.cs         # Startup configuration
    ├── McpLampada.csproj  # Project configuration
    ├── CLAUDE.md          # Claude Code configuration
    └── README.md          # Documentation
```

## 🤝 Come Funziona MCP

Il **Model Context Protocol** permette a Claude Code di interagire con applicazioni esterne. In questo progetto:

1. **Server MCP** (`mcp-server.js`) espone strumenti per il controllo della lampadina
2. **Claude Code** può invocare questi strumenti tramite comandi naturali
3. **Server Web** gestisce lo stato e sincronizza l'interfaccia
4. **WebSocket** mantiene aggiornati tutti i client in tempo reale

## 📝 Note Tecniche

### Node.js
- **Node.js** con Express per il server web
- **Socket.IO** per comunicazione real-time
- **MCP SDK** per integrazione con Claude Code
- **Vanilla JavaScript** per il frontend (nessun framework)

### .NET
- **ASP.NET Core** per server web e API
- **SignalR** per comunicazione real-time
- **System.Text.Json** per serializzazione JSON
- **Minimal APIs** per endpoint leggeri

## 🆚 Confronto Implementazioni

| Caratteristica | Node.js | .NET |
|---|---|---|
| **Runtime** | V8 Engine | .NET Runtime |
| **Linguaggio** | JavaScript | C# |
| **Server Web** | Express.js | ASP.NET Core |
| **WebSocket** | Socket.IO | SignalR |
| **Porta Default** | 3000 | 5000 |
| **Hot Reload** | nodemon | dotnet watch |
| **Memoria** | ~50MB | ~25MB |
| **Avvio** | ~2s | ~1s |
| **Tipizzazione** | Dinamica | Statica |
| **Package Manager** | npm | NuGet |

## 🎯 Scopo del Progetto

Questo progetto serve come esempio pratico per comprendere:
- Come implementare un server MCP
- Integrazione tra Claude Code e applicazioni custom
- Comunicazione real-time con WebSocket
- Architettura di un'applicazione IoT simulata

Perfetto per sperimentare con il protocollo MCP e capire come estendere le capacità di Claude Code! 🚀