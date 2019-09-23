using GubGub.Scripts.Command;
using GubGub.Scripts.Enum;
using GubGub.Scripts.Lib.ResourceSetting;

namespace GubGub.Scripts.Main
{
    /// <summary>
    /// コマンドのパラメータを設定シートの値で更新する
    /// </summary>
    public class CommandUpdater
    {
        /// <summary>
        /// コマンドのパラメータを設定シートのEntityで更新する
        /// </summary>
        /// <param name="command"></param>
        /// <param name="settingModel"></param>
        public void UpdateCommand(BaseScenarioCommand command, ScenarioResourceSettingModel settingModel)
        {
            if (command is StandCommand standCommand)
            {
                UpdateStandCommand(standCommand, settingModel);
            }
            else if (command is BgmCommand)
            {
            }
        }

        private void UpdateStandCommand(StandCommand command, ScenarioResourceSettingModel settingModel)
        {
            if (settingModel.GetSettingEntity(command.StandName,
                EResourceType.Character) is CharacterSheetEntity entity)
            {
                command.UpdateParameter(entity);
            }
        }
    }
}