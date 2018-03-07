using System.Linq;
using NUnit.Framework;
using Sofco.Framework.ValidationHelpers.Admin;
using Sofco.Model.Models.Admin;

namespace Sofco.UnitTest.Framework.ValidationHelpers.Admin
{
    [TestFixture]
    public class SettingValidationHelperTest
    {
        [TestCase("1")]
        [TestCase("10")]
        [TestCase("20")]
        [TestCase("36")]
        public void ShouldPassValidateAllocationManagementMonths(string monthValue)
        {
            var setting = new GlobalSetting
            {
                Value = monthValue
            };

            var response = SettingValidationHelper.ValidateAllocationManagementMonths(setting);

            Assert.False(response.HasErrors());
        }

        [TestCase("0")]
        [TestCase("37")]
        [TestCase("45")]
        public void ShouldFailValidateAllocationManagementMonths(string monthValue)
        {
            var setting = new GlobalSetting
            {
                Value = monthValue
            };

            var response = SettingValidationHelper.ValidateAllocationManagementMonths(setting);

            Assert.True(response.HasErrors());

            var message = response.Messages.FirstOrDefault();

            Assert.NotNull(message);

            Assert.AreEqual(nameof(Resources.AllocationManagement.Analytic.WrongMonthQuantity).ToUpper(), message.Code.ToUpper());
        }
    }
}
