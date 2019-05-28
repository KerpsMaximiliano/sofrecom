﻿using Microsoft.Extensions.Options;
using Sofco.Core.Config;
using Sofco.Core.DAL;
using Sofco.Core.FileManager;
using Sofco.Core.Services.Rrhh;
using Sofco.Framework.FileManager.Rrhh.Prepaids;

namespace Sofco.Service.Implementations.Rrhh
{
    public class PrepaidFactory : IPrepaidFactory
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly FileConfig fileConfig;

        public PrepaidFactory(IUnitOfWork unitOfWork, IOptions<FileConfig> fileOptions)
        {
            this.unitOfWork = unitOfWork;
            this.fileConfig = fileOptions.Value;
        }

        public IPrepaidFileManager GetInstance(string code)
        {
            switch (code)
            {
                case "SWISS": return new SwissPrepaidFileManager(unitOfWork);
                case "DOCTHOS": return new DocthosPrepaidFileManager(unitOfWork);
                case "OSDE": return new OsdePrepaidFileManager(unitOfWork);
                case "OMINT": return new OmintPrepaidFileManager(unitOfWork);
                case "MEDIFE": return new MedifePrepaidFileManager(unitOfWork);
                case "MEDICUS": return new MedicusPrepaidFileManager(unitOfWork);
                case "GALENO": return new GalenoPrepaidFileManager(unitOfWork);
                default: return null;
            }
        }
    }
}
