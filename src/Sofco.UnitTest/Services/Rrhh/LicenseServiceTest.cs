using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using Sofco.Core.Config;
using Sofco.Core.Data.Admin;
using Sofco.Core.DAL;
using Sofco.Core.DAL.Admin;
using Sofco.Core.DAL.AllocationManagement;
using Sofco.Core.DAL.Rrhh;
using Sofco.Core.DAL.WorkTimeManagement;
using Sofco.Core.Logger;
using Sofco.Core.Mail;
using Sofco.Core.Managers.UserApprovers;
using Sofco.Core.Models.Admin;
using Sofco.Core.Models.Rrhh;
using Sofco.Core.Services.Rrhh;
using Sofco.Core.StatusHandlers;
using Sofco.Domain.Enums;
using Sofco.Domain.Models.Admin;
using Sofco.Domain.Models.AllocationManagement;
using Sofco.Domain.Models.Rrhh;
using Sofco.Domain.Models.WorkTimeManagement;
using Sofco.Framework.StatusHandlers.License;
using Sofco.Service.Implementations.Rrhh.Licenses;
using System.Linq;

namespace Sofco.UnitTest.Services.Rrhh
{
	[TestFixture]
	public class LicenseServiceTest
	{
		private Mock<IUnitOfWork> unitOfWork;
		private Mock<ILogMailer<LicenseService>> loggerMock;
		private Mock<IOptions<EmailConfig>> emailConfigMock;
		private Mock<ILicenseStatusFactory> licenseStatusFactory;
		private Mock<IMailSender> mailSender;
		private Mock<ILicenseApproverManager> licenseApproverManager;
		private Mock<ILicenseGenerateWorkTimeService> licenseGenerateWorkTimeService;
		private Mock<IUserData> userDataMock;

		private Mock<ILicenseRepository> licenseRepositoryMock;
		private Mock<IWorkTimeRepository> workTimeRepositoryMock;
		private Mock<IEmployeeRepository> employeeRepositoryMock;
		private Mock<IUserRepository> userRepositoryMock;
		private Mock<IGroupRepository> groupRepositoryMock;
		private Mock<IHolidayRepository> holidayRepositoryMock;
		private Mock<ICloseDateRepository> closeDateRepositoryMock;

		private LicenseService sut;

		[SetUp]
		public void Setup()
		{
			unitOfWork = new Mock<IUnitOfWork>();
			loggerMock = new Mock<ILogMailer<LicenseService>>();
			licenseStatusFactory = new Mock<ILicenseStatusFactory>();
			mailSender = new Mock<IMailSender>();
			licenseApproverManager = new Mock<ILicenseApproverManager>();
			licenseGenerateWorkTimeService = new Mock<ILicenseGenerateWorkTimeService>();
			emailConfigMock = new Mock<IOptions<EmailConfig>>();
			userDataMock = new Mock<IUserData>();

			licenseRepositoryMock = new Mock<ILicenseRepository>();
			workTimeRepositoryMock = new Mock<IWorkTimeRepository>();
			employeeRepositoryMock = new Mock<IEmployeeRepository>();
			userRepositoryMock = new Mock<IUserRepository>();
			groupRepositoryMock = new Mock<IGroupRepository>();
			holidayRepositoryMock = new Mock<IHolidayRepository>();
			closeDateRepositoryMock = new Mock<ICloseDateRepository>();

			unitOfWork.Setup(x => x.LicenseRepository).Returns(licenseRepositoryMock.Object);
			unitOfWork.Setup(x => x.WorkTimeRepository).Returns(workTimeRepositoryMock.Object);
			unitOfWork.Setup(x => x.EmployeeRepository).Returns(employeeRepositoryMock.Object);
			unitOfWork.Setup(x => x.UserRepository).Returns(userRepositoryMock.Object);
			unitOfWork.Setup(x => x.GroupRepository).Returns(groupRepositoryMock.Object);
			unitOfWork.Setup(x => x.HolidayRepository).Returns(holidayRepositoryMock.Object);
			unitOfWork.Setup(x => x.CloseDateRepository).Returns(closeDateRepositoryMock.Object);

			emailConfigMock.Setup(x => x.Value).Returns(new EmailConfig { SiteUrl = "SiteUrl", RrhhCode = "RRHH" });
			groupRepositoryMock.Setup(x => x.GetEmail(It.IsAny<string>())).Returns("rrhh@mail.com");
			licenseApproverManager.Setup(x => x.GetEmailApproversByEmployeeId(It.IsAny<int>())).Returns(new List<string> { "rrhh@mail.com" });
			workTimeRepositoryMock.Setup(s =>
					s.GetByEmployeeId(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<int>()))
				.Returns(new List<WorkTime>());

			userDataMock.Setup(x => x.GetCurrentUser()).Returns(new UserLiteModel { Id = 1, UserName = "username", Email = "asd@gmail.com" });

			sut = new LicenseService(unitOfWork.Object, loggerMock.Object,
				licenseStatusFactory.Object, mailSender.Object,
				licenseApproverManager.Object, licenseGenerateWorkTimeService.Object);

			closeDateRepositoryMock.Setup(s => s.GetFirstBeforeNextMonth())
				.Returns(new CloseDate { Day = 20, Month = 9, Year = 2021 });
		}

