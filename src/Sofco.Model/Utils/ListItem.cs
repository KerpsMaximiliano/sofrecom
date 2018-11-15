namespace Sofco.Domain.Utils
{
    public class ListItem<T> where T : class 
    {
        public int Id { get; set; }

        public string Text { get; set; }

        public T ExtraValue { get; set; }
    }
    
    public class ListItem<T1, T2> : ListItem<T1> 
        where T1 : class 
        where T2 : class
    {
        public T2 ExtraValue2 { get; set; }
    }
}