using Moq;
using NUnit.Framework;
using Sofco.Core.Data.AllocationManagement;
using Sofco.Core.DAL;
using Sofco.Core.DAL.Admin;
using Sofco.Core.DAL.AllocationManagement;
using Sofco.Core.DAL.Common;
using Sofco.Core.DAL.Rrhh;
using Sofco.Core.Logger;
using Sofco.Core.Mail;
using Sofco.Core.Managers;
using Sofco.Core.Models.Common;
using Sofco.Framework.Managers;

namespace Sofco.UnitTest.Framework.Managers
{
    [TestFixture]
    public class WorkTimeSendMailManagerTest
    {
        private const int ValidEmployeeId = 1;

        private Mock<IMailSender> mailSenderMock;

        private Mock<IEmployeeData> employeeDataMock;

        private Mock<IUnitOfWork> unitOfWorkMock;

        private Mock<ILogMailer<WorkTimeSendMailManager>> loggerMock;

        private Mock<ICloseDateRepository> closeDateRepositoryMock;

        private IWorkTimeSendMailManager sut;

        [SetUp]
        private void SetUp()
        {
            mailSenderMock = new Mock<IMailSender>();

            sut = new WorkTimeSendMailManager(mailSenderMock.Object,
                employeeDataMock.Object,
                unitOfWorkMock.Object,
                loggerMock.Object);

            closeDateRepositoryMock = new Mock<ICloseDateRepository>();

            unitOfWorkMock.SetupGet(s => s.CloseDateRepository).Returns(closeDateRepositoryMock.Object);

            closeDateRepositoryMock.Setup(s => s.GetBeforeCurrentAndNext())
                .Returns(new CloseDatesSettings(1, 1, 1));
        }

        [Test]
        public void ShouldPassSendEmail()
        {
            sut.SendEmail();
        }
    }
}
