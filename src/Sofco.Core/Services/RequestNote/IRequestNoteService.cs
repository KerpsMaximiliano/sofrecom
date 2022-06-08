﻿using Sofco.Domain.DTO;
using Sofco.Domain.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sofco.Core.Services.RequestNote
{
    public interface IRequestNoteService
    {
        Response SaveBorrador(RequestNoteSubmitDTO dto);
        Response Get(int id);
        Response SavePendienteRevisionAbastecimiento(RequestNoteSubmitDTO dto);
    }
}
