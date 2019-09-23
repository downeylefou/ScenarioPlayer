using System;
using System.Collections.Generic;
using System.Linq;
using GubGub.Scripts.Enum;
using GubGub.Scripts.Lib.ResourceSetting;

namespace GubGub.Scripts.Command
{
    /// <summary>
    ///  シナリオコマンドのベースクラス
    /// </summary>
    public class BaseScenarioCommand
    {
        public EScenarioCommandType CommandType { get; private set; }

        protected List<string> rawParams;


        protected BaseScenarioCommand(
        EScenarioCommandType commandType = EScenarioCommandType.Unknown)
        {
            CommandType = commandType;
        }

        public void Dispose()
        {
            rawParams = null;
        }

        public void Initialize(List<string> rawParams)
        {
            Reset();

            this.rawParams = rawParams;
            MapParameters();
        }

        /// <summary>
        /// 設定シートのEntityからパラメータを更新する
        /// </summary>
        /// <param name="entity"></param>
        public virtual void UpdateParameter(IResourceSettingEntity entity)
        {
        }

        /// <summary>
        ///  パラメータ類を初期化する
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        protected virtual void Reset()
        {
            throw new NotImplementedException();
        }

        protected virtual void MapParameters()
        {
            throw new NotImplementedException();
        }

        protected bool GetBool(string key, bool defaultValue)
        {
            var findWord = new List<string>() {"true", "on"};
            var searchParam = GetParamStr(key, defaultValue).ToLower();

            return findWord.IndexOf(searchParam) != -1;
        }

        protected int GetInt(int key, int defaultValue)
        {
            int num;
            var numStr = GetParamStr(key, defaultValue);
            return int.TryParse(numStr, out num) ? num : defaultValue;
        }

        protected int GetInt(string key, int defaultValue)
        {
            int num;
            var numStr = GetParamStr(key, defaultValue);
            return int.TryParse(numStr, out num) ? num : defaultValue;
        }

        protected string GetString(string key, string defaultValue)
        {
            return GetParamStr(key, defaultValue);
        }

        protected string GetString(int key, string defaultValue)
        {
            return GetParamStr(key, defaultValue);
        }

        protected float GetFloat(int key, float defaultValue)
        {
            float num;
            var numStr = GetParamStr(key, defaultValue);
            return float.TryParse(numStr, out num) ? num : defaultValue;
        }

        protected float GetFloat(string key, float defaultValue)
        {
            float num;
            var numStr = GetParamStr(key, defaultValue);
            return float.TryParse(numStr, out num) ? num : defaultValue;
        }

        private string GetParamStr<T1, T2>(T1 key, T2 defaultValue)
        {
            if (key is int)
            {
                var keyNum = (int) (object) key;
                if (rawParams.Count > keyNum && rawParams[keyNum].Any())
                {
                    return rawParams[keyNum];
                }

                return defaultValue.ToString();
            }

            if (key is string)
            {
                var result = "";
                rawParams.ForEach(param =>
                {
                    if (param.IndexOf("=", StringComparison.Ordinal) != -1)
                    {
                        var splitted = param.Split('=');
                        if (splitted[0] == key.ToString())
                        {
                            result = splitted[1];
                        }
                    }
                });

                return result;
            }

            return defaultValue.ToString();
        }
    }
}