namespace AppCoreLite.Extensions
{
    public static class StringExtension
    {
        public static string FirstLetterToUpperOthersToLower(this string value, bool othersToLower = false)
        {
            string result = "";
            if (!string.IsNullOrWhiteSpace(value))
            {
                result = value.Substring(0, 1).ToUpper();
                if (value.Length > 1)
                    result += value.Substring(1).ToLower();
            }
            return result;
        }

        public static string RemoveHtmlTags(this string value, string brTagSeperator = ", ")
        {
            string result = "";
            if (!string.IsNullOrWhiteSpace(value))
            {
                value = value.Replace("&nbsp;", " ").Replace("<br>", brTagSeperator).Replace("<br />", brTagSeperator).Replace("<br/>", brTagSeperator)
                    .Replace("&amp;", "&").Trim();
                char[] array = new char[value.Length];
                int arrayIndex = 0;
                bool inside = false;
                for (int i = 0; i < value.Length; i++)
                {
                    char let = value[i];
                    if (let == '<')
                    {
                        inside = true;
                        continue;
                    }
                    if (let == '>')
                    {
                        inside = false;
                        continue;
                    }
                    if (!inside)
                    {
                        array[arrayIndex] = let;
                        arrayIndex++;
                    }
                }
                result = new string(array, 0, arrayIndex);
            }
            return result;
        }

        public static string ChangeTurkishCharactersToEnglish(this string turkishValue)
        {
			string englishValue = "";
            if (!string.IsNullOrWhiteSpace(turkishValue))
            {
                Dictionary<string, string> characterDictionary = new Dictionary<string, string>()
                {
                    { "Ö", "O" },
                    { "Ç", "C" },
                    { "Ş", "S" },
                    { "Ğ", "G" },
                    { "Ü", "U" },
                    { "ö", "o" },
                    { "ç", "c" },
                    { "ş", "s" },
                    { "ğ", "g" },
                    { "ü", "u" },
                    { "İ", "I" },
                    { "ı", "i" }
                };
                foreach (char character in turkishValue)
                {
                    if (characterDictionary.ContainsKey(character.ToString()))
                        englishValue += characterDictionary[character.ToString()];
                    else
                        englishValue += character.ToString();
                }
            }
            return englishValue;
		}
    }
}
