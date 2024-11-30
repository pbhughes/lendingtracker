namespace LendingTrackerApi.Extensions
{
    public static class ExceptionFlattner
    {
        public static string FlattenMessages(this Exception ex)
        {
            var messages = new List<string>();
            string result = string.Empty;
            while (ex != null)
            {
                messages.Add(ex.Message);
                ex = ex.InnerException;
            }
            foreach (var item in messages)
            {
                result += item + "/n";
            }
            return result;
        }
    }
}
