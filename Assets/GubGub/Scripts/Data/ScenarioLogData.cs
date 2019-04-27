using System;

namespace GubGub.Scripts.Data
{
    /// <summary>
    ///  シナリオバックログのリストデータ
    /// </summary>
    public class ScenarioLogData
    {
        public int Id { get; }
        public string SpeakerName { get; private set; }
        public string Message { get; private set; }
        public string VoicePath { get; private set; }

        public IObserver<string> PlayVoicePathStream { get; private set; }

        public ScenarioLogData(IObserver<string> playVoicePathStream,
                               string speakerName, string message, string voicePath)
        {
            PlayVoicePathStream = playVoicePathStream;
            SpeakerName = speakerName;
            Message = message;
            VoicePath = voicePath;
        }
    }
}
