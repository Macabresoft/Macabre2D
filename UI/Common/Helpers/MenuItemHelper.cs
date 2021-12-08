namespace Macabresoft.Macabre2D.UI.Common;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Avalonia.Controls;

/// <summary>
/// Helper for menu items.
/// </summary>
public static class MenuItemHelper {
    /// <summary>
    /// Creates a list of menu items to be used for "Add" menus.
    /// </summary>
    /// <param name="availableTypes">The available types.</param>
    /// <param name="showFindOption">A value indicating whether or not the "Find..." option should be shown.</param>
    /// <returns>A list of menu items with separators where appropriate.</returns>
    public static IReadOnlyCollection<IControl> CreateAddMenuItems(IReadOnlyCollection<Type> availableTypes, bool showFindOption) {
        var items = new List<IControl>();
        if (availableTypes?.Any() == true) {
            if (showFindOption) {
                items.Add(new MenuItem {
                    Header = "Find...",
                    Tag = "Open a window to find and select a type to add",
                    CommandParameter = null!
                });

                items.Add(new Separator());
            }

            items.AddRange(availableTypes.Select(CreateMenuItem));
        }

        return items;
    }

    private static MenuItem CreateMenuItem(Type type) {
        var menuItem = new MenuItem {
            Header = ToDisplayNameConverter.Instance.Convert(type, typeof(Type), null, CultureInfo.CurrentCulture),
            Tag = type.FullName,
            CommandParameter = type
        };

        return menuItem;
    }
}