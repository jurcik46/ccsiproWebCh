namespace WebChromiumCcsipro.Domain.Messages
{
    public class ExecuteJavaScriptMessage
    {
        public string Function { get; set; }
        public string[] Parameters { get; set; }
    }
}