using System.Linq;
using Sofco.Domain.Enums;
using Sofco.Domain.Models.Billing;

namespace Sofco.Domain.Helpers
{
    public class SolfacHelper
    {
        public static bool IsCreditNote(Solfac solfac)
        {
            return new[] { SolfacDocumentType.CreditNoteA, SolfacDocumentType.CreditNoteB }.Contains(solfac.DocumentTypeId);
        }

        public static bool IsDebitNote(Solfac solfac)
        {
            return SolfacDocumentType.DebitNote == solfac.DocumentTypeId;
        }
    }
}
