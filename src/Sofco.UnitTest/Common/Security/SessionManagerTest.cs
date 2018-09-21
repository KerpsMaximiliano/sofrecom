using System.Security.Claims;
using System.Security.Principal;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using Sofco.Common.Security;
using Sofco.Common.Security.Interfaces;
using Sofco.Common.Settings;

namespace Sofco.UnitTest.Common.Security
{
    public class SessionManagerTest
    {
        private const string Email = "user1@sofrecom.com.ar";

        private const string Domain = "sofrecom.com.ar";

        private readonly ISessionManager sut;

        private readonly Mock<IIdentity> identityMock;

        public SessionManagerTest()
        {
            var appSettingOption = new Mock<IOptions<AppSetting>>();

            appSettingOption.Setup(s => s.Value).Returns(new AppSetting
            {
                Domain = Domain
            });

            var contextAccessorMock = new Mock<IHttpContextAccessor>();

            var httpContextMock = new Mock<HttpContext>();

            var claimsPrincipalMock = new Mock<ClaimsPrincipal>();

            identityMock = new Mock<IIdentity>();

            claimsPrincipalMock.Setup(s => s.Identity).Returns(identityMock.Object);

            httpContextMock.Setup(s => s.User).Returns(claimsPrincipalMock.Object);

            contextAccessorMock.Setup(s => s.HttpContext).Returns(httpContextMock.Object);

            sut = new SessionManager(appSettingOption.Object, contextAccessorMock.Object);
        }

        [Test]
        public void ShouldPassGetUserName()
        {
            identityMock.Setup(s => s.Name).Returns(Email);

            var actual = sut.GetUserName();

            var expected = "user1";

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ShouldPassGetUserEmail()
        {
            identityMock.Setup(s => s.Name).Returns(Email);

            var actual = sut.GetUserEmail();

            Assert.AreEqual(Email, actual);
        }

        [Test]
        public void ShouldPassGetUserEmailWithParameter()
        {
            var actual = sut.GetUserEmail(Email);

            Assert.AreEqual(Email, actual);
        }
    }
}
