namespace Sofco.WebApi.Models
{
    public class Option
    {
        public Option(int value, string text)
        {
            Value = value;
            Text = text;
        }

        public int Value { get; set; }
        public string Text { get; set; }
    }
}