		[TestCase]
		public void ShouldAdd()
		{
			const int managerId = 2;
			const int employeeId = 1;

			employeeRepositoryMock.Setup(x => x.Exist(It.IsAny<int>())).Returns(true);
			employeeRepositoryMock.Setup(x => x.GetSingle(It.IsAny<Expression<Func<Employee, bool>>>())).Returns(new Employee() { Email = "asd@gmail.com" });
			employeeRepositoryMock.Setup(x => x.GetById(It.IsAny<int>())).Returns(new Employee { HolidaysPending = 10 });
			userRepositoryMock.Setup(x => x.ExistById(It.IsAny<int>())).Returns(true);
			userRepositoryMock.Setup(x => x.GetByEmail(It.IsAny<string>())).Returns(new User { Id = employeeId });
			licenseRepositoryMock.Setup(x => x.AreDatesOverlaped(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<int>())).Returns(false);
			licenseRepositoryMock.Setup(x => x.GetById(It.IsAny<int>())).Returns(new License
			{
				Status = LicenseStatus.Draft,
				StartDate = new DateTime(2025, 1, 6),
				EndDate = new DateTime(2025, 1, 12),
				Employee = new Employee(),
				Type = new LicenseType(),
				Manager = new User()
			});

			holidayRepositoryMock.Setup(x => x.Get(It.IsAny<int>(), It.IsAny<int>())).Returns(new List<Holiday>());

			licenseGenerateWorkTimeService.Setup(x => x.GenerateWorkTimes(It.IsAny<License>()));
			licenseStatusFactory.Setup(x => x.GetInstance(LicenseStatus.AuthPending)).Returns(new LicenseStatusAuthPendingHandler(emailConfigMock.Object.Value, licenseApproverManager.Object));

			var model = new LicenseAddModel
			{
				EmployeeId = employeeId,
				ManagerId = managerId,
				SectorId = 1,
				StartDate = new DateTime(2025, 1, 6),
				EndDate = new DateTime(2025, 1, 12),
				TypeId = 1,
				WithPayment = true,
				DaysQuantity = 7,
				UserId = 1,
				EmployeeLoggedId = 1
			};

			var response = sut.Add(model);

			Assert.False(response.HasErrors());
			licenseRepositoryMock.Verify(x => x.Insert(It.IsAny<License>()), Times.Once());
			unitOfWork.Verify(s => s.Save(), Times.Exactly(2));
			licenseGenerateWorkTimeService.Verify(x => x.GenerateWorkTimes(It.IsAny<License>()), Times.Once);
		}

