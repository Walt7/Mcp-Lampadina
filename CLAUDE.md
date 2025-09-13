# Configurazione Claude Code per MCP Lampadina

🚀 **Disponibile in tre implementazioni:**
- **Node.js** - Versione originale con Express e Socket.IO
- **.NET** - Versione moderna con ASP.NET Core e SignalR
- **WinForms** - Versione desktop con interfaccia Windows Forms (.NET 4.7)

## Configurazione MCP HTTP

### Node.js (Versione Originale)
```bash
# Avvia i server
npm run all

# Aggiungi il server MCP a Claude Code
claude mcp add lampadina --transport http http://localhost:3001/mcp
```

### .NET (Versione Moderna)
```bash
# Avvia il server .NET
cd MCP-Lampadina-NET/McpLampada
dotnet run

# Aggiungi il server MCP .NET a Claude Code
claude mcp add lampadina-net --transport http http://localhost:5000/mcp
```

### WinForms (Versione Desktop)
```bash
# Compila e avvia l'applicazione WinForms
cd MCP-Lampadina-WinForms
# Apri McpLampadinaWinForms.sln in Visual Studio oppure
# Avvia direttamente il file .exe dalla cartella bin/Debug

# Aggiungi il server MCP WinForms a Claude Code
claude mcp add lampadina-winforms --transport http http://localhost:5001/mcp
```

## Comandi di Test e Build

### Node.js
- `npm start` - Avvia il server web principale
- `npm run dev` - Modalità sviluppo con nodemon
- `npm run mcp` - Avvia solo il server MCP

### .NET
- `dotnet run` - Avvia il server
- `dotnet watch run` - Modalità sviluppo con hot reload
- `dotnet build` - Compila il progetto

### WinForms
- Avvia l'applicazione tramite Visual Studio o file .exe
- Il server MCP si avvia automaticamente sulla porta 5001
- Interfaccia grafica desktop per controllo diretto della lampadina
- Include log di debug integrato per monitorare le richieste MCP

## Struttura del Progetto

### Node.js
- `server.js` - Server web principale con API REST e WebSocket
- `mcp-server.js` - Server MCP per integrazione Claude Code
- `public/` - Interfaccia web della lampadina
- `package.json` - Configurazione dipendenze Node.js

### .NET
- `Controllers/` - API REST e MCP endpoints
- `Services/` - Business logic e server MCP
- `Models/` - Data models tipizzati
- `Hubs/` - SignalR Hub per real-time
- `Program.cs` - Configurazione startup

### WinForms
- `LampadinaForm.cs` - Form principale con UI e controlli
- `LampadinaForm.Designer.cs` - Definizione interfaccia grafica
- `McpServer.cs` - Server MCP HTTP integrato
- `Program.cs` - Entry point dell'applicazione
- Include pulsante "Pulisci Log" per gestione del debug

