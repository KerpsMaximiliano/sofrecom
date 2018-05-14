using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using Sofco.Core.Data.Admin;
using Sofco.Core.DAL;
using Sofco.Core.DAL.Admin;
using Sofco.Model.Models.Admin;
using Sofco.Service.Implementations.Admin;

namespace Sofco.UnitTest.Services.Admin
{
    [TestFixture]
    public class SettingServiceTest
    {
        private Mock<ISettingRepository> globalSettingRepository;
        private Mock<IUnitOfWork> unitOfWork;
        private Mock<ISettingData> settingDataMock;

        private SettingService sut;

        [SetUp]
        public void Setup()
        {
            globalSettingRepository = new Mock<ISettingRepository>();

            unitOfWork = new Mock<IUnitOfWork>();

            unitOfWork.Setup(x => x.SettingRepository).Returns(globalSettingRepository.Object);

            globalSettingRepository.Setup(s => s.GetAll()).Returns(
                new List<Setting> {
                    new Setting { Id = 1, Key = "Key1" }
                });
            globalSettingRepository.Setup(s => s.Save(It.IsAny<List<Setting>>()));

            settingDataMock = new Mock<ISettingData>();

            sut = new SettingService(unitOfWork.Object, settingDataMock.Object);
        }

        [Test]
        public void ShouldPassGetAll()
        {
            var actual = sut.GetAll();

            Assert.False(actual.HasErrors());

            Assert.NotNull(actual.Data);

            globalSettingRepository.Verify(s => s.GetAll(), Times.Once);
        }

        [Test]
        public void ShouldPassSave()
        {
            var globalSettings = new List<Setting>();

            var actual = sut.Save(globalSettings);

            Assert.False(actual.HasErrors());

            globalSettingRepository.Verify(s => s.Save(It.IsAny<List<Setting>>()), Times.Once);
        }
    }
}
