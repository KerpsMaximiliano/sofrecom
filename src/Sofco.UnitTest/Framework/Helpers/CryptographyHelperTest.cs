using NUnit.Framework;
using Sofco.Framework.Helpers;

namespace Sofco.UnitTest.Framework.Helpers
{
    [TestFixture]
    public class CryptographyHelperTest
    {
        [Test]
        public void ShouldPassEncryptDecrypt()
        {
            const string secretWord = "Heelflip";

            var encrypted = CryptographyHelper.Encrypt(secretWord);

            var decrypted = CryptographyHelper.Decrypt(encrypted);

            Assert.AreEqual(decrypted, secretWord);
        }
    }
}
