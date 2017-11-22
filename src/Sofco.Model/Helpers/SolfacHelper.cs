using System.Linq;
using Sofco.Model.Enums;
using Sofco.Model.Models.Billing;

namespace Sofco.Model.Helpers
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
