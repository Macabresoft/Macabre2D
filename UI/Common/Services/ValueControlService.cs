namespace Macabresoft.Macabre2D.UI.Common;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using Macabresoft.AvaloniaEx;
using Macabresoft.Core;
using Macabresoft.Macabre2D.Framework;
using ReactiveUI;
using Unity;
using Unity.Resolution;

/// <summary>
/// An interface for a service which produces value editor controls given an object that contains a
/// <see cref="DataContractAttribute" />.
/// </summary>
public interface IValueControlService {
    /// <summary>
    /// Creates a single control collection for an object.
    /// </summary>
    /// <param name="owner">The owner.</param>
    /// <param name="name">The name.</param>
    /// <returns>The editor collection.</returns>
    ValueControlCollection CreateControl(object owner, string name);

    /// <summary>
    /// Creates value controls for an object based on its properties and fields that contain a
    /// <see cref="DataMemberAttribute" />.
    /// </summary>
    /// <param name="owner">The owner for which to make controls.</param>
    /// <param name="typesToIgnore">The types to ignore.</param>
    /// <returns>The value controls in collections split by their category.</returns>
    IReadOnlyCollection<ValueControlCollection> CreateControls(object owner, params Type[] typesToIgnore);

    /// <summary>
    /// Returns the controls and disposes them.
    /// </summary>
    /// <param name="controlCollections">The control collections to return.</param>
    void ReturnControls(params ValueControlCollection[] controlCollections);
}

/// <summary>
/// A service which produces value editor controls given an object that contains a <see cref="DataContractAttribute" />.
/// </summary>
public class ValueControlService : ReactiveObject, IValueControlService {
    private const string DefaultCategoryName = "Uncategorized";
    private const string DefaultCategoryNameForInfo = "Info";
    private readonly IAssemblyService _assemblyService;
    private readonly IUnityContainer _container;

    /// <summary>
    /// Initializes a new instance of the <see cref="ValueControlService" /> class.
    /// </summary>
    /// <param name="assemblyService">The assembly service.</param>
    /// <param name="container">The unity container.</param>
    public ValueControlService(IAssemblyService assemblyService, IUnityContainer container) {
        this._assemblyService = assemblyService;
        this._container = container;
    }

    /// <inheritdoc />
    public ValueControlCollection CreateControl(object owner, string name) {
        var editors = this.CreateControls(string.Empty, owner, owner);
        return new ValueControlCollection(editors, name);
    }

    /// <inheritdoc />
    public IReadOnlyCollection<ValueControlCollection> CreateControls(object owner, params Type[] typesToIgnore) {
        var valueEditors = this.CreateControls(string.Empty, owner, owner, typesToIgnore);
        return valueEditors.GroupBy(x => x.Category).Select(x => new ValueControlCollection(x, x.Key)).ToList();
    }

    /// <inheritdoc />
    public void ReturnControls(params ValueControlCollection[] controlCollections) {
        foreach (var editorCollection in controlCollections) {
            editorCollection.Dispose();
        }
    }

    private IReadOnlyCollection<IValueControl> CreateControls(string currentPath, object owner, object originalObject, params Type[] typesToIgnore) {
        var result = new List<IValueControl>();
        var editableObjectType = owner.GetType();
        var members = editableObjectType.GetAllFieldsAndProperties(typeof(DataMemberAttribute))
            .Where(x => typesToIgnore?.Contains(x.DeclaringType) == false && x.GetCustomAttribute<BrowsableAttribute>() is not { Browsable: false });

        var membersWithAttributes = members
            .Select(x => new AttributeMemberInfo<DataMemberAttribute>(x, x.GetCustomAttribute(typeof(DataMemberAttribute), false) as DataMemberAttribute))
            .OrderBy(x => x.Attribute.Order);

        if (this.TryCreateValueInfo(originalObject, owner, out var info)) {
            result.Add(info);
        }

        foreach (var member in membersWithAttributes) {
            var propertyPath = currentPath == string.Empty ? member.MemberInfo.Name : $"{currentPath}.{member.MemberInfo.Name}";
            var value = member.MemberInfo.GetValue(owner);
            var memberType = member.MemberInfo.GetMemberReturnType();

            var editors = this.CreateControlsForMember(owner, value, memberType, member, propertyPath);
            result.AddRange(editors);
        }

        return result;
    }

