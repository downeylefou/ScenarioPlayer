using System;
using System.Collections.Generic;
using System.Diagnostics;
using GubGub.Scripts.Command;

namespace GubGub.Scripts.Main
{
    /// <summary>
    /// シナリオの各種コマンドを保持するクラス
    /// コマンドクラスのインスタンスは取得時に無ければ作成され、使い回される
    /// </summary>
    public class CommandPalette
    {
        private readonly Dictionary<string, BaseScenarioCommand> _commands =
            new Dictionary<string, BaseScenarioCommand>();

        private readonly UnknownCommand _unknownCommand = new UnknownCommand();


        /// <summary>
        ///  コマンドタイプに応じたシナリオコマンドを取得する
        ///  無効なコマンドの場合、Unknownコマンドが返される
        /// </summary>
        /// <param name="commandName"></param>
        /// <returns></returns>
        public BaseScenarioCommand GetCommand(string commandName)
        {
            if (_commands.ContainsKey(commandName))
            {
                return _commands[commandName];
            }

            return CreateCommand(commandName) ?? _unknownCommand;
        }

        /// <summary>
        ///  コマンドを生成
        ///  "コマンドタイプ文字列 + Command" がクラス名となっている前提
        /// </summary>
        /// <param name="commandName"></param>
        /// <returns></returns>
        private BaseScenarioCommand CreateCommand(string commandName)
        {
            try
            {
                // コマンドタイプ文字列の 1文字目は大文字に
                var className = char.ToUpper(commandName[0]) + commandName.Substring(1) + "Command";
                var assemblyQualifiedName = typeof(BaseScenarioCommand).AssemblyQualifiedName;

                if (assemblyQualifiedName != null)
                {
                    var assemblyName = assemblyQualifiedName.Replace("BaseScenarioCommand", className);
                    var classType = Type.GetType(assemblyName);

                    Debug.Assert(classType != null, nameof(classType) + " != null");

                    var command = (BaseScenarioCommand) Activator.CreateInstance(classType);
                    _commands[commandName] = command;

                    return command;
                }
            }
            catch (Exception error)
            {
                UnityEngine.Debug.LogWarning(error.ToString());
            }

            return null;
        }
    }
}