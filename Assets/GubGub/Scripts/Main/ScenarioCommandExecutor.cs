using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GubGub.Scripts.Command;
using GubGub.Scripts.Enum;
using UniRx;
using UniRx.Async;
using UnityEngine.Events;

namespace GubGub.Scripts.Main
{
    /// <summary>
    /// シナリオコマンドの実行クラス
    /// </summary>
    public class ScenarioCommandExecutor
    {
        /// <summary>
        ///  コマンド処理の終了が通知されるストリーム
        /// </summary>
        public IObservable<Unit> CommandEnd => _commandEnd;

        private readonly Subject<Unit> _commandEnd = new Subject<Unit>();

        /// <summary>
        ///  現在実行中のコマンド
        /// </summary>
        private BaseScenarioCommand _currentCommand;

        /// <summary>
        ///  コマンドに対応した関数リスト
        /// </summary>
        private readonly Dictionary<EScenarioCommandType, Func<BaseScenarioCommand, Task>>
            _commandActions = new Dictionary<EScenarioCommandType, Func<BaseScenarioCommand, Task>>();


        /// <summary>
        /// コマンドを追加する
        /// </summary>
        /// <param name="commandType"></param>
        /// <param name="commandAction"></param>
        public void AddCommand(EScenarioCommandType commandType,
        Func<BaseScenarioCommand, Task> commandAction)
        {
            if (!_commandActions.ContainsKey(commandType))
            {
                _commandActions.Add(commandType, commandAction);
            }
        }

        /// <summary>
        ///  行をコマンドとして処理する
        /// </summary>
        /// <param name="command"></param>
        public async void ProcessCommand(BaseScenarioCommand command)
        {
            _currentCommand = command;

            await _commandActions[_currentCommand.CommandType].Invoke(_currentCommand);
            _commandEnd.OnNext(Unit.Default);
        }
    }
}