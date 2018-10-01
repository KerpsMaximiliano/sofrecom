using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using Sofco.Core.DAL;
using Sofco.Core.DAL.Admin;
using Sofco.Core.DAL.Rrhh;
using Sofco.Core.Logger;
using Sofco.Domain;
using Sofco.Domain.Models.Admin;
using Sofco.Domain.Models.Rrhh;
using Sofco.Service.Implementations.Rrhh;

namespace Sofco.UnitTest.Services.Rrhh
{
    [TestFixture]
    public class CloseDateServiceTest
    {
        private Mock<ILogMailer<CloseDateService>> loggerMock;
        private Mock<IUnitOfWork> unitOfWork;
        private Mock<ICloseDateRepository> closeDateRepositoryMock;
        private Mock<ISettingRepository> settingRepositoryMock;

        private CloseDateService sut;

        [SetUp]
        public void Setup()
        {
            unitOfWork = new Mock<IUnitOfWork>();
            loggerMock = new Mock<ILogMailer<CloseDateService>>();
            closeDateRepositoryMock = new Mock<ICloseDateRepository>();
            settingRepositoryMock = new Mock<ISettingRepository>();

            unitOfWork.Setup(x => x.CloseDateRepository).Returns(closeDateRepositoryMock.Object);
            unitOfWork.Setup(x => x.SettingRepository).Returns(settingRepositoryMock.Object);

            sut = new CloseDateService(unitOfWork.Object, loggerMock.Object);
        }

        [TestCase]
        public void ShouldAdd()
        {
            var data = new List<CloseDate>()
            {
                new CloseDate { Id = 1, Day = 10, Month = 9, Year = 2019 },
                new CloseDate { Id = 2, Day = 10, Month = 10, Year = 2019 },
                new CloseDate { Day = 10, Month = 11, Year = 2019 },
                new CloseDate { Day = 10, Month = 12, Year = 2019 },
            };

            var response = sut.Add(data);

            Assert.False(response.HasErrors());
            closeDateRepositoryMock.Verify(s => s.Insert(It.IsAny<CloseDate>()), Times.Exactly(2));
            closeDateRepositoryMock.Verify(s => s.Update(It.IsAny<CloseDate>()), Times.Exactly(2));
            unitOfWork.Verify(s => s.Save(), Times.Once);
        }

        [TestCase]
        public void ShouldNotAdd()
        {
            var data = new List<CloseDate>()
            {
                new CloseDate { Day = 10, Month = 5, Year = 2018 },
                new CloseDate { Day = 10, Month = 12, Year = 2017 }
            };

            sut.Add(data);

            closeDateRepositoryMock.Verify(s => s.Insert(It.IsAny<CloseDate>()), Times.Never);
            closeDateRepositoryMock.Verify(s => s.Update(It.IsAny<CloseDate>()), Times.Never);
            unitOfWork.Verify(s => s.Save(), Times.Once);
        }

        [TestCase]
        public void ShouldValidate()
        {
            var data = new List<CloseDate>()
            {
                new CloseDate { Day = 0, Month = 5, Year = 2018 },
                new CloseDate { Day = 30, Month = 12, Year = 2018 }
            };

            var response = sut.Add(data);

            Assert.True(response.HasErrors());
            closeDateRepositoryMock.Verify(s => s.Insert(It.IsAny<CloseDate>()), Times.Never);
            closeDateRepositoryMock.Verify(s => s.Update(It.IsAny<CloseDate>()), Times.Never);
            unitOfWork.Verify(s => s.Save(), Times.Never);
        }

        [TestCase]
        public void ShouldGet()
        {
            closeDateRepositoryMock.Setup(x => x.Get(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
                .Returns(new List<CloseDate>()
                {
                    new CloseDate { Day = 20, Month = 9, Year = 2018 },
                    new CloseDate { Day = 20, Month = 10, Year = 2018 },
                });

            settingRepositoryMock.Setup(x => x.GetByKey(SettingConstant.CloseMonthKey)).Returns(new Setting(){ Value = "15" });

            var response = sut.Get(9, 2018, 12, 2018);

            Assert.True(response.Data.Items.Count == 4);
            Assert.True(response.Data.Items.Count(x => x.Day == 15) == 2);
            Assert.True(response.Data.Items.Count(x => x.Day == 20) == 2);
        }
    }
}
