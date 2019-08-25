namespace GubGub.Scripts.Data
{
    /// <summary>
    /// シナリオで使用するパラメータクラス
    /// </summary>
    public class ScenarioParameter
    {
        private object _value;

        public T GetParameter<T>()
        {
            return (T) _value;
        }

        public void SetValue<T>(T value)
        {
            _value = value;
        }
    }
}