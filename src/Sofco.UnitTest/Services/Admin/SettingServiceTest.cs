using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using Sofco.Core.DAL.Admin;
using Sofco.Model.Models.Admin;
using Sofco.Service.Implementations.Admin;

namespace Sofco.UnitTest.Services.Admin
{
    [TestFixture]
    public class SettingServiceTest
    {
        private Mock<IGlobalSettingRepository> globalSettingRepository;

        private SettingService sut;

        [SetUp]
        public void Setup()
        {
            globalSettingRepository = new Mock<IGlobalSettingRepository>();

            globalSettingRepository.Setup(s => s.GetAll()).Returns(
                new List<GlobalSetting> {
                    new GlobalSetting { Id = 1, Key = "Key1" }
                });
            globalSettingRepository.Setup(s => s.Save(It.IsAny<List<GlobalSetting>>()));

            sut = new SettingService(globalSettingRepository.Object);
        }

        [Test]
        public void ShouldPassGetAll()
        {
            var actual = sut.GetAll();

            Assert.False(actual.HasErrors);

            Assert.NotNull(actual.Data);

            globalSettingRepository.Verify(s => s.GetAll(), Times.Once);
        }

        [Test]
        public void ShouldPassSave()
        {
            var globalSettings = new List<GlobalSetting>();

            var actual = sut.Save(globalSettings);

            Assert.False(actual.HasErrors);

            globalSettingRepository.Verify(s => s.Save(It.IsAny<List<GlobalSetting>>()), Times.Once);
        }
    }
}
