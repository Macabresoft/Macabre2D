namespace Macabre2D.UI.Models {

    using System.Linq;

    public static class FileStringExtensions {

        public static string ToSafeString(this string value) {
            if (value != null) {
                return new string(value.Where(x => char.IsLetterOrDigit(x)).ToArray());
            }

            return string.Empty;
        }
    }
}