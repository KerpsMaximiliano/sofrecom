using Sofco.Model.Enums;

namespace Sofco.Model.Utils
{
    public class Message
    {
        public Message(string desc, MessageType type)
        {
            Description = desc;
            Type = type;
        }

        public string Description { get; set; }

        public MessageType Type { get; set; }
    }
}
