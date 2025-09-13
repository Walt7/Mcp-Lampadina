using System;
using System.Drawing;
using System.Windows.Forms;

namespace McpLampadinaWinForms
{
    public partial class LampadinaForm : Form
    {
        private McpServer _mcpServer;
        private bool _accesa = false;
        private string _colore = "#ffffff";
        private int _luminosita = 100;

        public bool IsAccesa => _accesa;
        public string Colore => _colore;
        public int Luminosita => _luminosita;

        public LampadinaForm()
        {
            InitializeComponent();
            _mcpServer = new McpServer(this);
            AggiornaInterfaccia();
        }

        private void LampadinaForm_Load(object sender, EventArgs e)
        {
            _mcpServer.Start(5001);
            lblServerStatus.Text = "🟢 MCP Server attivo su porta 5001";
            lblServerStatus.ForeColor = Color.Green;
            LogDebug("🚀 Applicazione WinForms avviata");
            LogDebug("🔌 MCP Server attivo su porta 5001");
        }

        private void LampadinaForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            _mcpServer.Stop();
        }

        public void ToggleLampadina()
        {
            _accesa = !_accesa;
            AggiornaInterfaccia();
        }

        public void CambiaColore(string colore)
        {
            if (!string.IsNullOrEmpty(colore) && colore.StartsWith("#") && colore.Length == 7)
            {
                _colore = colore;
                AggiornaInterfaccia();
            }
        }

        public void CambiaLuminosita(int luminosita)
        {
            if (luminosita >= 0 && luminosita <= 100)
            {
                _luminosita = luminosita;
                AggiornaInterfaccia();
            }
        }

        public void LogDebug(string message)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<string>(LogDebug), message);
                return;
            }

            txtDebug.AppendText($"[{DateTime.Now:HH:mm:ss}] {message}\r\n");
            txtDebug.ScrollToCaret();
        }

        private void AggiornaInterfaccia()
        {
            if (InvokeRequired)
            {
                Invoke(new Action(AggiornaInterfaccia));
                return;
            }

            lblStato.Text = _accesa ? "ACCESA" : "SPENTA";
            lblStato.ForeColor = _accesa ? Color.Green : Color.Red;

            lblColore.Text = $"Colore: {_colore.ToUpper()}";
            trackBarLuminosita.Value = _luminosita;
            lblLuminosita.Text = $"Luminosità: {_luminosita}%";

            if (_accesa)
            {
                try
                {
                    Color baseColor = ColorTranslator.FromHtml(_colore);

                    float factor = _luminosita / 100f;
                    int r = (int)(baseColor.R * factor + 255 * (1 - factor));
                    int g = (int)(baseColor.G * factor + 255 * (1 - factor));
                    int b = (int)(baseColor.B * factor + 255 * (1 - factor));

                    r = Math.Max(0, Math.Min(255, r));
                    g = Math.Max(0, Math.Min(255, g));
                    b = Math.Max(0, Math.Min(255, b));

                    Color displayColor = Color.FromArgb(r, g, b);
                    panelLampadina.BackColor = displayColor;
                }
                catch
                {
                    panelLampadina.BackColor = Color.White;
                }
            }
            else
            {
                panelLampadina.BackColor = Color.Gray;
            }

            btnToggle.Text = _accesa ? "SPEGNI" : "ACCENDI";
            btnToggle.BackColor = _accesa ? Color.FromArgb(255, 100, 100) : Color.FromArgb(100, 255, 100);
        }

        private void btnToggle_Click(object sender, EventArgs e)
        {
            ToggleLampadina();
        }

        private void trackBarLuminosita_Scroll(object sender, EventArgs e)
        {
            CambiaLuminosita(trackBarLuminosita.Value);
        }

        private void btnRosso_Click(object sender, EventArgs e)
        {
            CambiaColore("#ff0000");
        }

        private void btnVerde_Click(object sender, EventArgs e)
        {
            CambiaColore("#00ff00");
        }

        private void btnBlu_Click(object sender, EventArgs e)
        {
            CambiaColore("#0000ff");
        }

        private void btnBianco_Click(object sender, EventArgs e)
        {
            CambiaColore("#ffffff");
        }

        private void btnGiallo_Click(object sender, EventArgs e)
        {
            CambiaColore("#ffff00");
        }

        private void btnArancione_Click(object sender, EventArgs e)
        {
            CambiaColore("#ff6600");
        }
    }
}