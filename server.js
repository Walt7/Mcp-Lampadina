const express = require('express');
const cors = require('cors');
const WebSocket = require('ws');
const path = require('path');

const app = express();
const PORT = 3000;

app.use(cors());
app.use(express.json());
app.use(express.static('public'));

let lampadinaState = {
  accesa: false,
  colore: '#ffffff',
  luminosita: 100
};

const wss = new WebSocket.Server({ port: 8080 });

function broadcastState() {
  const message = JSON.stringify({
    type: 'state',
    data: lampadinaState
  });

  wss.clients.forEach(client => {
    if (client.readyState === WebSocket.OPEN) {
      client.send(message);
    }
  });
}

wss.on('connection', (ws) => {
  console.log('Nuovo client WebSocket connesso');

  ws.send(JSON.stringify({
    type: 'state',
    data: lampadinaState
  }));

  ws.on('close', () => {
    console.log('Client WebSocket disconnesso');
  });
});

app.get('/api/lampadina', (req, res) => {
  res.json({
    success: true,
    data: lampadinaState
  });
});

app.post('/api/lampadina/toggle', (req, res) => {
  lampadinaState.accesa = !lampadinaState.accesa;
  broadcastState();
  res.json({
    success: true,
    message: lampadinaState.accesa ? 'Lampadina accesa' : 'Lampadina spenta',
    data: lampadinaState
  });
});

app.post('/api/lampadina/colore', (req, res) => {
  const { colore } = req.body;
  if (!colore || typeof colore !== 'string') {
    return res.status(400).json({
      success: false,
      message: 'Colore non valido'
    });
  }

  lampadinaState.colore = colore;
  broadcastState();
  res.json({
    success: true,
    message: 'Colore cambiato',
    data: lampadinaState
  });
});

app.post('/api/lampadina/luminosita', (req, res) => {
  const { luminosita } = req.body;
  if (typeof luminosita !== 'number' || luminosita < 0 || luminosita > 100) {
    return res.status(400).json({
      success: false,
      message: 'Luminosità deve essere un numero tra 0 e 100'
    });
  }

  lampadinaState.luminosita = luminosita;
  broadcastState();
  res.json({
    success: true,
    message: 'Luminosità cambiata',
    data: lampadinaState
  });
});

app.listen(PORT, () => {
  console.log(`Server HTTP avviato su porta ${PORT}`);
  console.log(`Server WebSocket avviato su porta 8080`);
  console.log(`Interfaccia web: http://localhost:${PORT}`);
});