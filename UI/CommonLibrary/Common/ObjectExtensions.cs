namespace Macabre2D.UI.CommonLibrary.Common {

    using System;
    using System.Reflection;

    public static class ObjectExtensions {

        public static object GetProperty(this object baseObject, string pathToProperty) {
            var splitPath = pathToProperty.Split('.');
            var target = baseObject;
            MemberInfo member = null;

            foreach (var memberName in splitPath) {
                member = target.GetType().GetMemberInfoForFieldOrProperty(memberName);
                target = member.GetValue(target);
            }

            return target;
        }

        public static void SetProperty(this object baseObject, string pathToProperty, object newValue) {
            var splitPath = pathToProperty.Split('.');
            var target = baseObject;
            MemberInfo member = null;

            for (var i = 0; i < splitPath.Length; i++) {
                var memberName = splitPath[i];
                member = target.GetType().GetMemberInfoForFieldOrProperty(memberName);
                if (member != null && i < splitPath.Length - 1) {
                    target = member.GetValue(target);
                }
                else {
                    break;
                }
            }

            if (member != null) {
                member.SetValue(target, newValue);
            }
            else {
                throw new NotSupportedException("Invalid property path provided.");
            }
        }
    }
}