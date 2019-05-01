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

        public IObserver<string> PlayVoiceStream { get; private set; }

        public ScenarioLogData(IObserver<string> playVoiceStream,
                               string speakerName, string message, string voicePath)
        {
            PlayVoiceStream = playVoiceStream;
            SpeakerName = speakerName;
            Message = message;
            VoicePath = voicePath;
        }
    }
}
