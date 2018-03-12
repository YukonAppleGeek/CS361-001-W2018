using System.Globalization;

namespace StudyUp {
    public static class Extentions {
        public static string ToTitleCase(this string s)
        {
            return CultureInfo.InvariantCulture.TextInfo.ToTitleCase(s.ToLower());
        }
    } 
}