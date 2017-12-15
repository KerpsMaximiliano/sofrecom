using Sofco.Model.Enums;

namespace Sofco.Model.Utils
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

        public string Folder { get; private set; }

        public string Code { get; private set; }

        public MessageType Type { get; set; }
    }
}
