#!/usr/bin/env node

const { Server } = require('@modelcontextprotocol/sdk/server/index.js');
const { StdioServerTransport } = require('@modelcontextprotocol/sdk/server/stdio.js');
const {
  CallToolRequestSchema,
  ErrorCode,
  ListToolsRequestSchema,
  McpError,
} = require('@modelcontextprotocol/sdk/types.js');

const BASE_URL = 'http://localhost:3000/api/lampadina';

async function makeApiCall(endpoint, method = 'GET', body = null) {
  try {
    const options = {
      method,
      headers: {
        'Content-Type': 'application/json',
      },
    };

    if (body) {
      options.body = JSON.stringify(body);
    }

    const response = await fetch(`${BASE_URL}${endpoint}`, options);
    const data = await response.json();

    if (!response.ok) {
      throw new Error(`API Error: ${data.message || 'Errore sconosciuto'}`);
    }

    return data;
  } catch (error) {
    throw new Error(`Errore di connessione: ${error.message}`);
  }
}

class LampadinaMCPServer {
  constructor() {
    this.server = new Server(
      {
        name: 'mcp-lampadina',
        version: '1.0.0',
      },
      {
        capabilities: {
          tools: {},
        },
      }
    );

    this.setupToolHandlers();
    this.setupErrorHandling();
  }

  setupErrorHandling() {
    this.server.onerror = (error) => {
      console.error('[MCP Error]', error);
    };

    process.on('SIGINT', async () => {
      await this.server.close();
      process.exit(0);
    });
  }

  setupToolHandlers() {
    this.server.setRequestHandler(ListToolsRequestSchema, async () => {
      return {
        tools: [
          {
            name: 'lampadina_stato',
            description: 'Ottiene lo stato attuale della lampadina (accesa/spenta, colore, luminosità)',
            inputSchema: {
              type: 'object',
              properties: {},
              required: [],
            },
          },
          {
            name: 'lampadina_toggle',
            description: 'Accende o spegne la lampadina',
            inputSchema: {
              type: 'object',
              properties: {},
              required: [],
            },
          },
          {
            name: 'lampadina_colore',
            description: 'Cambia il colore della lampadina',
            inputSchema: {
              type: 'object',
              properties: {
                colore: {
                  type: 'string',
                  description: 'Il colore in formato esadecimale (es: #ff0000 per rosso, #00ff00 per verde)',
                  pattern: '^#[0-9a-fA-F]{6}$'
                }
              },
              required: ['colore'],
            },
          },
          {
            name: 'lampadina_luminosita',
            description: 'Regola la luminosità della lampadina',
            inputSchema: {
              type: 'object',
              properties: {
                luminosita: {
                  type: 'number',
                  description: 'Livello di luminosità da 0 a 100',
                  minimum: 0,
                  maximum: 100
                }
              },
              required: ['luminosita'],
            },
          },
          {
            name: 'lampadina_preset',
            description: 'Applica un preset di colore alla lampadina',
            inputSchema: {
              type: 'object',
              properties: {
                preset: {
                  type: 'string',
                  description: 'Nome del preset colore',
                  enum: ['bianco', 'rosso', 'verde', 'blu', 'giallo', 'magenta', 'ciano', 'arancione']
                }
              },
              required: ['preset'],
            },
          }
        ],
      };
    });

    this.server.setRequestHandler(CallToolRequestSchema, async (request) => {
      const { name, arguments: args } = request.params;

      try {
        switch (name) {
          case 'lampadina_stato':
            return await this.getStato();

          case 'lampadina_toggle':
            return await this.toggleLampadina();

          case 'lampadina_colore':
            return await this.cambiaColore(args.colore);

          case 'lampadina_luminosita':
            return await this.cambiaLuminosita(args.luminosita);

          case 'lampadina_preset':
            return await this.applicaPreset(args.preset);

          default:
            throw new McpError(
              ErrorCode.MethodNotFound,
              `Strumento sconosciuto: ${name}`
            );
        }
      } catch (error) {
        throw new McpError(
          ErrorCode.InternalError,
          `Errore nell'esecuzione di ${name}: ${error.message}`
        );
      }
    });
  }

  async getStato() {
    const result = await makeApiCall('');
    const stato = result.data;

    return {
      content: [
        {
          type: 'text',
          text: `🔌 Stato della lampadina:
• Stato: ${stato.accesa ? '🟢 Accesa' : '🔴 Spenta'}
• Colore: ${stato.colore}
• Luminosità: ${stato.luminosita}%

La lampadina è ${stato.accesa ? 'accesa' : 'spenta'} con colore ${stato.colore} e luminosità al ${stato.luminosita}%.`
        }
      ]
    };
  }

  async toggleLampadina() {
    const result = await makeApiCall('/toggle', 'POST');
    const stato = result.data;

    return {
      content: [
        {
          type: 'text',
          text: `✅ ${result.message}

🔌 Stato aggiornato:
• Stato: ${stato.accesa ? '🟢 Accesa' : '🔴 Spenta'}
• Colore: ${stato.colore}
• Luminosità: ${stato.luminosita}%`
        }
      ]
    };
  }

  async cambiaColore(colore) {
    if (!colore.match(/^#[0-9a-fA-F]{6}$/)) {
      throw new Error('Il colore deve essere in formato esadecimale (es: #ff0000)');
    }

    const result = await makeApiCall('/colore', 'POST', { colore });
    const stato = result.data;

    return {
      content: [
        {
          type: 'text',
          text: `🎨 Colore cambiato a ${colore}

🔌 Stato aggiornato:
• Stato: ${stato.accesa ? '🟢 Accesa' : '🔴 Spenta'}
• Colore: ${stato.colore}
• Luminosità: ${stato.luminosita}%`
        }
      ]
    };
  }

  async cambiaLuminosita(luminosita) {
    if (typeof luminosita !== 'number' || luminosita < 0 || luminosita > 100) {
      throw new Error('La luminosità deve essere un numero tra 0 e 100');
    }

    const result = await makeApiCall('/luminosita', 'POST', { luminosita });
    const stato = result.data;

    return {
      content: [
        {
          type: 'text',
          text: `💡 Luminosità cambiata al ${luminosita}%

🔌 Stato aggiornato:
• Stato: ${stato.accesa ? '🟢 Accesa' : '🔴 Spenta'}
• Colore: ${stato.colore}
• Luminosità: ${stato.luminosita}%`
        }
      ]
    };
  }

  async applicaPreset(preset) {
    const presetColors = {
      'bianco': '#ffffff',
      'rosso': '#ff0000',
      'verde': '#00ff00',
      'blu': '#0000ff',
      'giallo': '#ffff00',
      'magenta': '#ff00ff',
      'ciano': '#00ffff',
      'arancione': '#ffa500'
    };

    const colore = presetColors[preset];
    if (!colore) {
      throw new Error(`Preset sconosciuto: ${preset}`);
    }

    const result = await makeApiCall('/colore', 'POST', { colore });
    const stato = result.data;

    return {
      content: [
        {
          type: 'text',
          text: `🎯 Preset "${preset}" applicato (${colore})

🔌 Stato aggiornato:
• Stato: ${stato.accesa ? '🟢 Accesa' : '🔴 Spenta'}
• Colore: ${stato.colore}
• Luminosità: ${stato.luminosita}%`
        }
      ]
    };
  }

  async run() {
    const transport = new StdioServerTransport();
    await this.server.connect(transport);
    console.error('Server MCP Lampadina avviato');
  }
}

const server = new LampadinaMCPServer();
server.run().catch(console.error);