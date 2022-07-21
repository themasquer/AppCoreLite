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
    }
}
