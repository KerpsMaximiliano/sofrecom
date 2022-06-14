using System;
using System.Collections.Generic;
using System.Text;

namespace Sofco.Core.Services.RequestNote
{
    public interface IRequestNoteAnalitycService
    {
        void CambiarAPendienteAprobacion(int id);
        void Rechazar(int id);
    }
}
