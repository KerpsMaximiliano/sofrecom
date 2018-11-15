using System;
using Moq;
using NUnit.Framework;
using Sofco.Core.Data.Admin;
using Sofco.Core.DAL;
using Sofco.Core.DAL.WorkTimeManagement;
using Sofco.Core.Models.WorkTimeManagement;
using Sofco.Domain;
using Sofco.Domain.Models.Admin;
using Sofco.Domain.Utils;
using Sofco.Framework.Validations.WorkTimeManagement;

namespace Sofco.UnitTest.Framework.Validations.WorkTimeManagement
{
    public class WorkTimeValidationValidateHours
    {
        private WorkTimeValidation sut;

        private Mock<IUnitOfWork> unitOfWorkMock;

        private Mock<ISettingData> settingDataMock;

        private Mock<IWorkTimeRepository> workTimeRepositoryMock;

        [SetUp]
        public void Setup()
        {
            unitOfWorkMock = new Mock<IUnitOfWork>();

            settingDataMock = new Mock<ISettingData>();

            workTimeRepositoryMock = new Mock<IWorkTimeRepository>();

            unitOfWorkMock.SetupGet(s => s.WorkTimeRepository).Returns(workTimeRepositoryMock.Object);

            settingDataMock.Setup(s => s.GetByKey(SettingConstant.WorkingHoursPerDaysMaxKey))
                .Returns(new Setting
                {
                    Value = "8"
                });

            sut = new WorkTimeValidation(unitOfWorkMock.Object, settingDataMock.Object);
        }

        [Test]
        public void ShouldPassValidateHours()
        {
            workTimeRepositoryMock.Setup(s => s
                .GetTotalHoursByDateExceptCurrentId(It.IsAny<DateTime>(), It.IsAny<int>(), It.IsAny<int>())
            ).Returns(1);

            var model = new WorkTimeAddModel
            {
                Hours = 1
            };

            var response = new Response();

            sut.ValidateHours(response, model);

            Assert.False(response.HasErrors());
        }

        [Test]
        public void ShouldPassValidateHoursWithoutSetting()
        {
            workTimeRepositoryMock.Setup(s => s
                .GetTotalHoursByDateExceptCurrentId(It.IsAny<DateTime>(), It.IsAny<int>(), It.IsAny<int>())
            ).Returns(1);

            var model = new WorkTimeAddModel
            {
                Hours = 1
            };

            var response = new Response();

            settingDataMock.Setup(s => s.GetByKey(SettingConstant.WorkingHoursPerDaysMaxKey))
                .Returns((Setting)null);

            sut = new WorkTimeValidation(unitOfWorkMock.Object, settingDataMock.Object);

            sut.ValidateHours(response, model);

            Assert.False(response.HasErrors());
        }

        [Test]
        public void ShouldFailValidateHours()
        {
            var model = new WorkTimeAddModel();

            var response = new Response();

            sut.ValidateHours(response, model);

            Assert.True(response.HasErrors());
        }
    }
}
