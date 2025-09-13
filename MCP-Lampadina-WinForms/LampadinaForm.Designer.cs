namespace McpLampadinaWinForms
{
    partial class LampadinaForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.panelLampadina = new System.Windows.Forms.Panel();
            this.lblStato = new System.Windows.Forms.Label();
            this.btnToggle = new System.Windows.Forms.Button();
            this.lblColore = new System.Windows.Forms.Label();
            this.trackBarLuminosita = new System.Windows.Forms.TrackBar();
            this.lblLuminosita = new System.Windows.Forms.Label();
            this.groupBoxPreset = new System.Windows.Forms.GroupBox();
            this.btnArancione = new System.Windows.Forms.Button();
            this.btnGiallo = new System.Windows.Forms.Button();
            this.btnBianco = new System.Windows.Forms.Button();
            this.btnBlu = new System.Windows.Forms.Button();
            this.btnVerde = new System.Windows.Forms.Button();
            this.btnRosso = new System.Windows.Forms.Button();
            this.lblServerStatus = new System.Windows.Forms.Label();
            this.lblTitolo = new System.Windows.Forms.Label();
            this.txtDebug = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarLuminosita)).BeginInit();
            this.groupBoxPreset.SuspendLayout();
            this.SuspendLayout();

            // panelLampadina
            this.panelLampadina.BackColor = System.Drawing.Color.Gray;
            this.panelLampadina.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelLampadina.Location = new System.Drawing.Point(50, 80);
            this.panelLampadina.Name = "panelLampadina";
            this.panelLampadina.Size = new System.Drawing.Size(200, 200);
            this.panelLampadina.TabIndex = 0;

            // lblStato
            this.lblStato.AutoSize = true;
            this.lblStato.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Bold);
            this.lblStato.ForeColor = System.Drawing.Color.Red;
            this.lblStato.Location = new System.Drawing.Point(115, 300);
            this.lblStato.Name = "lblStato";
            this.lblStato.Size = new System.Drawing.Size(90, 26);
            this.lblStato.TabIndex = 1;
            this.lblStato.Text = "SPENTA";

            // btnToggle
            this.btnToggle.BackColor = System.Drawing.Color.FromArgb(100, 255, 100);
            this.btnToggle.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold);
            this.btnToggle.Location = new System.Drawing.Point(100, 340);
            this.btnToggle.Name = "btnToggle";
            this.btnToggle.Size = new System.Drawing.Size(100, 40);
            this.btnToggle.TabIndex = 2;
            this.btnToggle.Text = "ACCENDI";
            this.btnToggle.UseVisualStyleBackColor = false;
            this.btnToggle.Click += new System.EventHandler(this.btnToggle_Click);

            // lblColore
            this.lblColore.AutoSize = true;
            this.lblColore.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.lblColore.Location = new System.Drawing.Point(280, 80);
            this.lblColore.Name = "lblColore";
            this.lblColore.Size = new System.Drawing.Size(107, 17);
            this.lblColore.TabIndex = 3;
            this.lblColore.Text = "Colore: #FFFFFF";

            // trackBarLuminosita
            this.trackBarLuminosita.Location = new System.Drawing.Point(280, 120);
            this.trackBarLuminosita.Maximum = 100;
            this.trackBarLuminosita.Name = "trackBarLuminosita";
            this.trackBarLuminosita.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.trackBarLuminosita.Size = new System.Drawing.Size(45, 200);
            this.trackBarLuminosita.TabIndex = 4;
            this.trackBarLuminosita.TickFrequency = 10;
            this.trackBarLuminosita.Value = 100;
            this.trackBarLuminosita.Scroll += new System.EventHandler(this.trackBarLuminosita_Scroll);

            // lblLuminosita
            this.lblLuminosita.AutoSize = true;
            this.lblLuminosita.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.lblLuminosita.Location = new System.Drawing.Point(280, 330);
            this.lblLuminosita.Name = "lblLuminosita";
            this.lblLuminosita.Size = new System.Drawing.Size(106, 17);
            this.lblLuminosita.TabIndex = 5;
            this.lblLuminosita.Text = "Luminosità: 100%";

            // groupBoxPreset
            this.groupBoxPreset.Controls.Add(this.btnArancione);
            this.groupBoxPreset.Controls.Add(this.btnGiallo);
            this.groupBoxPreset.Controls.Add(this.btnBianco);
            this.groupBoxPreset.Controls.Add(this.btnBlu);
            this.groupBoxPreset.Controls.Add(this.btnVerde);
            this.groupBoxPreset.Controls.Add(this.btnRosso);
            this.groupBoxPreset.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.groupBoxPreset.Location = new System.Drawing.Point(350, 80);
            this.groupBoxPreset.Name = "groupBoxPreset";
            this.groupBoxPreset.Size = new System.Drawing.Size(200, 200);
            this.groupBoxPreset.TabIndex = 6;
            this.groupBoxPreset.TabStop = false;
            this.groupBoxPreset.Text = "Colori Preset";

            // btnRosso
            this.btnRosso.BackColor = System.Drawing.Color.Red;
            this.btnRosso.Location = new System.Drawing.Point(20, 30);
            this.btnRosso.Name = "btnRosso";
            this.btnRosso.Size = new System.Drawing.Size(70, 30);
            this.btnRosso.TabIndex = 0;
            this.btnRosso.Text = "Rosso";
            this.btnRosso.UseVisualStyleBackColor = false;
            this.btnRosso.Click += new System.EventHandler(this.btnRosso_Click);

            // btnVerde
            this.btnVerde.BackColor = System.Drawing.Color.Lime;
            this.btnVerde.Location = new System.Drawing.Point(110, 30);
            this.btnVerde.Name = "btnVerde";
            this.btnVerde.Size = new System.Drawing.Size(70, 30);
            this.btnVerde.TabIndex = 1;
            this.btnVerde.Text = "Verde";
            this.btnVerde.UseVisualStyleBackColor = false;
            this.btnVerde.Click += new System.EventHandler(this.btnVerde_Click);

            // btnBlu
            this.btnBlu.BackColor = System.Drawing.Color.Blue;
            this.btnBlu.ForeColor = System.Drawing.Color.White;
            this.btnBlu.Location = new System.Drawing.Point(20, 80);
            this.btnBlu.Name = "btnBlu";
            this.btnBlu.Size = new System.Drawing.Size(70, 30);
            this.btnBlu.TabIndex = 2;
            this.btnBlu.Text = "Blu";
            this.btnBlu.UseVisualStyleBackColor = false;
            this.btnBlu.Click += new System.EventHandler(this.btnBlu_Click);

            // btnBianco
            this.btnBianco.BackColor = System.Drawing.Color.White;
            this.btnBianco.Location = new System.Drawing.Point(110, 80);
            this.btnBianco.Name = "btnBianco";
            this.btnBianco.Size = new System.Drawing.Size(70, 30);
            this.btnBianco.TabIndex = 3;
            this.btnBianco.Text = "Bianco";
            this.btnBianco.UseVisualStyleBackColor = false;
            this.btnBianco.Click += new System.EventHandler(this.btnBianco_Click);

            // btnGiallo
            this.btnGiallo.BackColor = System.Drawing.Color.Yellow;
            this.btnGiallo.Location = new System.Drawing.Point(20, 130);
            this.btnGiallo.Name = "btnGiallo";
            this.btnGiallo.Size = new System.Drawing.Size(70, 30);
            this.btnGiallo.TabIndex = 4;
            this.btnGiallo.Text = "Giallo";
            this.btnGiallo.UseVisualStyleBackColor = false;
            this.btnGiallo.Click += new System.EventHandler(this.btnGiallo_Click);

            // btnArancione
            this.btnArancione.BackColor = System.Drawing.Color.Orange;
            this.btnArancione.Location = new System.Drawing.Point(110, 130);
            this.btnArancione.Name = "btnArancione";
            this.btnArancione.Size = new System.Drawing.Size(70, 30);
            this.btnArancione.TabIndex = 5;
            this.btnArancione.Text = "Arancione";
            this.btnArancione.UseVisualStyleBackColor = false;
            this.btnArancione.Click += new System.EventHandler(this.btnArancione_Click);

            // lblServerStatus
            this.lblServerStatus.AutoSize = true;
            this.lblServerStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.lblServerStatus.Location = new System.Drawing.Point(20, 400);
            this.lblServerStatus.Name = "lblServerStatus";
            this.lblServerStatus.Size = new System.Drawing.Size(145, 15);
            this.lblServerStatus.TabIndex = 7;
            this.lblServerStatus.Text = "🔴 MCP Server non attivo";

            // lblTitolo
            this.lblTitolo.AutoSize = true;
            this.lblTitolo.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Bold);
            this.lblTitolo.Location = new System.Drawing.Point(200, 20);
            this.lblTitolo.Name = "lblTitolo";
            this.lblTitolo.Size = new System.Drawing.Size(200, 29);
            this.lblTitolo.TabIndex = 8;
            this.lblTitolo.Text = "🔌 MCP Lampadina";

            // txtDebug
            this.txtDebug.BackColor = System.Drawing.Color.Black;
            this.txtDebug.ForeColor = System.Drawing.Color.Lime;
            this.txtDebug.Font = new System.Drawing.Font("Courier New", 8F);
            this.txtDebug.Location = new System.Drawing.Point(20, 430);
            this.txtDebug.Multiline = true;
            this.txtDebug.Name = "txtDebug";
            this.txtDebug.ReadOnly = true;
            this.txtDebug.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtDebug.Size = new System.Drawing.Size(560, 120);
            this.txtDebug.TabIndex = 9;
            this.txtDebug.Text = "[Debug MCP]\r\n";

            // LampadinaForm
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(240, 240, 240);
            this.ClientSize = new System.Drawing.Size(600, 580);
            this.Controls.Add(this.txtDebug);
            this.Controls.Add(this.lblTitolo);
            this.Controls.Add(this.lblServerStatus);
            this.Controls.Add(this.groupBoxPreset);
            this.Controls.Add(this.lblLuminosita);
            this.Controls.Add(this.trackBarLuminosita);
            this.Controls.Add(this.lblColore);
            this.Controls.Add(this.btnToggle);
            this.Controls.Add(this.lblStato);
            this.Controls.Add(this.panelLampadina);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "LampadinaForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "MCP Lampadina WinForms";
            this.Load += new System.EventHandler(this.LampadinaForm_Load);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.LampadinaForm_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.trackBarLuminosita)).EndInit();
            this.groupBoxPreset.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Panel panelLampadina;
        private System.Windows.Forms.Label lblStato;
        private System.Windows.Forms.Button btnToggle;
        private System.Windows.Forms.Label lblColore;
        private System.Windows.Forms.TrackBar trackBarLuminosita;
        private System.Windows.Forms.Label lblLuminosita;
        private System.Windows.Forms.GroupBox groupBoxPreset;
        private System.Windows.Forms.Button btnArancione;
        private System.Windows.Forms.Button btnGiallo;
        private System.Windows.Forms.Button btnBianco;
        private System.Windows.Forms.Button btnBlu;
        private System.Windows.Forms.Button btnVerde;
        private System.Windows.Forms.Button btnRosso;
        private System.Windows.Forms.Label lblServerStatus;
        private System.Windows.Forms.Label lblTitolo;
        private System.Windows.Forms.TextBox txtDebug;
    }
}