namespace Sofco.Model.Utils
{
    public class ListItem<T> where T : class 
    {
        public T Id { get; set; }

        public string Text { get; set; }
    }
}