    private ICollection<IValueControl> CreateControlsForMember(
        object owner,
        object value,
        Type memberType,
        AttributeMemberInfo<DataMemberAttribute> member,
        string propertyPath) {
        var result = new List<IValueControl>();

        if (this._assemblyService.LoadFirstGenericType(typeof(IValueEditor<>), memberType) is { } memberEditorType) {
            var editor = this.CreateValueEditorFromType(memberEditorType, owner, value, memberType, member, propertyPath);
            if (editor != null) {
                result.Add(editor);
            }
        }
        else if (memberType.IsEnum) {
            if (memberType.GetCustomAttribute<FlagsAttribute>() != null) {
                var editor = this.CreateValueEditorFromType(typeof(FlagsEnumEditor), owner, value, memberType, member, propertyPath);
                if (editor != null) {
                    result.Add(editor);
                }
            }
            else {
                var editor = this.CreateValueEditorFromType(typeof(EnumEditor), owner, value, memberType, member, propertyPath);
                if (editor != null) {
                    result.Add(editor);
                }
            }
        }
        else if (memberType == typeof(Guid) && (owner is AssetReference && member.MemberInfo.Name == nameof(AssetReference.ContentId) || memberType.GetCustomAttribute<AssetGuidAttribute>() != null)) {
            var editor = this.CreateValueEditorFromType(typeof(AssetGuidEditor), owner, value, memberType, member, propertyPath);
            if (editor != null) {
                result.Add(editor);
            }
        }
        else if (memberType.IsAssignableTo(typeof(EntityReference))) {
            var editor = this.CreateValueEditorFromType(typeof(EntityReferenceEditor), owner, value, memberType, member, propertyPath);
            if (editor != null) {
                result.Add(editor);
            }
        }
        else if (memberType.IsAssignableTo(typeof(LoopReference))) {
            var editor = this.CreateValueEditorFromType(typeof(LoopReferenceEditor), owner, value, memberType, member, propertyPath);
            if (editor != null) {
                result.Add(editor);
            }
        }
        else if (!memberType.IsValueType) {
            var editors = this.CreateControls(propertyPath, value, owner);
            result.AddRange(editors);
        }

        return result;
    }

    private IValueEditor CreateValueEditorFromType(
        Type editorType,
        object owner,
        object value,
        Type memberType,
        AttributeMemberInfo<DataMemberAttribute> member,
        string propertyPath) {
        var dependencies = new ValueControlDependencies(value, memberType, GetPropertyName(propertyPath), GetTitle(member), owner);
        if (this._container.Resolve(editorType, new DependencyOverride(typeof(ValueControlDependencies), dependencies)) is IValueEditor editor) {
            editor.Category = DefaultCategoryName;

            if (member.MemberInfo.GetCustomAttribute(typeof(CategoryAttribute), false) is CategoryAttribute memberCategory) {
                editor.Category = memberCategory.Category;
            }
            else if (member.MemberInfo.DeclaringType != null) {
                if (Attribute.GetCustomAttribute(member.MemberInfo.DeclaringType, typeof(CategoryAttribute), false) is CategoryAttribute classCategory) {
                    editor.Category = classCategory.Category;
                }
                else {
                    editor.Category = owner.GetType().Name;
                }
            }

            return editor;
        }

        return null;
    }

    private static string GetPropertyName(string propertyPath) => !string.IsNullOrWhiteSpace(propertyPath) ? propertyPath.Split('.').Last() : propertyPath;

    private static string GetTitle(AttributeMemberInfo<DataMemberAttribute> member) => !string.IsNullOrEmpty(member.Attribute.Name) ? member.Attribute.Name : Regex.Replace(member.MemberInfo.Name, @"(\B[A-Z]+?(?=[A-Z][^A-Z])|\B[A-Z]+?(?=[^A-Z]))", " $1");

    private bool TryCreateValueInfo(object originalObject, object value, out IValueControl info) {
        info = null;

        if (value?.GetType() is { } ownerType && this._assemblyService.LoadFirstGenericType(typeof(IValueInfo<>), ownerType) is { } controlType && !controlType.IsAssignableTo(typeof(IValueEditor))) {
            var dependencies = new ValueControlDependencies(value, ownerType, null, DefaultCategoryNameForInfo, originalObject);

            if (this._container.Resolve(controlType, new DependencyOverride(typeof(ValueControlDependencies), dependencies)) is IValueControl control) {
                info = control;

                if (Attribute.GetCustomAttribute(ownerType, typeof(DataContractAttribute), false) is DataContractAttribute dataContractAttribute && !string.IsNullOrEmpty(dataContractAttribute.Name)) {
                    control.Category = $"{dataContractAttribute.Name} {DefaultCategoryNameForInfo}";
                }
                else {
                    control.Category = $"{DefaultCategoryNameForInfo} ({ownerType.Name})";
                }
            }
        }

        return info != null;
    }
}