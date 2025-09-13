#!/usr/bin/env node

const express = require('express');
const cors = require('cors');
const { Server } = require('@modelcontextprotocol/sdk/server/index.js');
const {
  CallToolRequestSchema,
  ErrorCode,
  ListToolsRequestSchema,
  McpError,
} = require('@modelcontextprotocol/sdk/types.js');

const app = express();
const PORT = 3001;
const BASE_URL = 'http://localhost:3000/api/lampadina';

app.use(cors());
app.use(express.json());

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
        name: 'mcp-lampadina-http',
        version: '1.0.0',
      },
      {
        capabilities: {
          tools: {},
        },
      }
    );

    this.setupToolHandlers();
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
}

const mcpServer = new LampadinaMCPServer();

app.post('/mcp', async (req, res) => {
  try {
    const request = req.body;

    let response;
    if (request.method === 'tools/list') {
      const handler = mcpServer.server.getRequestHandler(ListToolsRequestSchema);
      response = await handler(request);
    } else if (request.method === 'tools/call') {
      const handler = mcpServer.server.getRequestHandler(CallToolRequestSchema);
      response = await handler(request);
    } else {
      return res.status(400).json({
        jsonrpc: '2.0',
        id: request.id,
        error: {
          code: -32601,
          message: 'Method not found'
        }
      });
    }

    res.json({
      jsonrpc: '2.0',
      id: request.id,
      result: response
    });
  } catch (error) {
    console.error('MCP Error:', error);
    res.status(500).json({
      jsonrpc: '2.0',
      id: req.body.id || null,
      error: {
        code: -32603,
        message: error.message
      }
    });
  }
});

app.get('/health', (req, res) => {
  res.json({ status: 'ok', service: 'mcp-lampadina-http' });
});

app.listen(PORT, () => {
  console.log(`Server MCP HTTP avviato su porta ${PORT}`);
  console.log(`Endpoint MCP: http://localhost:${PORT}/mcp`);
  console.log(`Health check: http://localhost:${PORT}/health`);
});