## Stato della Lampadina (Tutte le Versioni)
La lampadina ha tre proprietà principali:
- `accesa` (boolean) - Se la lampadina è accesa o spenta
- `colore` (string) - Colore in formato esadecimale (es: #ff0000)
- `luminosita` (number) - Luminosità da 0 a 100

## Server MCP Configurato (Identici per Tutte le Versioni)
Il server MCP espone questi strumenti:
- `lampadina_stato` - Ottiene stato attuale
- `lampadina_toggle` - Accende/spegne
- `lampadina_colore` - Cambia colore
- `lampadina_luminosita` - Regola luminosità
- `lampadina_preset` - Applica preset colore

## Note per Claude Code

### Node.js
- Il server web deve essere avviato (`npm start`) prima di usare gli strumenti MCP
- L'interfaccia web è disponibile su http://localhost:3000
- Gli aggiornamenti sono sincronizzati in tempo reale via Socket.IO
- I preset colori includono: bianco, rosso, verde, blu, giallo, magenta, ciano, arancione

### .NET
- Il server deve essere avviato (`dotnet run`) prima di usare gli strumenti MCP
- L'interfaccia web è disponibile su http://localhost:5000 (porta dinamica)
- Gli aggiornamenti sono sincronizzati in tempo reale via SignalR
- I preset colori sono identici alla versione Node.js

### WinForms
- L'applicazione desktop deve essere avviata per utilizzare gli strumenti MCP
- Interfaccia grafica nativa Windows con controlli diretti per lampadina
- Server MCP integrato in esecuzione su porta 5001
- Log di debug in tempo reale per monitorare le richieste MCP
- I preset colori sono identici alle altre versioni

## Differenze Principali

| Caratteristica | Node.js | .NET | WinForms |
|---|---|---|---|
| **Tipo Interface** | Web Browser | Web Browser | Desktop Nativo |
| **Porta Web** | 3000 | 5000 | N/A |
| **Porta MCP** | 3001 | 5000 | 5001 |
| **Real-time** | Socket.IO | SignalR | Form Events |
| **Avvio** | `npm start` | `dotnet run` | Visual Studio/.exe |
| **Hot Reload** | `npm run dev` | `dotnet watch run` | N/A |
| **Config MCP** | `lampadina` | `lampadina-net` | `lampadina-winforms` |
| **Debug Log** | Console | Console | UI Integrato |

## Testing MCP

### Node.js (Versione Originale)
Per testare l'integrazione MCP:
1. Avvia il server: `npm start`
2. Usa comandi come "accendi la lampadina", "cambia colore a rosso", etc.
3. Verifica i cambiamenti nell'interfaccia web

### .NET (Versione Moderna)
Per testare l'integrazione MCP:
1. Avvia il server: `cd MCP-Lampadina-NET/McpLampada && dotnet run`
2. Configura MCP: `claude mcp add lampadina-net --transport http http://localhost:5000/mcp`
3. Usa gli stessi comandi conversazionali della versione Node.js
4. Verifica i cambiamenti nell'interfaccia web su porta 5000

### WinForms (Versione Desktop)
Per testare l'integrazione MCP:
1. Avvia l'applicazione: Apri in Visual Studio o esegui il file .exe
2. Configura MCP: `claude mcp add lampadina-winforms --transport http http://localhost:5001/mcp`
3. Usa gli stessi comandi conversazionali delle altre versioni
4. Verifica i cambiamenti direttamente nell'interfaccia desktop
5. Monitora le richieste MCP nel log di debug integrato

### Esempi di Utilizzo MCP (Identici per Tutte le Versioni)

#### Controllo dello Stato
```javascript
// Ottieni lo stato attuale della lampadina
mcp__lampadina__lampadina_stato()
// Risposta: 🔌 Stato della lampadina: • Stato: 🟢 Accesa • Colore: #ffffff • Luminosità: 100%
```

#### Toggle Accensione/Spegnimento
```javascript
// Accendi o spegni la lampadina
mcp__lampadina__lampadina_toggle()
// Risposta: ✅ Lampadina accesa (oppure ✅ Lampadina spenta)
```

#### Cambio Colore con Preset
```javascript
// Usa preset colori predefiniti
mcp__lampadina__lampadina_preset({preset: "rosso"})
mcp__lampadina__lampadina_preset({preset: "blu"})
mcp__lampadina__lampadina_preset({preset: "verde"})
mcp__lampadina__lampadina_preset({preset: "giallo"})
mcp__lampadina__lampadina_preset({preset: "magenta"})
mcp__lampadina__lampadina_preset({preset: "ciano"})
mcp__lampadina__lampadina_preset({preset: "arancione"})
mcp__lampadina__lampadina_preset({preset: "bianco"})
// Risposta: 🎯 Preset "rosso" applicato (#ff0000)
```

#### Cambio Colore Personalizzato
```javascript
// Usa colori esadecimali personalizzati
mcp__lampadina__lampadina_colore({colore: "#ff6600"}) // Arancione
mcp__lampadina__lampadina_colore({colore: "#9932cc"}) // Viola
mcp__lampadina__lampadina_colore({colore: "#20b2aa"}) // Turchese
// Risposta: 🎨 Colore cambiato a #ff6600
```

#### Regolazione Luminosità
```javascript
// Regola la luminosità (0-100)
mcp__lampadina__lampadina_luminosita({luminosita: 50})  // 50%
mcp__lampadina__lampadina_luminosita({luminosita: 25})  // 25%
mcp__lampadina__lampadina_luminosita({luminosita: 100}) // 100%
// Risposta: 💡 Luminosità regolata al 50%
```

### Comandi Conversazionali
Puoi anche usare comandi in linguaggio naturale:
- "accendi la lampadina"
- "spegni la luce"
- "cambia colore a rosso"
- "metti la lampadina blu"
- "imposta luminosità al 30%"
- "diminuisci la luminosità"
- "qual è lo stato della lampadina?"

### Verifica Funzionamento

#### Node.js
1. **Server attivo**: Verifica che `npm start` mostri "Server HTTP avviato su porta 3000"
2. **MCP funzionante**: Testa con `mcp__lampadina__lampadina_stato()`
3. **Interfaccia web**: Apri http://localhost:3000 per vedere i cambiamenti in tempo reale
4. **Socket.IO**: I cambiamenti via MCP si riflettono immediatamente nell'interfaccia web

#### .NET
1. **Server attivo**: Verifica che `dotnet run` mostri "Now listening on: http://localhost:5000"
2. **MCP funzionante**: Usa lo stesso nome strumento ma con server `lampadina-net`
3. **Interfaccia web**: Apri http://localhost:5000 per vedere i cambiamenti in tempo reale
4. **SignalR**: I cambiamenti via MCP si riflettono immediatamente nell'interfaccia web

#### WinForms
1. **Applicazione attiva**: Verifica che l'app WinForms sia in esecuzione e mostri "MCP Server attivo su porta 5001"
2. **MCP funzionante**: Usa gli strumenti con server `lampadina-winforms`
3. **Interfaccia desktop**: I cambiamenti sono visibili immediatamente nel form
4. **Debug integrato**: Monitora le richieste MCP nel log di debug in tempo reale

### Quale Versione Scegliere?

| Scegli Node.js se: | Scegli .NET se: | Scegli WinForms se: |
|---|---|---|
| Hai familiarità con JavaScript | Preferisci tipizzazione statica | Vuoi un'applicazione desktop |
| Vuoi prototipare rapidamente | Vuoi performance migliori | Lavori principalmente su Windows |
| Lavori in ambiente JavaScript | Lavori in ambiente Microsoft | Preferisci interfacce native |
| Preferisci meno struttura | Vuoi più struttura e sicurezza | Vuoi debug integrato visuale |
| Sviluppo web-first | Architettura moderna | Controllo diretto dell'UI |