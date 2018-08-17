using System.Collections.Generic;
using System.IO;
using System.Linq;
using Moq;
using NUnit.Framework;
using Sofco.Core.Data.Admin;
using Sofco.Core.DAL;
using Sofco.Core.DAL.Admin;
using Sofco.Core.DAL.AllocationManagement;
using Sofco.Core.DAL.WorkTimeManagement;
using Sofco.Core.Logger;
using Sofco.Core.Models.WorkTimeManagement;
using Sofco.Domain.Models.Admin;
using Sofco.Domain.Models.AllocationManagement;
using Sofco.Domain.Models.Rrhh;
using Sofco.Domain.Models.WorkTimeManagement;
using Sofco.Domain.Utils;
using Sofco.Framework.FileManager;

namespace Sofco.IntegrationTest.Worktime
{
    [TestFixture]
    public class WorkTimeFileManagerTest
    {
        private Mock<ITaskRepository> taskRepository;
        private Mock<ISettingRepository> settingRepository;
        private Mock<IEmployeeRepository> employeeRepository;
        private Mock<IWorkTimeRepository> workTimeRepository;
        private Mock<IUserRepository> userRepository;
        private Mock<IHolidayRepository> holidayRepository;

        private Mock<IUserData> userdata;
        private Mock<IUnitOfWork> unitOfWork;
        private Mock<ILogMailer<WorkTimeFileManager>> loggerMock;

        private WorkTimeFileManager sut;

        [SetUp]
        public void Setup()
        {
            taskRepository = new Mock<ITaskRepository>();
            settingRepository = new Mock<ISettingRepository>();
            employeeRepository = new Mock<IEmployeeRepository>();
            workTimeRepository = new Mock<IWorkTimeRepository>();
            userRepository = new Mock<IUserRepository>();
            holidayRepository = new Mock<IHolidayRepository>();

            unitOfWork = new Mock<IUnitOfWork>();
            userdata = new Mock<IUserData>();
            loggerMock = new Mock<ILogMailer<WorkTimeFileManager>>();

            unitOfWork.Setup(x => x.TaskRepository).Returns(taskRepository.Object);
            unitOfWork.Setup(x => x.SettingRepository).Returns(settingRepository.Object);
            unitOfWork.Setup(x => x.EmployeeRepository).Returns(employeeRepository.Object);
            unitOfWork.Setup(x => x.WorkTimeRepository).Returns(workTimeRepository.Object);
            unitOfWork.Setup(x => x.UserRepository).Returns(userRepository.Object);
            unitOfWork.Setup(x => x.HolidayRepository).Returns(holidayRepository.Object);

            taskRepository.Setup(x => x.GetAllIds()).Returns(new List<int> { 1 });

            settingRepository.Setup(x => x.GetByKey("WorkingHoursPerDaysMax")).Returns(new Setting
            {
                Value = "8"
            });

            settingRepository.Setup(x => x.GetByKey("CloseMonth")).Returns(new Setting
            {
                Value = "22"
            });

            holidayRepository.Setup(x => x.Get(It.IsAny<int>(), It.IsAny<int>())).Returns(new List<Holiday>());

            employeeRepository.Setup(x => x.GetByAnalyticWithWorkTimes(It.IsAny<int>()))
                .Returns(new List<Employee>
                {
                    new Employee
                    {
                        Id = 1,
                        EmployeeNumber = "830",
                        Name = "Hugo Alberto Basile",
                        WorkTimes = new List<WorkTime>(),
                        Licenses = new List<License>()
                    }
                });

            sut = new WorkTimeFileManager(unitOfWork.Object, loggerMock.Object, userdata.Object);
        }

        [TestCase]
        public void ImportWithErrors()
        {
            var currentDirectory = TestContext.CurrentContext.WorkDirectory;
            var filePath = $"{currentDirectory}\\Files\\worktime-template - prueba.xlsx";

            var file = File.ReadAllBytes(filePath);
            var memoryStream = new MemoryStream(file);

            var response = new Response<IList<WorkTimeImportResult>>();
            var analyticId = 82;

            sut.Import(analyticId, memoryStream, response);

            Assert.True(response.Data.Count == 10);
            Assert.True(response.Data.Count(x => x.Error == Resources.WorkTimeManagement.WorkTime.ImportDatesOutOfRange) == 1);
            Assert.True(response.Data.Count(x => x.Error == Resources.WorkTimeManagement.WorkTime.ImportDateNull) == 2);
            Assert.True(response.Data.Count(x => x.Error == Resources.WorkTimeManagement.WorkTime.ImportDateWrong) == 1);
            Assert.True(response.Data.Count(x => x.Error == Resources.WorkTimeManagement.WorkTime.ImportTaskNoExist) == 3);
            Assert.True(response.Data.Count(x => x.Error == Resources.WorkTimeManagement.WorkTime.ImportHoursExceed) == 1);

            unitOfWork.Verify(x => x.Save(), Times.Never);
        }
    }
}
