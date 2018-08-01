using System;
using System.Collections.Generic;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using Sofco.Common.Domains;
using Sofco.Core.Config;
using Sofco.Core.CrmServices;
using Sofco.Core.DAL;
using Sofco.Core.DAL.Billing;
using Sofco.Core.Mail;
using Sofco.Domain.Crm;
using Sofco.Domain;
using Sofco.Domain.Models.Billing;
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
        private Mock<ICrmInvoiceService> crmInvoiceServiceMock;
        private Mock<IMailBuilder> mailBuilderMock;
        private Mock<IMailSender> mailSenderMock;
        private Mock<IOptions<EmailConfig>> emailOptionsMock;
        private Mock<IOptions<JobSetting>> jobOptionsMock;

        private Mock<IUnitOfWork> unitOfWork;

        [SetUp]
        public void Setup()
        {
            solfacRepositoryMock = new Mock<ISolfacRepository>();
            crmInvoiceServiceMock = new Mock<ICrmInvoiceService>();
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
        public void ShouldPassSendHitosNotfications()
        {
            crmInvoiceServiceMock.Setup(s => s.GetHitosToExpire(It.IsAny<int>()))
                .Returns(new Result<List<CrmHito>>(new List<CrmHito> {
                    new CrmHito { Id = Guid.NewGuid(), ManagerMail = ManagerMail, Name = Hitos1 },
                    new CrmHito { Id = Guid.NewGuid(), ManagerMail = ManagerMail, Name = Hitos2 }
                }));

            solfacRepositoryMock.Setup(s => s.GetHitosByExternalIds(It.IsAny<List<Guid>>()))
                .Returns(new List<Hito> { new Hito { Id = 1, ExternalHitoId = Guid.NewGuid().ToString() } });

            mailSenderMock.Setup(s => s.Send(It.IsAny<List<Email>>()));

            sut.SendHitosNotifications();

            crmInvoiceServiceMock.Verify(s => s.GetHitosToExpire(It.IsAny<int>()), Times.Once);

            solfacRepositoryMock.Verify(s => s.GetHitosByExternalIds(It.IsAny<List<Guid>>()), Times.Once);

            mailSenderMock.Verify(s => s.Send(It.IsAny<List<Email>>()), Times.Once);
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
            ICrmInvoiceService crmInvoiceService,
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
