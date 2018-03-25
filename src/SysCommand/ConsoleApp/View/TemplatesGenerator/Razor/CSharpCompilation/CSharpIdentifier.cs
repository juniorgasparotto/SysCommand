using System.Globalization;
using System.Text;

namespace SysCommand.ConsoleApp.View.TemplatesGenerator.Razor
{
    internal static class CSharpIdentifier
    {
        public static string GetClassNameFromPath(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return path;
            }

            return SanitizeClassName(path);
        }

        // CSharp Spec §2.4.2
        private static bool IsIdentifierStart(char character)
        {
            return char.IsLetter(character) ||
                character == '_' ||
                CharUnicodeInfo.GetUnicodeCategory(character) == UnicodeCategory.LetterNumber;
        }

        public static bool IsIdentifierPart(char character)
        {
            return char.IsDigit(character) ||
                   IsIdentifierStart(character) ||
                   IsIdentifierPartByUnicodeCategory(character);
        }

        private static bool IsIdentifierPartByUnicodeCategory(char character)
        {
            var category = CharUnicodeInfo.GetUnicodeCategory(character);

            return category == UnicodeCategory.NonSpacingMark || // Mn
                category == UnicodeCategory.SpacingCombiningMark || // Mc
                category == UnicodeCategory.ConnectorPunctuation || // Pc
                category == UnicodeCategory.Format; // Cf
        }

        public static string GetFullNameWithoutGenerics<T>(bool returnFullName = true)
        {
            var t = typeof(T);
            int index = t.Name.IndexOf('`');
            var name = index == -1 ? t.Name : t.Name.Substring(0, index);
            if (returnFullName)
                return $"{t.Namespace}.{name}";
            return name;
        }

        public static string SanitizeClassName(string inputName)
        {
            if (!IsIdentifierStart(inputName[0]) && IsIdentifierPart(inputName[0]))
            {
                inputName = "_" + inputName;
            }

            var builder = new StringBuilder(inputName.Length);
            for (var i = 0; i < inputName.Length; i++)
            {
                var ch = inputName[i];
                builder.Append(IsIdentifierPart(ch) ? ch : '_');
            }

            return builder.ToString();
        }
    }
}
