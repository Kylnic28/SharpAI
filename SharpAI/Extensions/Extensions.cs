namespace SharpAI.Extensions
{
    public static class ObjectExtensions
    {
        public static List<KeyValuePair<string, string>> GetProperties(this object me)
        {
            List<KeyValuePair<string, string>> result = new List<KeyValuePair<string, string>>();
            foreach (var property in me.GetType().GetProperties())
            {
                result.Add(new KeyValuePair<string, string>(property.Name, Convert.ToString(property.GetValue(me))));
            }
            return result;
        }
    }
}
