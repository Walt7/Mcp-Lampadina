class LampadinaController {
    constructor() {
        this.init();
        this.connectWebSocket();
    }

    init() {
        this.lampadina = document.getElementById('lampadina');
        this.toggleBtn = document.getElementById('toggleBtn');
        this.colorPicker = document.getElementById('colorPicker');
        this.brightnessSlider = document.getElementById('brightnessSlider');
        this.brightnessValue = document.getElementById('brightnessValue');
        this.statusText = document.getElementById('statusText');
        this.colorText = document.getElementById('colorText');
        this.luminositaText = document.getElementById('luminositaText');

        this.setupEventListeners();
        this.loadInitialState();
    }

    connectWebSocket() {
        this.ws = new WebSocket('ws://localhost:8080');

        this.ws.onopen = () => {
            console.log('WebSocket connesso');
        };

        this.ws.onmessage = (event) => {
            const message = JSON.parse(event.data);
            if (message.type === 'state') {
                this.updateUI(message.data);
            }
        };

        this.ws.onclose = () => {
            console.log('WebSocket disconnesso, tentativo di riconnessione...');
            setTimeout(() => this.connectWebSocket(), 3000);
        };

        this.ws.onerror = (error) => {
            console.error('Errore WebSocket:', error);
        };
    }

    setupEventListeners() {
        this.toggleBtn.addEventListener('click', () => this.toggleLampadina());

        this.colorPicker.addEventListener('change', (e) => {
            this.cambiaColore(e.target.value);
        });

        this.brightnessSlider.addEventListener('input', (e) => {
            const value = parseInt(e.target.value);
            this.brightnessValue.textContent = value;
            this.cambiaLuminosita(value);
        });

        document.querySelectorAll('.preset-color').forEach(preset => {
            preset.addEventListener('click', (e) => {
                const colore = e.target.dataset.color;
                this.colorPicker.value = colore;
                this.cambiaColore(colore);
            });
        });
    }

    async loadInitialState() {
        try {
            const response = await fetch('/api/lampadina');
            const result = await response.json();
            if (result.success) {
                this.updateUI(result.data);
            }
        } catch (error) {
            console.error('Errore nel caricamento dello stato:', error);
        }
    }

    updateUI(state) {
        this.lampadina.className = state.accesa ? 'lampadina accesa' : 'lampadina spenta';

        if (state.accesa) {
            const opacity = state.luminosita / 100;
            this.lampadina.style.backgroundColor = this.hexToRgba(state.colore, opacity);
            this.lampadina.style.color = state.colore;
        } else {
            this.lampadina.style.backgroundColor = '#333';
            this.lampadina.style.color = '#333';
        }

        this.colorPicker.value = state.colore;
        this.brightnessSlider.value = state.luminosita;
        this.brightnessValue.textContent = state.luminosita;

        this.statusText.textContent = state.accesa ? 'Accesa' : 'Spenta';
        this.colorText.textContent = state.colore;
        this.luminositaText.textContent = state.luminosita + '%';
    }

    hexToRgba(hex, alpha) {
        const r = parseInt(hex.slice(1, 3), 16);
        const g = parseInt(hex.slice(3, 5), 16);
        const b = parseInt(hex.slice(5, 7), 16);
        return `rgba(${r}, ${g}, ${b}, ${alpha})`;
    }

    async toggleLampadina() {
        try {
            const response = await fetch('/api/lampadina/toggle', {
                method: 'POST'
            });
            const result = await response.json();
            console.log(result.message);
        } catch (error) {
            console.error('Errore nel toggle della lampadina:', error);
        }
    }

    async cambiaColore(colore) {
        try {
            const response = await fetch('/api/lampadina/colore', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({ colore })
            });
            const result = await response.json();
            console.log(result.message);
        } catch (error) {
            console.error('Errore nel cambio colore:', error);
        }
    }

    async cambiaLuminosita(luminosita) {
        try {
            const response = await fetch('/api/lampadina/luminosita', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({ luminosita })
            });
            const result = await response.json();
            console.log(result.message);
        } catch (error) {
            console.error('Errore nel cambio luminosità:', error);
        }
    }
}

document.addEventListener('DOMContentLoaded', () => {
    new LampadinaController();
});