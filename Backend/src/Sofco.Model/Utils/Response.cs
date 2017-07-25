using Sofco.Model.Enums;
using System.Collections.Generic;
using System.Linq;

namespace Sofco.Model.Utils
{
    public class Response<T> where T : class
    {
        public Response()
        {
            Messages = new List<Message>();
        }

        public T Data { get; set; }

        public IList<Message> Messages { get; set; }

        public bool HasErrors()
        {
            return Messages.Any(x => x.Type == MessageType.Error);
        }
    }
}
