using System.Text.Json;
using NAudio.Wave;
using Vosk;

namespace SpeechApp
{
    /// <summary>Tanınan bir konuşma sonucu.</summary>
    public record RecognitionResultInfo(DateTime Time, string Text, float Confidence);

    /// <summary>
    /// Mikrofondan gelen sesi Vosk konuşma tanıma motoruyla metne çevirir.
    /// Tamamen çevrimdışı çalışır; dil, Models klasöründeki modele göre belirlenir (ör. Türkçe).
    /// Ses NAudio ile 16 kHz, mono olarak alınır.
    /// </summary>
    public class SpeechToTextService : IDisposable
    {
        private const int SampleRate = 16000;

        private Model model;
        private string loadedModelPath;
        private VoskRecognizer recognizer;
        private WaveInEvent microphone;
        private readonly object recognizerLock = new object();

        public bool IsListening { get; private set; }

        /// <summary>Bir cümle tanındığında.</summary>
        public event EventHandler<RecognitionResultInfo> Recognized;
        /// <summary>Konuşma sürerken o ana kadar duyulan kısım (anlık geri bildirim).</summary>
        public event EventHandler<string> Hypothesis;
        /// <summary>Dinleme başladığında veya durduğunda.</summary>
        public event EventHandler ListeningChanged;
        /// <summary>Dinleme sırasında beklenmeyen bir hata olduğunda (ör. mikrofon çıkarıldı).</summary>
        public event EventHandler<string> Error;

        /// <summary>Uygulama klasöründeki Models altında bulunan model klasörleri.</summary>
        public static List<string> GetAvailableModels()
        {
            string dir = Path.Combine(AppContext.BaseDirectory, "Models");
            if (!Directory.Exists(dir))
                return new List<string>();
            return Directory.GetDirectories(dir).Select(Path.GetFileName).OrderBy(n => n).ToList();
        }

        /// <summary>
        /// Verilen modelle dinlemeyi başlatır. Model veya mikrofon yoksa açıklayıcı mesajlı
        /// InvalidOperationException fırlatır.
        /// </summary>
        public void Start(string modelName)
        {
            if (IsListening) return;

            string modelPath = modelName == null ? null : Path.Combine(AppContext.BaseDirectory, "Models", modelName);
            if (modelPath == null || !Directory.Exists(modelPath))
                throw new InvalidOperationException(
                    "Konuşma tanıma modeli bulunamadı.\n" +
                    "SpeechApp/Models klasöründe bir Vosk modeli (ör. vosk-model-small-tr-0.3) olmalıdır. README dosyasına bakın.");

            if (WaveInEvent.DeviceCount == 0)
                throw new InvalidOperationException(
                    "Mikrofon bulunamadı.\nMikrofonun takılı olduğundan ve Windows gizlilik ayarlarında " +
                    "masaüstü uygulamalarına mikrofon izni verildiğinden emin olun.");

            // Model büyük olduğu için bir kez yüklenir, sonraki dinlemelerde tekrar kullanılır
            if (model == null || loadedModelPath != modelPath)
            {
                model?.Dispose();
                Vosk.Vosk.SetLogLevel(-1);   // motorun konsola yazdığı ayrıntılı günlükleri kapat
                try
                {
                    model = new Model(modelPath);
                }
                catch (Exception ex)
                {
                    model = null;
                    throw new InvalidOperationException("Model dosyası okunamadı: " + ex.Message);
                }
                loadedModelPath = modelPath;
            }

            recognizer?.Dispose();
            recognizer = new VoskRecognizer(model, SampleRate);
            recognizer.SetWords(true);   // her kelime için güven değeri de gelsin

            microphone = new WaveInEvent
            {
                WaveFormat = new WaveFormat(SampleRate, 16, 1),
                BufferMilliseconds = 100
            };
            microphone.DataAvailable += Microphone_DataAvailable;
            microphone.RecordingStopped += Microphone_RecordingStopped;
            try
            {
                microphone.StartRecording();
            }
            catch (Exception ex)
            {
                microphone.Dispose();
                microphone = null;
                throw new InvalidOperationException("Mikrofon açılamadı: " + ex.Message);
            }

            IsListening = true;
            ListeningChanged?.Invoke(this, EventArgs.Empty);
        }

        public void Stop()
        {
            if (!IsListening) return;
            microphone?.StopRecording();   // RecordingStopped olayında son sonuç alınır
        }

        /// <summary>Mikrofondan her 100 ms'de bir gelen ses parçası tanıyıcıya verilir.</summary>
        private void Microphone_DataAvailable(object sender, WaveInEventArgs e)
        {
            lock (recognizerLock)
            {
                if (recognizer == null) return;
                if (recognizer.AcceptWaveform(e.Buffer, e.BytesRecorded))
                    RaiseResult(recognizer.Result());          // bir cümle tamamlandı
                else
                {
                    string partial = ReadString(recognizer.PartialResult(), "partial");
                    if (!string.IsNullOrEmpty(partial))
                        Hypothesis?.Invoke(this, partial);
                }
            }
        }

        private void Microphone_RecordingStopped(object sender, StoppedEventArgs e)
        {
            lock (recognizerLock)
            {
                if (recognizer != null)
                    RaiseResult(recognizer.FinalResult());     // yarım kalan son cümle
            }
            microphone.Dispose();
            microphone = null;
            IsListening = false;
            ListeningChanged?.Invoke(this, EventArgs.Empty);
            if (e.Exception != null)
                Error?.Invoke(this, "Mikrofon bağlantısı kesildi: " + e.Exception.Message);
        }

        /// <summary>Vosk'un JSON sonucunu okuyup metni ve ortalama güven değerini çıkarır.</summary>
        private void RaiseResult(string json)
        {
            using JsonDocument doc = JsonDocument.Parse(json);
            JsonElement root = doc.RootElement;
            string text = root.TryGetProperty("text", out var t) ? t.GetString() : "";
            if (string.IsNullOrWhiteSpace(text))
                return;

            float confidence = 1f;
            if (root.TryGetProperty("result", out var words) && words.GetArrayLength() > 0)
                confidence = (float)words.EnumerateArray().Average(w => w.GetProperty("conf").GetDouble());

            Recognized?.Invoke(this, new RecognitionResultInfo(DateTime.Now, text, confidence));
        }

        private static string ReadString(string json, string property)
        {
            using JsonDocument doc = JsonDocument.Parse(json);
            return doc.RootElement.TryGetProperty(property, out var p) ? p.GetString() : "";
        }

        public void Dispose()
        {
            if (microphone != null)
            {
                microphone.DataAvailable -= Microphone_DataAvailable;
                microphone.RecordingStopped -= Microphone_RecordingStopped;
                microphone.StopRecording();
                microphone.Dispose();
            }
            lock (recognizerLock)
            {
                recognizer?.Dispose();
                recognizer = null;
            }
            model?.Dispose();
        }
    }
}
