using System;
using JetBrains.Annotations;

namespace Code.Model.Settings
{
    [Serializable]
    public class SettingsSerializeModel
    {
        [CanBeNull] public SettingsInfoModel.Setting<int> FpsValue;
        [CanBeNull] public SettingsInfoModel.Setting<string> WindowSize;
        [CanBeNull] public SettingsInfoModel.Setting<int> AllVolume;
        [CanBeNull] public SettingsInfoModel.Setting<int> VfxVolume;
        [CanBeNull] public SettingsInfoModel.Setting<int> MusicVolume;
        [CanBeNull] public SettingsInfoModel.Setting<bool> IsTextSlowed;
        [CanBeNull] public SettingsInfoModel.Setting<bool> LightFlicker;
    }
}