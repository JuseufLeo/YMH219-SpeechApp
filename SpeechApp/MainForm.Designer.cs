namespace SpeechApp
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            grpTts = new GroupBox();
            grpStt = new GroupBox();
            rtbTts = new RichTextBox();
            lblVoice = new Label();
            cmbVoice = new ComboBox();
            lblRate = new Label();
            trkRate = new TrackBar();
            lblVolume = new Label();
            trkVolume = new TrackBar();
            btnSpeak = new Button();
            btnPause = new Button();
            btnStopTts = new Button();
            btnClearTts = new Button();
            btnSaveTts = new Button();
            lblCulture = new Label();
            cmbCulture = new ComboBox();
            btnListen = new Button();
            btnStopStt = new Button();
            btnClearStt = new Button();
            btnSaveStt = new Button();
            lblHypothesis = new Label();
            rtbStt = new RichTextBox();
            lblHistory = new Label();
            lstStt = new ListBox();
            lblActive = new Label();
            saveFileDialog = new SaveFileDialog();
            SuspendLayout();
            //
            // grpTts
            //
            grpTts.Name = "grpTts";
            grpTts.Text = "Metinden Sese (Text-to-Speech)";
            grpTts.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold);
            grpTts.Location = new Point(12, 12);
            grpTts.Size = new Size(430, 520);
            //
            // grpStt
            //
            grpStt.Name = "grpStt";
            grpStt.Text = "Sesten Metne (Speech-to-Text)";
            grpStt.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold);
            grpStt.Location = new Point(456, 12);
            grpStt.Size = new Size(430, 520);
            //
            // rtbTts
            //
            rtbTts.Name = "rtbTts";
            rtbTts.Font = new Font("Segoe UI", 11F);
            rtbTts.Location = new Point(12, 26);
            rtbTts.Size = new Size(406, 230);
            rtbTts.Text = "Hello! This application was prepared for the Object Oriented Programming course. It can read your text aloud and convert your speech to text.";
            grpTts.Controls.Add(rtbTts);
            //
            // lblVoice
            //
            lblVoice.Name = "lblVoice";
            lblVoice.Text = "Ses:";
            lblVoice.Font = new Font("Segoe UI", 9F);
            lblVoice.AutoSize = true;
            lblVoice.Location = new Point(12, 271);
            grpTts.Controls.Add(lblVoice);
            //
            // cmbVoice
            //
            cmbVoice.Name = "cmbVoice";
            cmbVoice.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbVoice.Font = new Font("Segoe UI", 9F);
            cmbVoice.Location = new Point(80, 267);
            cmbVoice.Size = new Size(338, 23);
            cmbVoice.SelectedIndexChanged += CmbVoice_SelectedIndexChanged;
            grpTts.Controls.Add(cmbVoice);
            //
            // lblRate
            //
            lblRate.Name = "lblRate";
            lblRate.Text = "Hız: 0";
            lblRate.Font = new Font("Segoe UI", 9F);
            lblRate.AutoSize = true;
            lblRate.Location = new Point(12, 310);
            grpTts.Controls.Add(lblRate);
            //
            // trkRate
            //
            trkRate.Name = "trkRate";
            trkRate.Minimum = -10;
            trkRate.Maximum = 10;
            trkRate.Value = 0;
            trkRate.TickFrequency = 2;
            trkRate.Location = new Point(80, 300);
            trkRate.Size = new Size(338, 45);
            trkRate.Scroll += TrkRate_Scroll;
            grpTts.Controls.Add(trkRate);
            //
            // lblVolume
            //
            lblVolume.Name = "lblVolume";
            lblVolume.Text = "Ses: 100";
            lblVolume.Font = new Font("Segoe UI", 9F);
            lblVolume.AutoSize = true;
            lblVolume.Location = new Point(12, 360);
            grpTts.Controls.Add(lblVolume);
            //
            // trkVolume
            //
            trkVolume.Name = "trkVolume";
            trkVolume.Minimum = 0;
            trkVolume.Maximum = 100;
            trkVolume.Value = 100;
            trkVolume.TickFrequency = 10;
            trkVolume.Location = new Point(80, 350);
            trkVolume.Size = new Size(338, 45);
            trkVolume.Scroll += TrkVolume_Scroll;
            grpTts.Controls.Add(trkVolume);
            //
            // btnSpeak
            //
            btnSpeak.Name = "btnSpeak";
            btnSpeak.Text = "▶ Oku";
            btnSpeak.BackColor = Color.FromArgb(46, 125, 50);
            btnSpeak.ForeColor = Color.White;
            btnSpeak.FlatStyle = FlatStyle.Flat;
            btnSpeak.Location = new Point(12, 410);
            btnSpeak.Size = new Size(96, 36);
            btnSpeak.Click += BtnSpeak_Click;
            grpTts.Controls.Add(btnSpeak);
            //
            // btnPause
            //
            btnPause.Name = "btnPause";
            btnPause.Text = "❚❚ Duraklat";
            btnPause.Font = new Font("Segoe UI", 9F);
            btnPause.Location = new Point(114, 410);
            btnPause.Size = new Size(100, 36);
            btnPause.Click += BtnPause_Click;
            grpTts.Controls.Add(btnPause);
            //
            // btnStopTts
            //
            btnStopTts.Name = "btnStopTts";
            btnStopTts.Text = "■ Durdur";
            btnStopTts.BackColor = Color.FromArgb(198, 40, 40);
            btnStopTts.ForeColor = Color.White;
            btnStopTts.FlatStyle = FlatStyle.Flat;
            btnStopTts.Location = new Point(220, 410);
            btnStopTts.Size = new Size(96, 36);
            btnStopTts.Click += BtnStopTts_Click;
            grpTts.Controls.Add(btnStopTts);
            //
            // btnClearTts
            //
            btnClearTts.Name = "btnClearTts";
            btnClearTts.Text = "Temizle";
            btnClearTts.Font = new Font("Segoe UI", 9F);
            btnClearTts.Location = new Point(322, 410);
            btnClearTts.Size = new Size(96, 36);
            btnClearTts.Click += BtnClearTts_Click;
            grpTts.Controls.Add(btnClearTts);
            //
            // btnSaveTts
            //
            btnSaveTts.Name = "btnSaveTts";
            btnSaveTts.Text = "Metni .txt Olarak Kaydet";
            btnSaveTts.Font = new Font("Segoe UI", 9F);
            btnSaveTts.Location = new Point(12, 456);
            btnSaveTts.Size = new Size(406, 32);
            btnSaveTts.Click += BtnSaveTts_Click;
            grpTts.Controls.Add(btnSaveTts);
            //
            // lblCulture
            //
            lblCulture.Name = "lblCulture";
            lblCulture.Text = "Model:";
            lblCulture.Font = new Font("Segoe UI", 9F);
            lblCulture.AutoSize = true;
            lblCulture.Location = new Point(12, 30);
            grpStt.Controls.Add(lblCulture);
            //
            // cmbCulture
            //
            cmbCulture.Name = "cmbCulture";
            cmbCulture.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbCulture.Font = new Font("Segoe UI", 9F);
            cmbCulture.Location = new Point(62, 26);
            cmbCulture.Size = new Size(356, 23);
            grpStt.Controls.Add(cmbCulture);
            //
            // btnListen
            //
            btnListen.Name = "btnListen";
            btnListen.Text = "🎤 Dinlemeyi Başlat";
            btnListen.BackColor = Color.FromArgb(21, 101, 192);
            btnListen.ForeColor = Color.White;
            btnListen.FlatStyle = FlatStyle.Flat;
            btnListen.Location = new Point(12, 60);
            btnListen.Size = new Size(150, 36);
            btnListen.Click += BtnListen_Click;
            grpStt.Controls.Add(btnListen);
            //
            // btnStopStt
            //
            btnStopStt.Name = "btnStopStt";
            btnStopStt.Text = "■ Durdur";
            btnStopStt.BackColor = Color.FromArgb(198, 40, 40);
            btnStopStt.ForeColor = Color.White;
            btnStopStt.FlatStyle = FlatStyle.Flat;
            btnStopStt.Location = new Point(168, 60);
            btnStopStt.Size = new Size(80, 36);
            btnStopStt.Click += BtnStopStt_Click;
            grpStt.Controls.Add(btnStopStt);
            //
            // btnClearStt
            //
            btnClearStt.Name = "btnClearStt";
            btnClearStt.Text = "Temizle";
            btnClearStt.Font = new Font("Segoe UI", 9F);
            btnClearStt.Location = new Point(254, 60);
            btnClearStt.Size = new Size(76, 36);
            btnClearStt.Click += BtnClearStt_Click;
            grpStt.Controls.Add(btnClearStt);
            //
            // btnSaveStt
            //
            btnSaveStt.Name = "btnSaveStt";
            btnSaveStt.Text = "Kaydet";
            btnSaveStt.Font = new Font("Segoe UI", 9F);
            btnSaveStt.Location = new Point(336, 60);
            btnSaveStt.Size = new Size(82, 36);
            btnSaveStt.Click += BtnSaveStt_Click;
            grpStt.Controls.Add(btnSaveStt);
            //
            // lblHypothesis
            //
            lblHypothesis.Name = "lblHypothesis";
            lblHypothesis.Font = new Font("Segoe UI", 9F, FontStyle.Italic);
            lblHypothesis.ForeColor = Color.DimGray;
            lblHypothesis.Location = new Point(12, 104);
            lblHypothesis.Size = new Size(406, 20);
            grpStt.Controls.Add(lblHypothesis);
            //
            // rtbStt
            //
            rtbStt.Name = "rtbStt";
            rtbStt.Font = new Font("Segoe UI", 11F);
            rtbStt.Location = new Point(12, 128);
            rtbStt.Size = new Size(406, 190);
            grpStt.Controls.Add(rtbStt);
            //
            // lblHistory
            //
            lblHistory.Name = "lblHistory";
            lblHistory.Text = "Tanınan cümleler (zaman · güven):";
            lblHistory.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblHistory.AutoSize = true;
            lblHistory.Location = new Point(12, 326);
            grpStt.Controls.Add(lblHistory);
            //
            // lstStt
            //
            lstStt.Name = "lstStt";
            lstStt.Font = new Font("Consolas", 9F);
            lstStt.IntegralHeight = false;
            lstStt.HorizontalScrollbar = true;
            lstStt.Location = new Point(12, 348);
            lstStt.Size = new Size(406, 160);
            grpStt.Controls.Add(lstStt);
            //
            // lblActive
            //
            lblActive.Name = "lblActive";
            lblActive.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            lblActive.ForeColor = Color.White;
            lblActive.TextAlign = ContentAlignment.MiddleCenter;
            lblActive.Location = new Point(12, 540);
            lblActive.Size = new Size(874, 34);
            //
            // saveFileDialog
            //
            saveFileDialog.DefaultExt = "txt";
            saveFileDialog.Filter = "Metin dosyası (*.txt)|*.txt";
            //
            // MainForm
            //
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(898, 586);
            Controls.Add(grpTts);
            Controls.Add(grpStt);
            Controls.Add(lblActive);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            Name = "MainForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Speech-to-Text & Text-to-Speech - YMH219";
            FormClosing += MainForm_FormClosing;
            ResumeLayout(false);
        }

        private GroupBox grpTts;
        private GroupBox grpStt;
        private RichTextBox rtbTts;
        private Label lblVoice;
        private ComboBox cmbVoice;
        private Label lblRate;
        private TrackBar trkRate;
        private Label lblVolume;
        private TrackBar trkVolume;
        private Button btnSpeak;
        private Button btnPause;
        private Button btnStopTts;
        private Button btnClearTts;
        private Button btnSaveTts;
        private Label lblCulture;
        private ComboBox cmbCulture;
        private Button btnListen;
        private Button btnStopStt;
        private Button btnClearStt;
        private Button btnSaveStt;
        private Label lblHypothesis;
        private RichTextBox rtbStt;
        private Label lblHistory;
        private ListBox lstStt;
        private Label lblActive;
        private SaveFileDialog saveFileDialog;
    }
}