		[TestCase]
		public void ShouldNotAdd()
		{
			const int managerId = 0;
			const int employeeId = 0;

			employeeRepositoryMock.Setup(x => x.Exist(It.IsAny<int>())).Returns(true);
			employeeRepositoryMock.Setup(x => x.GetSingle(It.IsAny<Expression<Func<Employee, bool>>>())).Returns(new Employee() { Email = "asd@gmail.com" });
			employeeRepositoryMock.Setup(x => x.GetById(It.IsAny<int>())).Returns(new Employee { HolidaysPending = 10 });
			userRepositoryMock.Setup(x => x.ExistById(It.IsAny<int>())).Returns(true);
			userRepositoryMock.Setup(x => x.GetByEmail(It.IsAny<string>())).Returns(new User { Id = employeeId });
			licenseRepositoryMock.Setup(x => x.AreDatesOverlaped(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<int>())).Returns(true);
			licenseRepositoryMock.Setup(x => x.GetById(It.IsAny<int>())).Returns(new License
			{
				Status = LicenseStatus.Draft,
				StartDate = new DateTime(2025, 1, 6),
				EndDate = new DateTime(2025, 1, 12)
			});

			licenseGenerateWorkTimeService.Setup(x => x.GenerateWorkTimes(It.IsAny<License>()));
			licenseStatusFactory.Setup(x => x.GetInstance(LicenseStatus.AuthPending)).Returns(new LicenseStatusAuthPendingHandler(emailConfigMock.Object.Value, licenseApproverManager.Object));

			var model = new LicenseAddModel
			{
				EmployeeId = employeeId,
				ManagerId = managerId,
				SectorId = 1,
				StartDate = new DateTime(2025, 1, 12),
				EndDate = new DateTime(2025, 1, 6),
				TypeId = 0,
				WithPayment = true,
				DaysQuantity = 7,
				UserId = 1,
				EmployeeLoggedId = 1
			};

			var response = sut.Add(model);

			Assert.True(response.HasErrors());
			Assert.True(response.Messages.Count == 5);
			licenseRepositoryMock.Verify(x => x.Insert(It.IsAny<License>()), Times.Never);
			unitOfWork.Verify(s => s.Save(), Times.Never);
			licenseGenerateWorkTimeService.Verify(x => x.GenerateWorkTimes(It.IsAny<License>()), Times.Never);
		}

		[TestCase]
		public void ShouldChangeStatusToAuthorize()
		{
			licenseRepositoryMock.Setup(x => x.GetById(It.IsAny<int>())).Returns(GetLicense(LicenseStatus.AuthPending, 2));
			licenseStatusFactory.Setup(x => x.GetInstance(LicenseStatus.Pending)).Returns(new LicenseStatusPendingHandler(emailConfigMock.Object.Value, licenseApproverManager.Object, userDataMock.Object));

			var model = new LicenseStatusChangeModel
			{
				IsRrhh = false,
				Status = LicenseStatus.Pending,
				UserId = 1
			};

			var response = sut.ChangeStatus(1, model, null);

			Assert.False(response.HasErrors());
			licenseRepositoryMock.Verify(x => x.UpdateStatus(It.IsAny<License>()), Times.Once());
			unitOfWork.Verify(s => s.Save(), Times.Once);
			mailSender.Verify(x => x.Send(It.IsAny<IMailData>()), Times.Once());
		}

		[TestCase]
		public void ShouldChangeStatusToApprovePending()
		{
			licenseRepositoryMock.Setup(x => x.GetById(It.IsAny<int>())).Returns(GetLicense(LicenseStatus.Pending, 2));
			licenseStatusFactory.Setup(x => x.GetInstance(LicenseStatus.ApprovePending)).Returns(new LicenseStatusApprovePendingHandler(emailConfigMock.Object.Value, licenseApproverManager.Object));

			var model = new LicenseStatusChangeModel
			{
				IsRrhh = true,
				Status = LicenseStatus.ApprovePending,
				UserId = 1
			};

			var response = sut.ChangeStatus(1, model, null);

			Assert.False(response.HasErrors());
			licenseRepositoryMock.Verify(x => x.UpdateStatus(It.IsAny<License>()), Times.Once());
			unitOfWork.Verify(s => s.Save(), Times.Once);
			mailSender.Verify(x => x.Send(It.IsAny<IMailData>()), Times.Once());
		}

