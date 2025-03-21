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
using Macabresoft.Macabre2D.Common.Attributes;
using Macabresoft.Macabre2D.Framework;
using Macabresoft.Macabre2D.Project.Common;
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

        if (memberType.IsAssignableTo(typeof(Type)) && member.MemberInfo.GetCustomAttribute<TypeRestrictionAttribute>() is { } typeRestrictionAttribute) {
            if (this.CreateTypeEditor(owner, value, typeRestrictionAttribute.Type, member, propertyPath) is var editor) {
                result.Add(editor);
            }
        }
        else if (memberType == typeof(string)) {
            var isResource = member.MemberInfo.GetCustomAttribute<ResourceNameAttribute>() != null;
            var editor = this.CreateValueEditorFromType(isResource ? typeof(ResourceStringEditor) : typeof(StringEditor), owner, value, memberType, member, propertyPath);

            if (editor != null) {
                result.Add(editor);
            }
        }
        else if (memberType == typeof(int)) {
            IValueEditor editor;
            if (member.MemberInfo.GetCustomAttribute<PredefinedIntegerAttribute>() is { } predefinedIntegerAttribute) {
                editor = this.CreateValueEditorFromType(typeof(PredefinedIntEditor), owner, value, memberType, member, propertyPath, new DependencyOverride(typeof(PredefinedIntegerKind), predefinedIntegerAttribute.Kind));
            }
            else {
                editor = this.CreateValueEditorFromType(typeof(IntEditor), owner, value, memberType, member, propertyPath);
            }

            if (editor != null) {
                result.Add(editor);
            }
        }
        else if (memberType == typeof(Guid) && (owner is IAssetReference && member.MemberInfo.Name == nameof(SpriteSheet.ContentId) || HasAssetGuidReference(memberType))) {
            var editor = this.CreateValueEditorFromType(typeof(AssetGuidEditor), owner, value, memberType, member, propertyPath);
            if (editor != null) {
                editor.Title = $"Content Id ({propertyPath})";
                result.Add(editor);
            }
        }
        else if (memberType == typeof(Guid) && (owner is EntityReference && member.MemberInfo.Name == nameof(EntityReference.EntityId) || memberType.GetCustomAttribute<EntityGuidAttribute>() != null)) {
            var editor = this.CreateValueEditorFromType(typeof(EntityGuidEditor), owner, value, memberType, member, propertyPath);

            if (editor != null) {
                editor.Title = $"Entity Id ({propertyPath})";
                result.Add(editor);
            }
        }
        else if (this._assemblyService.LoadFirstGenericType(typeof(IValueEditor<>), memberType) is { } memberEditorType) {
            var editor = this.CreateValueEditorFromType(memberEditorType, owner, value, memberType, member, propertyPath);
            if (editor != null) {
                result.Add(editor);
            }
        }
        else if (memberType.IsAssignableTo(typeof(ISpriteSheetAssetReference))) {
            var editor = this.CreateValueEditorFromType(typeof(SpriteSheetAssetReferenceEditor), owner, value, memberType, member, propertyPath);
            if (editor != null) {
                result.Add(editor);
            }
        }
        else if (memberType.IsAssignableTo(typeof(IAssetReferenceCollection))) {
            var editor = this.CreateValueEditorFromType(typeof(AssetReferenceCollectionEditor), owner, value, memberType, member, propertyPath);
            if (editor != null) {
                result.Add(editor);
            }
        }
        else if (memberType.IsAssignableTo(typeof(IEntityReferenceCollection))) {
            var editor = this.CreateValueEditorFromType(typeof(EntityReferenceCollectionEditor), owner, value, memberType, member, propertyPath);
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
        else if (!memberType.IsValueType) {
            var editors = this.CreateControls(propertyPath, value, owner);
            result.AddRange(editors);
        }

        return result;
    }

    private TypeEditor CreateTypeEditor(
        object owner,
        object value,
        Type typeRestriction,
        AttributeMemberInfo<DataMemberAttribute> member,
        string propertyPath) {
        var dependencies = new ValueControlDependencies(value, typeRestriction, GetPropertyName(propertyPath), GetTitle(member), owner);
        var editor = this._container.Resolve<TypeEditor>(new DependencyOverride(typeof(ValueControlDependencies), dependencies));
        this.SetCategoryForEditor(editor, owner, member, propertyPath);
        return editor;
    }

    private IValueEditor CreateValueEditorFromType(
        Type editorType,
        object owner,
        object value,
        Type memberType,
        AttributeMemberInfo<DataMemberAttribute> member,
        string propertyPath,
        params ResolverOverride[] overrides) {
        var dependencies = new ValueControlDependencies(value, memberType, GetPropertyName(propertyPath), GetTitle(member), owner);
        var actualOverrides = overrides.Concat(new[] { new DependencyOverride(typeof(ValueControlDependencies), dependencies) }).ToArray();
        if (this._container.Resolve(editorType, actualOverrides) is IValueEditor editor) {
            this.SetCategoryForEditor(editor, owner, member, propertyPath);
            return editor;
        }

        return null;
    }

    private static string GetPropertyName(string propertyPath) => !string.IsNullOrWhiteSpace(propertyPath) ? propertyPath.Split('.').Last() : propertyPath;

    private static string GetTitle(AttributeMemberInfo<DataMemberAttribute> member) => !string.IsNullOrEmpty(member.Attribute.Name) ? member.Attribute.Name : Regex.Replace(member.MemberInfo.Name, @"(\B[A-Z]+?(?=[A-Z][^A-Z])|\B[A-Z]+?(?=[^A-Z]))", " $1");

    private static bool HasAssetGuidReference(Type type) =>
        type.GetCustomAttribute<AssetGuidAttribute>() != null ||
        type.GetCustomAttribute<SceneGuidAttribute>() != null ||
        type.GetCustomAttribute<SpriteSheetGuidAttribute>() != null ||
        type.GetCustomAttribute<PrefabGuidAttribute>() != null;

    private void SetCategoryForEditor(IValueEditor editor, object owner, AttributeMemberInfo<DataMemberAttribute> member, string propertyPath) {
        editor.Category = DefaultCategoryName;

        if (member.MemberInfo.GetCustomAttribute(typeof(CategoryAttribute), false) is CategoryAttribute memberCategory) {
            editor.Category = memberCategory.Category;
        }
        else if (member.MemberInfo.DeclaringType != null) {
            if (Attribute.GetCustomAttribute(member.MemberInfo.DeclaringType, typeof(CategoryAttribute), false) is CategoryAttribute classCategory) {
                editor.Category = classCategory.Category;
            }
            else if (GetPropertyName(propertyPath) is {} propertyName && !string.Equals(propertyName, propertyPath)) {
                editor.Category = propertyPath.Replace(propertyName, string.Empty).TrimEnd('.');
            }
            else {
                editor.Category = owner.GetType().Name;
            }
        }
    }

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