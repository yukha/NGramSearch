using System.Globalization;
using System.Text;

namespace NGramSearch
{
    public class SimpleNormalizer : INormalizer
    {
        public string Normalize(string raw)
        {
            raw = raw.Normalize(NormalizationForm.FormD).ToLower();
            StringBuilder sb = new StringBuilder();

            foreach (char t in raw)
            {
                if (CharUnicodeInfo.GetUnicodeCategory(t) != UnicodeCategory.NonSpacingMark)
                {
                    sb.Append(t);
                }
            }

            return System.Text.RegularExpressions.Regex.Replace(sb.ToString(), @"\s+", " ").Trim();
        }
    }
}
