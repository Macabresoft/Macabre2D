namespace Macabresoft.Macabre2D.UI.Common.Settings;

using System;
using Macabresoft.Macabre2D.Framework;

public class NamedSettingModel {
    public NamedSettingModel(NamedSetting setting) {
        this.Setting = setting;
    }

    public NamedSetting Setting { get; private set; }

    public Type SettingType {
        get => this.Setting.ValueType;
        set => this.Setting = this.InitializeNewSetting(value);
    }

    private NamedSetting InitializeNewSetting(Type newType) {
        var setting = this.Setting;
        if (newType != this.Setting.ValueType) {
            if (newType == typeof(bool)) {
                setting = new BoolSetting();
                setting.CopyInformationFromOther(this.Setting);
            }
            else {
                throw new NotSupportedException("Not a valid setting type, sorry!");
            }
        }

        return setting;
    }
}