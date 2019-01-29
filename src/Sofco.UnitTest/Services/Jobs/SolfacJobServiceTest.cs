using System.Collections.Generic;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using Sofco.Core.Config;
using Sofco.Core.DAL;
using Sofco.Core.DAL.Billing;
using Sofco.Core.Mail;
using Sofco.Domain.Crm;
using Sofco.Domain;
using Sofco.Service.Crm.Interfaces;
using Sofco.Service.Implementations.Jobs;
using Sofco.Service.Settings.Jobs;

namespace Sofco.UnitTest.Services.Jobs
{
    [TestFixture]
    public class SolfacJobServiceTest
    {
        const string Hitos1 = "Hitos1";
        const string Hitos2 = "Hitos2";
        const string ManagerMail = "manager@mail.com";

        private SolfacJobServiceTesteable sut;

        private Mock<ISolfacRepository> solfacRepositoryMock;
        private Mock<ICrmInvoicingMilestoneService> crmInvoiceServiceMock;
        private Mock<IMailBuilder> mailBuilderMock;
        private Mock<IMailSender> mailSenderMock;
        private Mock<IOptions<EmailConfig>> emailOptionsMock;
        private Mock<IOptions<JobSetting>> jobOptionsMock;

        private Mock<IUnitOfWork> unitOfWork;

        [SetUp]
        public void Setup()
        {
            solfacRepositoryMock = new Mock<ISolfacRepository>();
            crmInvoiceServiceMock = new Mock<ICrmInvoicingMilestoneService>();
            mailBuilderMock = new Mock<IMailBuilder>();
            mailSenderMock = new Mock<IMailSender>();
            emailOptionsMock = new Mock<IOptions<EmailConfig>>();
            jobOptionsMock = new Mock<IOptions<JobSetting>>();

            unitOfWork = new Mock<IUnitOfWork>();

            unitOfWork.Setup(x => x.SolfacRepository).Returns(solfacRepositoryMock.Object);

            emailOptionsMock.SetupGet(s => s.Value).Returns(new EmailConfig
            {
                SiteUrl = "sofcoar.com.ar"
            });

            jobOptionsMock.SetupGet(s => s.Value).Returns(new JobSetting
            {
                SolfacJob = new SolfacJobSetting { DaysToExpire = 5 }
            });

            sut = new SolfacJobServiceTesteable(unitOfWork.Object,
                crmInvoiceServiceMock.Object,
                mailBuilderMock.Object,
                mailSenderMock.Object,
                emailOptionsMock.Object,
                jobOptionsMock.Object);
        }

        [Test]
        public void ShouldPassGetEmailByHitos()
        {
            List<CrmHito> hitos = new List<CrmHito> {
                new CrmHito { ManagerMail = ManagerMail, Name = Hitos1 },
                new CrmHito { ManagerMail = ManagerMail, Name = Hitos2 },
            };

            var actual = sut.GetEmailByHitos(hitos);

            Assert.AreEqual(1, actual.Count);
        }
    }

    internal class SolfacJobServiceTesteable : SolfacJobService
    {
        public SolfacJobServiceTesteable(IUnitOfWork unitOfWork, 
            ICrmInvoicingMilestoneService crmInvoiceService,
            IMailBuilder mailBuilder,
            IMailSender mailSender, 
            IOptions<EmailConfig> emailConfigOptions, 
            IOptions<JobSetting> jobOptions) 
            : base(unitOfWork, 
                  crmInvoiceService, 
                  mailBuilder,
                  mailSender, 
                  emailConfigOptions, 
                  jobOptions)
        {
        }

        internal new List<Email> GetEmailByHitos(List<CrmHito> hitos)
        {
            return base.GetEmailByHitos(hitos);
        }
    }
}
