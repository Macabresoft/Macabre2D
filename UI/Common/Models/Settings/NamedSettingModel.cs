namespace Macabresoft.Macabre2D.UI.Common;

using System;
using Macabresoft.AvaloniaEx;
using Macabresoft.Core;
using Macabresoft.Macabre2D.Framework;

public sealed class NamedSettingModel : PropertyChangedNotifier {
    private readonly NamedSettingCollection _collection;
    private readonly IUndoService _undoService;

    public NamedSettingModel(NamedSetting setting, NamedSettingCollection collection, IUndoService undoService) {
        this.Setting = setting;
        this._collection = collection;
        this._undoService = undoService;
    }

    public SettingsCategory Category {
        get => this.Setting.Category;
        set {
            var originalValue = this.Setting.Category;
            if (value != originalValue) {
                this._undoService.Do(() =>
                {
                    this.Setting.Category = value;
                    this.RaisePropertyChanged();
                }, () =>
                {
                    this.Setting.Category = originalValue;
                    this.RaisePropertyChanged();
                });
            }
        }
    }

    public string Name {
        get => this.Setting.Name;
        set {
            var originalValue = this.Setting.Name;
            if (value != originalValue) {
                this._undoService.Do(() =>
                {
                    this.Setting.Name = value;
                    this.RaisePropertyChanged();
                }, () =>
                {
                    this.Setting.Name = originalValue;
                    this.RaisePropertyChanged();
                });
            }
        }
    }

    public NamedSetting Setting { get; private set; }

    public Type SettingType {
        get => this.Setting.ValueType;
        set {
            if (value != this.Setting.ValueType) {
                var originalValue = this.Setting;
                var newValue = this.InitializeNewSetting(value);
                newValue.CopyInformationFromOther(originalValue);

                this._undoService.Do(() =>
                {
                    this.Setting = newValue;
                    this._collection.Replace(originalValue, newValue);
                    this.RaiseAllProperties();
                }, () =>
                {
                    this.Setting = originalValue;
                    this._collection.Replace(newValue, originalValue);
                    this.RaiseAllProperties();
                });
            }
        }
    }

    public object Value {
        get => this.Setting.UntypedValue;
        set => this.SetValue(value);
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

    private void RaiseAllProperties() {
        this.RaisePropertyChanged(nameof(this.SettingType));
        this.RaisePropertyChanged(nameof(this.Setting));
        this.RaisePropertyChanged(nameof(this.Name));
        this.RaisePropertyChanged(nameof(this.Category));
    }

    private void SetValue(object value) {
        if (this.Setting is BoolSetting setting && value is bool boolValue) {
            var originalValue = setting.Value;
            this._undoService.Do(() =>
            {
                setting.Value = boolValue;
                this.RaisePropertyChanged();
            }, () => { setting.Value = originalValue; });
        }
    }
}