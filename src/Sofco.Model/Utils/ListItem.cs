namespace Sofco.Model.Utils
{
    public class ListItem<T> where T : class 
    {
        public int Id { get; set; }

        public string Text { get; set; }

        public T ExtraValue { get; set; }
    }
}