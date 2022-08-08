using Sofco.Domain.Enums;

namespace Sofco.Domain.Utils
{
    public class Message
    {
        public Message(string route, MessageType type, bool mustTranslate)
        {
            Translate = mustTranslate;

            SetMessage(route, type);
        }

        public Message(string route, MessageType type)
        {
            Translate = false;

            SetMessage(route, type);
        }

        private void SetMessage(string route, MessageType type)
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

            Text = route;
            Type = type;
        }

        public string Folder { get; set; }

        public string Code { get; set; }

        public string Text { get; set; }

        public MessageType Type { get; set; }

        public bool Translate { get; set; }
    }
}
