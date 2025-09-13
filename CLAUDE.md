# Configurazione Claude Code per MCP Lampadina

## Configurazione MCP HTTP
```bash
# Avvia i server
npm run all

# Aggiungi il server MCP a Claude Code
claude mcp add lampadina --transport http http://localhost:3001/mcp
```

## Comandi di Test e Build
- `npm start` - Avvia il server web principale
- `npm run dev` - Modalità sviluppo con nodemon
- `npm run mcp` - Avvia solo il server MCP

## Struttura del Progetto
- `server.js` - Server web principale con API REST e WebSocket
- `mcp-server.js` - Server MCP per integrazione Claude Code
- `public/` - Interfaccia web della lampadina
- `package.json` - Configurazione dipendenze Node.js

## Stato della Lampadina
La lampadina ha tre proprietà principali:
- `accesa` (boolean) - Se la lampadina è accesa o spenta
- `colore` (string) - Colore in formato esadecimale (es: #ff0000)
- `luminosita` (number) - Luminosità da 0 a 100

## Server MCP Configurato
Il server MCP espone questi strumenti:
- `lampadina_stato` - Ottiene stato attuale
- `lampadina_toggle` - Accende/spegne
- `lampadina_colore` - Cambia colore
- `lampadina_luminosita` - Regola luminosità
- `lampadina_preset` - Applica preset colore

## Note per Claude Code
- Il server web deve essere avviato (`npm start`) prima di usare gli strumenti MCP
- L'interfaccia web è disponibile su http://localhost:3000
- Gli aggiornamenti sono sincronizzati in tempo reale via WebSocket
- I preset colori includono: bianco, rosso, verde, blu, giallo, magenta, ciano, arancione

## Testing MCP
Per testare l'integrazione MCP:
1. Avvia il server: `npm start`
2. Usa comandi come "accendi la lampadina", "cambia colore a rosso", etc.
3. Verifica i cambiamenti nell'interfaccia web

### Esempi di Utilizzo MCP

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
1. **Server attivo**: Verifica che `npm start` mostri "Server HTTP avviato su porta 3000"
2. **MCP funzionante**: Testa con `mcp__lampadina__lampadina_stato()`
3. **Interfaccia web**: Apri http://localhost:3000 per vedere i cambiamenti in tempo reale
4. **WebSocket**: I cambiamenti via MCP si riflettono immediatamente nell'interfaccia web