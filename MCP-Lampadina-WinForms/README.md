# MCP Lampadina WinForms (.NET Framework 4.7)

Simulatore di lampadina intelligente con server MCP integrato per Windows Forms.

## 🔧 Caratteristiche

- **Interfaccia WinForms nativa** per Windows
- **Server MCP integrato** implementato solo con librerie base .NET
- **Controllo completo della lampadina**: accensione/spegnimento, colore, luminosità
- **Preset colori** per cambio rapido
- **Sincronizzazione in tempo reale** tra interfaccia e comandi MCP

## 🏗️ Architettura

### Classe McpServer
- Server HTTP implementato con `HttpListener` (.NET base)
- Parsing JSON manuale senza dipendenze esterne
- Gestione thread-safe per le chiamate dall'interfaccia WinForms
- Supporto completo protocollo MCP 2024-11-05

### Interfaccia LampadinaForm
- Panel colorato per rappresentazione visiva della lampadina
- Controlli per accensione/spegnimento e regolazione luminosità
- Bottoni preset per colori comuni
- Aggiornamenti real-time dello stato

## 🚀 Avvio

### Prerequisiti
- Visual Studio 2022 (o equivalente)
- .NET Framework 4.7

### Compilazione ed Esecuzione

1. **Apertura del progetto**:
   ```cmd
   # Apri il file .csproj con Visual Studio 2022
   start McpLampadinaWinForms.csproj
   ```

2. **Compilazione**:
   - Premi F5 in Visual Studio o
   - Usa Build → Build Solution

3. **Il server MCP si avvia automaticamente** sulla porta 5000 all'apertura dell'applicazione

## 🔌 Configurazione MCP

### Aggiunta a Claude Code
```bash
claude mcp add lampadina-winforms --transport http http://localhost:5000/mcp
```

### Strumenti MCP Disponibili

| Strumento | Descrizione | Parametri |
|-----------|-------------|-----------|
| `lampadina_stato` | Ottiene stato attuale | Nessuno |
| `lampadina_toggle` | Accende/spegne | Nessuno |
| `lampadina_colore` | Cambia colore | `colore` (hex: #rrggbb) |
| `lampadina_luminosita` | Regola luminosità | `luminosita` (0-100) |
| `lampadina_preset` | Applica preset | `preset` (bianco/rosso/verde/blu/giallo/magenta/ciano/arancione) |

## 📋 Esempi di Utilizzo

### Comandi MCP Diretti

```javascript
// Stato della lampadina
mcp__lampadina-winforms__lampadina_stato()
// Risposta: 🔌 Stato della lampadina: • Stato: 🟢 Accesa • Colore: #ffffff • Luminosità: 100%

// Accendi/Spegni
mcp__lampadina-winforms__lampadina_toggle()
// Risposta: ✅ Lampadina accesa

// Cambia colore
mcp__lampadina-winforms__lampadina_colore({colore: "#ff0000"})
// Risposta: 🎨 Colore cambiato a #ff0000

// Regola luminosità
mcp__lampadina-winforms__lampadina_luminosita({luminosita: 50})
// Risposta: 💡 Luminosità regolata al 50%

// Preset colore
mcp__lampadina-winforms__lampadina_preset({preset: "rosso"})
// Risposta: 🎯 Preset "rosso" applicato (#ff0000)
```

### Comandi in Linguaggio Naturale
- "accendi la lampadina"
- "spegni la luce"
- "cambia colore a verde"
- "metti la luminosità al 30%"
- "che colore ha la lampadina?"

## 💡 Caratteristiche Tecniche

### Implementazione MCP
- **Solo librerie .NET base**: HttpListener, Thread, JSON parsing manuale
- **Thread-safe**: Chiamate sicure tra thread MCP e UI thread
- **Gestione errori**: Validazione parametri e gestione eccezioni
- **Protocollo MCP**: Supporto completo JSON-RPC 2.0

### Interfaccia WinForms
- **Rappresentazione visuale**: Panel che cambia colore in base allo stato
- **Controlli interattivi**: Bottoni, trackbar per luminosità
- **Aggiornamento real-time**: Modifiche MCP si riflettono immediatamente
- **Design responsive**: Layout fisso ottimizzato per usabilità

## 🔍 Struttura File

```
MCP-Lampadina-WinForms/
├── McpLampadinaWinForms.csproj    # Progetto Visual Studio
├── Program.cs                      # Entry point applicazione
├── LampadinaForm.cs               # Form principale con logica UI
├── LampadinaForm.Designer.cs      # Designer generato (layout)
├── LampadinaForm.resx             # Risorse del form
├── McpServer.cs                   # Server MCP con HttpListener
├── App.config                     # Configurazione applicazione
├── Properties/                    # Assembly info e risorse
├── .gitignore                     # Esclusioni Git
└── README.md                      # Questa documentazione
```

## 🐛 Risoluzione Problemi

### Server MCP non si avvia
- Verifica che la porta 5000 sia libera
- Esegui l'applicazione come amministratore se necessario
- Controlla Windows Firewall

### Comandi MCP non funzionano
- Verifica che l'applicazione WinForms sia in esecuzione
- Controlla la configurazione MCP in Claude Code
- Usa `claude mcp list` per verificare la configurazione

### Errori di compilazione
- Verifica di avere .NET Framework 4.7 installato
- Controlla che tutte le reference siano corrette nel .csproj
- Pulisci e ricompila la soluzione (Build → Clean Solution)

## 📜 Licenza

Progetto dimostrativo per integrazione MCP con WinForms .NET Framework 4.7.