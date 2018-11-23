using Sofco.Domain.Enums;

namespace Sofco.Domain.Utils
{
    public class Message
    {
        public Message(string route, MessageType type)
        {
            var routeSplitted = route.Split('.');

            if (routeSplitted.Length > 0)
            {
                Folder = routeSplitted[0];

                if (routeSplitted.Length == 2)
                {
                    Code = routeSplitted[1];
                }
            }

            Type = type;
        }

        public string Folder { get; set; }

        public string Code { get; set; }

        public string Text { get; set; }

        public MessageType Type { get; set; }
    }
}
