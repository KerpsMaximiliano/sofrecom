using System;
using Sofco.Domain.Utils;

namespace Sofco.Core.Models.Workflow
{
    public class TransitionSuccessModel
    {
        public bool MustDoNextTransition { get; set; }

        public Func<Response> OnError;
    }
}
