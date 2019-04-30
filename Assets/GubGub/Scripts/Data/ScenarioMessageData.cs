namespace GubGub.Scripts.Data
{
    /// <summary>
    /// メッセージ表示用パラメータクラス
    /// </summary>
    public class ScenarioMessageData
    {
        public string message;
        public string speakerName;
        public int messageSpeed;
        public bool isSkip;


        /// <summary>
        /// パラメータを設定する
        /// </summary>
        /// <param name="message"></param>
        /// <param name="speakerName"></param>
        /// <param name="messageSpeed"></param>
        /// <param name="isSkip"></param>
        public void SetParam(string message, string speakerName, int messageSpeed, bool isSkip)
        {
            this.message = message;
            this.speakerName = speakerName;
            this.messageSpeed = messageSpeed;
            this.isSkip = isSkip;
        }
    }
}
