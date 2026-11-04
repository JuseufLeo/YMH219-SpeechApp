using System.Speech.Synthesis;

namespace SpeechApp
{
    /// <summary>Metin okuma (Text-to-Speech) durumları.</summary>
    public enum TtsState { Idle, Speaking, Paused }

    /// <summary>
    /// Windows'un SpeechSynthesizer sınıfını sarmalar: metni sesli okur, duraklatır, devam ettirir, durdurur.
    /// Arayüzden bağımsızdır; durum değişikliklerini olaylarla bildirir.
    /// </summary>
    public class TextToSpeechService : IDisposable
    {
        private readonly SpeechSynthesizer synth = new SpeechSynthesizer();

        public TtsState State { get; private set; } = TtsState.Idle;

        /// <summary>Durum (okuyor / duraklatıldı / boşta) değiştiğinde.</summary>
        public event EventHandler StateChanged;
        /// <summary>Okunan kelime değiştiğinde: (başlangıç, uzunluk).</summary>
        public event EventHandler<(int Start, int Length)> WordSpoken;

        public TextToSpeechService()
        {
            synth.SetOutputToDefaultAudioDevice();
            synth.SpeakProgress += (s, e) => WordSpoken?.Invoke(this, (e.CharacterPosition, e.CharacterCount));
            synth.SpeakCompleted += (s, e) => SetState(TtsState.Idle);
        }

        /// <summary>Bilgisayarda yüklü seslerin adları.</summary>
        public List<string> GetVoiceNames() =>
            synth.GetInstalledVoices()
                 .Where(v => v.Enabled)
                 .Select(v => $"{v.VoiceInfo.Name} ({v.VoiceInfo.Culture.Name})")
                 .ToList();

        public void SelectVoice(int index)
        {
            var voices = synth.GetInstalledVoices().Where(v => v.Enabled).ToList();
            if (index >= 0 && index < voices.Count)
                synth.SelectVoice(voices[index].VoiceInfo.Name);
        }

        /// <summary>Okuma hızı: -10 (yavaş) ile 10 (hızlı) arası.</summary>
        public int Rate { get => synth.Rate; set => synth.Rate = Math.Clamp(value, -10, 10); }

        /// <summary>Ses düzeyi: 0 - 100.</summary>
        public int Volume { get => synth.Volume; set => synth.Volume = Math.Clamp(value, 0, 100); }

        public void Speak(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                throw new ArgumentException("Okunacak metin boş.");
            Stop();
            synth.SpeakAsync(text);
            SetState(TtsState.Speaking);
        }

        public void Pause()
        {
            if (State != TtsState.Speaking) return;
            synth.Pause();
            SetState(TtsState.Paused);
        }

        public void Resume()
        {
            if (State != TtsState.Paused) return;
            synth.Resume();
            SetState(TtsState.Speaking);
        }

        public void Stop()
        {
            if (State == TtsState.Idle) return;
            if (State == TtsState.Paused)
                synth.Resume();          // duraklatılmış konuşma önce devam ettirilmeli, sonra iptal edilir
            synth.SpeakAsyncCancelAll();
            SetState(TtsState.Idle);
        }

        private void SetState(TtsState state)
        {
            if (State == state) return;
            State = state;
            StateChanged?.Invoke(this, EventArgs.Empty);
        }

        public void Dispose() => synth.Dispose();
    }
}