		[TestCase]
		public void ShouldChangeStatusToApprove()
		{
			licenseRepositoryMock.Setup(x => x.GetById(It.IsAny<int>())).Returns(GetLicense(LicenseStatus.Pending, 2));
			licenseStatusFactory.Setup(x => x.GetInstance(LicenseStatus.Approved)).Returns(new LicenseStatusApproveHandler(emailConfigMock.Object.Value, licenseApproverManager.Object));

			var model = new LicenseStatusChangeModel
			{
				IsRrhh = true,
				Status = LicenseStatus.Approved,
				UserId = 1
			};

			var response = sut.ChangeStatus(1, model, null);

			Assert.False(response.HasErrors());

			licenseRepositoryMock.Setup(x => x.GetById(It.IsAny<int>())).Returns(GetLicense(LicenseStatus.Pending, 1));

			var model2 = new LicenseStatusChangeModel
			{
				IsRrhh = true,
				Status = LicenseStatus.Approved,
				UserId = 1
			};

			response = sut.ChangeStatus(1, model2, null);

			Assert.False(response.HasErrors());

			licenseRepositoryMock.Setup(x => x.GetById(It.IsAny<int>())).Returns(GetLicense(LicenseStatus.Pending, 7));

			var model3 = new LicenseStatusChangeModel
			{
				IsRrhh = true,
				Status = LicenseStatus.Approved,
				UserId = 1
			};

			response = sut.ChangeStatus(1, model3, null);

			Assert.False(response.HasErrors());
			licenseRepositoryMock.Verify(x => x.UpdateStatus(It.IsAny<License>()), Times.Exactly(3));
			unitOfWork.Verify(s => s.Save(), Times.Exactly(3));
			mailSender.Verify(x => x.Send(It.IsAny<IMailData>()), Times.Exactly(3));
		}

		[TestCase]
		public void ShouldChangeStatusToReject()
		{
			licenseRepositoryMock.Setup(x => x.GetById(It.IsAny<int>())).Returns(GetLicense(LicenseStatus.Pending, 2));
			licenseStatusFactory.Setup(x => x.GetInstance(LicenseStatus.Rejected)).Returns(new LicenseStatusRejectHandler(emailConfigMock.Object.Value, licenseApproverManager.Object, userDataMock.Object));
			licenseApproverManager.Setup(x => x.HasUserAuthorizer()).Returns(true);

			var model = new LicenseStatusChangeModel
			{
				IsRrhh = true,
				Status = LicenseStatus.Rejected,
				UserId = 1
			};

			var response = sut.ChangeStatus(1, model, null);

			Assert.False(response.HasErrors());
			licenseRepositoryMock.Verify(x => x.UpdateStatus(It.IsAny<License>()), Times.Once());
			unitOfWork.Verify(s => s.Save(), Times.Exactly(2));
			mailSender.Verify(x => x.Send(It.IsAny<IMailData>()), Times.Once());
		}

		private License GetLicense(LicenseStatus status, int type)
		{
			return new License
			{
				Status = status,
				StartDate = new DateTime(2025, 1, 6),
				EndDate = new DateTime(2025, 1, 12),
				Employee = new Employee { Name = "User Test", Email = "employee@mail.com", HolidaysPending = 10, ExamDaysTaken = 0 },
				Type = new LicenseType { Description = "description", Days = 10 },
				Manager = new User { Email = "manager@mail.com", Id = 1 },
				ManagerId = 1,
				EmployeeId = 1,
				TypeId = type,
				DaysQuantity = 5,
			};
		}
	}
}
