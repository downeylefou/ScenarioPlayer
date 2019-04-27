using System.Linq;
using System.Text;
using GubGub.Scripts.Enum;

namespace GubGub.Scripts.Command
{
    /// <inheritdoc />
    /// <summary>
    ///  メッセージ・話者名テキストの表示、ボイス再生用パラメータを持つ
    /// </summary>
    public class MessageCommand : BaseScenarioCommand
    {
        private readonly StringBuilder _messageBuilder = new StringBuilder();
        public string Message => _messageBuilder.ToString();

        public string VoiceName { get; private set; }
        public string SpeakerName { get; private set; }


        public MessageCommand() : base(EScenarioCommandType.Message)
        {
        }

        protected override void Reset()
        {
            _messageBuilder.Clear();
            VoiceName = "";
            SpeakerName = "";
        }

        protected sealed override void MapParameters()
        {
            VoiceName = rawParams[0];
            SpeakerName = rawParams[1];

            foreach (var message in rawParams.Skip(2))
            {
                _messageBuilder.Append(message).Append("\n");
            }
        }
    }
}
