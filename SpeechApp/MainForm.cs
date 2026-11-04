namespace SpeechApp
{
    /// <summary>
    /// Ana pencere. Sol tarafta metni sesli okuma (TTS), sağ tarafta konuşmayı metne çevirme (STT) bulunur.
    /// Asıl işi TextToSpeechService ve SpeechToTextService sınıfları yapar; form yalnızca arayüzü yönetir.
    /// </summary>
    public partial class MainForm : Form
    {
        private readonly TextToSpeechService tts = new TextToSpeechService();
        private readonly SpeechToTextService stt = new SpeechToTextService();
        private readonly List<string> models = SpeechToTextService.GetAvailableModels();
        private int lastHighlightStart = -1, lastHighlightLength;

        public MainForm()
        {
            InitializeComponent();

            // Metin okuma: yüklü sesler
            foreach (string voice in tts.GetVoiceNames())
                cmbVoice.Items.Add(voice);
            if (cmbVoice.Items.Count > 0)
                cmbVoice.SelectedIndex = 0;

            // Konuşma tanıma: Models klasöründeki modeller
            foreach (string m in models)
                cmbCulture.Items.Add(m);
            if (models.Count == 0)
                cmbCulture.Items.Add("Model bulunamadı (README'ye bakın)");
            cmbCulture.SelectedIndex = 0;

            tts.StateChanged += (s, e) => RunOnUi(OnTtsStateChanged);
            tts.WordSpoken += (s, w) => RunOnUi(() => HighlightWord(w.Start, w.Length));
            stt.ListeningChanged += (s, e) => RunOnUi(UpdateStatus);
            stt.Hypothesis += (s, text) => RunOnUi(() => lblHypothesis.Text = "Algılanıyor: " + text);
            stt.Recognized += (s, r) => RunOnUi(() => AddRecognized(r));
            stt.Error += (s, msg) => RunOnUi(() =>
                MessageBox.Show(msg, "Konuşma tanıma hatası", MessageBoxButtons.OK, MessageBoxIcon.Warning));

            UpdateStatus();
        }

        // ---------- Metinden sese ----------

        private void BtnSpeak_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(rtbTts.Text))
            {
                MessageBox.Show("Okunacak bir metin yazın.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            try
            {
                ClearHighlight();
                tts.Speak(rtbTts.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Metin okunamadı. Hoparlör veya ses çıkışı kullanılamıyor olabilir.\n\n" + ex.Message,
                    "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnPause_Click(object sender, EventArgs e)
        {
            if (tts.State == TtsState.Speaking) tts.Pause();
            else if (tts.State == TtsState.Paused) tts.Resume();
        }

        private void BtnStopTts_Click(object sender, EventArgs e) => tts.Stop();

        private void BtnClearTts_Click(object sender, EventArgs e)
        {
            tts.Stop();
            rtbTts.Clear();
        }

        private void BtnSaveTts_Click(object sender, EventArgs e) => SaveText(rtbTts.Text, "tts_metin");

        private void CmbVoice_SelectedIndexChanged(object sender, EventArgs e) => tts.SelectVoice(cmbVoice.SelectedIndex);

        private void TrkRate_Scroll(object sender, EventArgs e)
        {
            tts.Rate = trkRate.Value;
            lblRate.Text = $"Hız: {trkRate.Value}";
        }

        private void TrkVolume_Scroll(object sender, EventArgs e)
        {
            tts.Volume = trkVolume.Value;
            lblVolume.Text = $"Ses: {trkVolume.Value}";
        }

        private void OnTtsStateChanged()
        {
            if (tts.State == TtsState.Idle)
                ClearHighlight();
            UpdateStatus();
        }

        /// <summary>O an okunan kelimeyi sarıyla işaretler.</summary>
        private void HighlightWord(int start, int length)
        {
            if (start + length > rtbTts.TextLength) return;
            ClearHighlight();
            rtbTts.Select(start, length);
            rtbTts.SelectionBackColor = Color.Gold;
            rtbTts.Select(start + length, 0);
            lastHighlightStart = start;
            lastHighlightLength = length;
        }

        private void ClearHighlight()
        {
            if (lastHighlightStart < 0 || lastHighlightStart + lastHighlightLength > rtbTts.TextLength) return;
            rtbTts.Select(lastHighlightStart, lastHighlightLength);
            rtbTts.SelectionBackColor = rtbTts.BackColor;
            rtbTts.Select(rtbTts.TextLength, 0);
            lastHighlightStart = -1;
        }

        // ---------- Sesten metne ----------

        private void BtnListen_Click(object sender, EventArgs e)
        {
            string model = models.Count > 0 ? models[cmbCulture.SelectedIndex] : null;
            try
            {
                Cursor = Cursors.WaitCursor;   // model ilk seferde birkaç saniyede yüklenir
                stt.Start(model);
                lblHypothesis.Text = "Konuşmaya başlayabilirsiniz...";
            }
            catch (Exception ex) when (ex is InvalidOperationException || ex is ArgumentException)
            {
                MessageBox.Show(ex.Message, "Konuşma tanıma başlatılamadı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        private void BtnStopStt_Click(object sender, EventArgs e)
        {
            stt.Stop();
            lblHypothesis.Text = "";
        }

        private void BtnClearStt_Click(object sender, EventArgs e)
        {
            rtbStt.Clear();
            lstStt.Items.Clear();
            lblHypothesis.Text = "";
        }

        private void BtnSaveStt_Click(object sender, EventArgs e)
        {
            string text = rtbStt.Text + Environment.NewLine + Environment.NewLine +
                          string.Join(Environment.NewLine, lstStt.Items.Cast<string>());
            SaveText(rtbStt.Text.Length == 0 ? "" : text, "stt_metin");
        }

        private void AddRecognized(RecognitionResultInfo r)
        {
            rtbStt.AppendText((rtbStt.TextLength > 0 ? " " : "") + r.Text);
            lstStt.Items.Add($"[{r.Time:HH:mm:ss}] (%{r.Confidence * 100:0}) {r.Text}");
            lstStt.TopIndex = lstStt.Items.Count - 1;
            lblHypothesis.Text = "Dinleniyor...";
        }

        // ---------- Ortak ----------

        /// <summary>Hangi işlemin aktif olduğunu alttaki şeritte gösterir ve butonları ayarlar.</summary>
        private void UpdateStatus()
        {
            string text;
            Color color;
            if (stt.IsListening && tts.State == TtsState.Speaking) { text = "Aktif: Metin okunuyor + Mikrofon dinleniyor"; color = Color.FromArgb(106, 27, 154); }
            else if (stt.IsListening) { text = "🎤 Aktif işlem: Mikrofon dinleniyor (konuşma metne çevriliyor)"; color = Color.FromArgb(21, 101, 192); }
            else if (tts.State == TtsState.Speaking) { text = "🔊 Aktif işlem: Metin sesli okunuyor"; color = Color.FromArgb(46, 125, 50); }
            else if (tts.State == TtsState.Paused) { text = "❚❚ Okuma duraklatıldı"; color = Color.FromArgb(245, 124, 0); }
            else { text = "Aktif işlem yok"; color = Color.FromArgb(120, 120, 120); }
            lblActive.Text = text;
            lblActive.BackColor = color;

            btnPause.Text = tts.State == TtsState.Paused ? "▶ Devam" : "❚❚ Duraklat";
            btnPause.Enabled = tts.State != TtsState.Idle;
            btnStopTts.Enabled = tts.State != TtsState.Idle;
            btnListen.Enabled = !stt.IsListening;
            btnStopStt.Enabled = stt.IsListening;
            // Basılamayan renkli butonlar gri görünsün
            btnSpeak.BackColor = Color.FromArgb(46, 125, 50);
            btnStopTts.BackColor = btnStopTts.Enabled ? Color.FromArgb(198, 40, 40) : Color.Silver;
            btnListen.BackColor = btnListen.Enabled ? Color.FromArgb(21, 101, 192) : Color.Silver;
            btnStopStt.BackColor = btnStopStt.Enabled ? Color.FromArgb(198, 40, 40) : Color.Silver;
            cmbCulture.Enabled = !stt.IsListening;
        }

        private void SaveText(string text, string prefix)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                MessageBox.Show("Kaydedilecek metin yok.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            saveFileDialog.FileName = $"{prefix}_{DateTime.Now:yyyyMMdd_HHmmss}.txt";
            if (saveFileDialog.ShowDialog(this) != DialogResult.OK) return;
            try
            {
                File.WriteAllText(saveFileDialog.FileName, text);
                MessageBox.Show("Dosya kaydedildi.", "Kaydedildi", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException)
            {
                MessageBox.Show("Dosya kaydedilemedi: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            tts.Dispose();
            stt.Dispose();
        }

        private void RunOnUi(Action action)
        {
            if (IsDisposed) return;
            if (InvokeRequired) BeginInvoke(action);
            else action();
        }
    }
}
