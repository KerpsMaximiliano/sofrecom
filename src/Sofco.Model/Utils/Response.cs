using Sofco.Model.Enums;
using System.Collections.Generic;
using System.Linq;

namespace Sofco.Model.Utils
{
    public class Response<T> : Response where T : class
    {
        public T Data { get; set; }
    }

    public class Response
    {
        public Response()
        {
            Messages = new List<Message>();
        }

        public IList<Message> Messages { get; set; }

        public bool HasErrors()
        {
            return Messages.Any(x => x.Type == MessageType.Error);
        }

        public void AddError(string msg)
        {
            Messages.Add(new Message(msg, MessageType.Error));
        }

        public void AddSuccess(string msg)
        {
            Messages.Add(new Message(msg, MessageType.Success));
        }

        public void AddMessages(IList<Message> list)
        {
            foreach (var message in list)
                Messages.Add(message);
        }
    }
}
