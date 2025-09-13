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