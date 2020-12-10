namespace MdDocGenerator.Util
{
    public class StringHelper
    {
        public static string XSubstring(string value,int start)
        {
            if (value.Length > start)
            {
                return value.Substring(start);
            }

            return value;
        }
    }
}