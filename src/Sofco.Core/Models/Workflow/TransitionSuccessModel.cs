using System;
using Sofco.Domain.Utils;

namespace Sofco.Core.Models.Workflow
{
    public class TransitionSuccessModel
    {
        public bool MustDoNextTransition { get; set; }

        public int UserApplicantId { get; set; }

        public string UserName { get; set; }

        public Func<Response> OnError;
    }
}
