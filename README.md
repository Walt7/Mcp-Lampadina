# 🔌 MCP Lampadina

Un progetto dimostrativo che implementa una lampadina virtuale controllabile tramite **Model Context Protocol (MCP)** per integrazione con Claude Code.

## ✨ Caratteristiche

- 🌐 **Interfaccia Web**: Interfaccia grafica reattiva per controllare la lampadina
- 🎨 **Controllo Colore**: Cambia il colore della lampadina con picker o preset
- 💡 **Controllo Luminosità**: Regola l'intensità luminosa da 0 a 100%
- 🔄 **Aggiornamenti Real-time**: WebSocket per sincronizzazione live
- 🤖 **Integrazione MCP**: Server MCP per controllo tramite Claude Code
- 📡 **API REST**: Endpoint per integrazioni esterne

## 🚀 Installazione

```bash
# Clona il repository
git clone <url-repository>
cd mcp-lampadina

# Installa le dipendenze
npm install
```

## 🎮 Utilizzo

### 1. Avvia il Server Web
```bash
npm start
```

La lampadina sarà disponibile su:
- **Interfaccia Web**: http://localhost:3000
- **WebSocket**: ws://localhost:8080

### 2. Configurazione MCP per Claude Code

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

# Avvia solo il server MCP
npm run mcp
```

## 📁 Struttura Progetto

```
mcp-lampadina/
├── server.js          # Server web principale
├── mcp-server.js      # Server MCP
├── package.json       # Configurazione Node.js
├── public/            # File statici web
│   ├── index.html     # Interfaccia utente
│   └── script.js      # Logica frontend
└── README.md         # Documentazione
```

## 🤝 Come Funziona MCP

Il **Model Context Protocol** permette a Claude Code di interagire con applicazioni esterne. In questo progetto:

1. **Server MCP** (`mcp-server.js`) espone strumenti per il controllo della lampadina
2. **Claude Code** può invocare questi strumenti tramite comandi naturali
3. **Server Web** gestisce lo stato e sincronizza l'interfaccia
4. **WebSocket** mantiene aggiornati tutti i client in tempo reale

## 📝 Note Tecniche

- **Node.js** con Express per il server web
- **WebSocket** per comunicazione real-time
- **MCP SDK** per integrazione con Claude Code
- **Vanilla JavaScript** per il frontend (nessun framework)

## 🎯 Scopo del Progetto

Questo progetto serve come esempio pratico per comprendere:
- Come implementare un server MCP
- Integrazione tra Claude Code e applicazioni custom
- Comunicazione real-time con WebSocket
- Architettura di un'applicazione IoT simulata

Perfetto per sperimentare con il protocollo MCP e capire come estendere le capacità di Claude Code! 🚀