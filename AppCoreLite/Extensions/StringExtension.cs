namespace AppCoreLite.Extensions
{
    public static class StringExtension
    {
        public static string FirstLetterToUpperOthersToLower(this string value, bool othersToLower = false)
        {
            string result = "";
            if (value.Length > 0)
            {
                char[] valueCharArray = value.ToCharArray();
                for (int i = 0; i < valueCharArray.Length; i++)
                {
                    if (i == 0)
                    {
                        valueCharArray[i] = valueCharArray[i].ToString().ToUpper()[0];
                    }
                    else
                    {
                        if (othersToLower)
                            valueCharArray[i] = valueCharArray[i].ToString().ToLower()[0];
                        else
                            valueCharArray[i] = valueCharArray[i].ToString()[0];
                    }
                }
                result = new string(valueCharArray);
            }
            return result;
        }

        public static string RemoveHtmlTags(this string value, string brTagSeperator = ", ")
        {
            string result = "";
            if (value != null)
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
    }
}
