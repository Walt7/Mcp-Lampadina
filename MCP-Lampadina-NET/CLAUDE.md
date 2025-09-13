# Configurazione Claude Code per MCP Lampadina .NET

## Configurazione MCP HTTP
```bash
# Avvia il server .NET
cd MCP-Lampadina-NET/McpLampada
dotnet run

# Aggiungi il server MCP a Claude Code
claude mcp add lampadina-net --transport http http://localhost:5000/mcp
```

## Comandi di Sviluppo
- `dotnet run` - Avvia il server
- `dotnet watch run` - Modalità sviluppo con hot reload
- `dotnet build` - Compila il progetto
- `dotnet test` - Esegue i test (se presenti)

## Struttura del Progetto .NET
- `Controllers/` - Controller API REST e MCP
- `Services/` - Logica business e server MCP
- `Models/` - Modelli dati
- `Hubs/` - SignalR Hub per real-time
- `Program.cs` - Configurazione e startup

## Stato della Lampadina .NET
La lampadina ha le stesse proprietà della versione Node.js:
- `Accesa` (bool) - Se la lampadina è accesa o spenta
- `Colore` (string) - Colore in formato esadecimale (es: #ff0000)
- `Luminosita` (int) - Luminosità da 0 a 100

## Server MCP .NET Configurato
Il server MCP espone gli stessi strumenti della versione Node.js:
- `lampadina_stato` - Ottiene stato attuale
- `lampadina_toggle` - Accende/spegne
- `lampadina_colore` - Cambia colore
- `lampadina_luminosita` - Regola luminosità
- `lampadina_preset` - Applica preset colore

## Note per Claude Code (.NET)
- Il server deve essere avviato (`dotnet run`) prima di usare gli strumenti MCP
- L'interfaccia web è disponibile su http://localhost:5000
- Gli aggiornamenti sono sincronizzati in tempo reale via SignalR
- I preset colori sono identici alla versione Node.js
- Supporto completo per CORS durante sviluppo

## Testing MCP (.NET)
Per testare l'integrazione MCP:
1. Avvia il server: `dotnet run`
2. Configura Claude Code: `claude mcp add lampadina-net --transport http http://localhost:5000/mcp`
3. Usa comandi come "accendi la lampadina", "cambia colore a rosso", etc.
4. Verifica i cambiamenti nell'interfaccia web

### Esempi di Utilizzo MCP (.NET)

#### Controllo dello Stato
```csharp
// Ottieni lo stato attuale della lampadina (stesso risultato della versione Node.js)
mcp__lampadina_net__lampadina_stato()
// Risposta: 🔌 Stato della lampadina: • Stato: 🟢 Accesa • Colore: #ffffff • Luminosità: 100%
```

#### Toggle Accensione/Spegnimento
```csharp
// Accendi o spegni la lampadina
mcp__lampadina_net__lampadina_toggle()
// Risposta: ✅ Lampadina accesa (oppure ✅ Lampadina spenta)
```

#### Cambio Colore con Preset
```csharp
// Usa preset colori predefiniti (stessi della versione Node.js)
mcp__lampadina_net__lampadina_preset({preset: "rosso"})
mcp__lampadina_net__lampadina_preset({preset: "blu"})
mcp__lampadina_net__lampadina_preset({preset: "verde"})
mcp__lampadina_net__lampadina_preset({preset: "giallo"})
mcp__lampadina_net__lampadina_preset({preset: "magenta"})
mcp__lampadina_net__lampadina_preset({preset: "ciano"})
mcp__lampadina_net__lampadina_preset({preset: "arancione"})
mcp__lampadina_net__lampadina_preset({preset: "bianco"})
// Risposta: 🎯 Preset "rosso" applicato (#ff0000)
```

#### Cambio Colore Personalizzato
```csharp
// Usa colori esadecimali personalizzati
mcp__lampadina_net__lampadina_colore({colore: "#ff6600"}) // Arancione
mcp__lampadina_net__lampadina_colore({colore: "#9932cc"}) // Viola
mcp__lampadina_net__lampadina_colore({colore: "#20b2aa"}) // Turchese
// Risposta: 🎨 Colore cambiato a #ff6600
```

#### Regolazione Luminosità
```csharp
// Regola la luminosità (0-100)
mcp__lampadina_net__lampadina_luminosita({luminosita: 50})  // 50%
mcp__lampadina_net__lampadina_luminosita({luminosita: 25})  // 25%
mcp__lampadina_net__lampadina_luminosita({luminosita: 100}) // 100%
// Risposta: 💡 Luminosità regolata al 50%
```

## API REST (.NET)
Oltre al server MCP, la versione .NET espone anche API REST:

### Endpoint Disponibili
- `GET /api/lampadina/stato` - Ottiene stato
- `POST /api/lampadina/toggle` - Toggle accensione
- `POST /api/lampadina/colore` - Cambia colore (body: `{"colore": "#ff0000"}`)
- `POST /api/lampadina/luminosita` - Regola luminosità (body: `{"luminosita": 75}`)
- `POST /api/lampadina/preset` - Applica preset (body: `{"preset": "rosso"}`)

### Test con cURL
```bash
# Ottieni stato
curl http://localhost:5000/api/lampadina/stato

# Accendi/spegni
curl -X POST http://localhost:5000/api/lampadina/toggle

# Cambia colore
curl -X POST http://localhost:5000/api/lampadina/colore \
  -H "Content-Type: application/json" \
  -d '{"colore": "#ff0000"}'

# Regola luminosità
curl -X POST http://localhost:5000/api/lampadina/luminosita \
  -H "Content-Type: application/json" \
  -d '{"luminosita": 50}'
```

## Differenze Principali con Node.js

| Caratteristica | Node.js | .NET |
|---|---|---|
| **Porta default** | 3000 | 5000 |
| **WebSocket** | Socket.IO | SignalR |
| **Linguaggio** | JavaScript | C# |
| **Comando avvio** | `npm start` | `dotnet run` |
| **Hot reload** | `npm run dev` | `dotnet watch run` |
| **Config MCP** | `http://localhost:3001/mcp` | `http://localhost:5000/mcp` |

## Comandi Conversazionali (.NET)
Identici alla versione Node.js:
- "accendi la lampadina"
- "spegni la luce"
- "cambia colore a rosso"
- "metti la lampadina blu"
- "imposta luminosità al 30%"
- "diminuisci la luminosità"
- "qual è lo stato della lampadina?"

## Verifica Funzionamento (.NET)
1. **Server attivo**: Verifica che `dotnet run` mostri "Now listening on: http://localhost:5000"
2. **MCP funzionante**: Testa con `mcp__lampadina_net__lampadina_stato()`
3. **Interfaccia web**: Apri http://localhost:5000 per vedere i cambiamenti in tempo reale
4. **SignalR**: I cambiamenti via MCP si riflettono immediatamente nell'interfaccia web
5. **API REST**: Testa con curl o Postman gli endpoint REST

## Performance e Risorse (.NET)
- **Avvio più veloce** rispetto a Node.js
- **Minore consumo di memoria** (~25MB vs ~50MB)
- **Tipizzazione statica** per maggiore sicurezza
- **Hot reload** integrato con `dotnet watch`
- **Compilazione ahead-of-time** per performance ottimali