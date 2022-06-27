namespace B2B.API.Attributes
{
    public class ApiLogAttribute : Attribute
    {
        public ApiLogEvent Event { get; set; }

        public ApiLogAttribute(ApiLogEvent ev = ApiLogEvent.None)
        {
            Event = ev;
        }
    }

    /// <summary>
    /// 註記特殊事件定義
    /// </summary>
    public enum ApiLogEvent
    {
        None,
        Transaction
    }
}
