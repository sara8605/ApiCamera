using CameraApiTestTests;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CameraApiTest.Tests
{
    [TestClass]
    public class AuthorizationHelperTests
    {
        [TestMethod]
        public void GetResponseValueTest()
        {
            // Arrange
            var url = "http://192.168.1.230/cgi-bin/videoStatServer.cgi?action=attach&channel=1&heartbeat=5";

            var nonce = "395053096";
            var opaq = "cb6e076d2e92dc2e7076e8fccb97239afdb53c08";
            var nc = "00000001";

            // Act
            var response = AuthorizationHelper.GetResponseValue(new Uri(url), nonce: nonce, opaque: opaq, nonceCount: nc);

            // Assert
            var expected = "92e46ea7cdb6b4e4fc8c98c2a4aaafd1";
            Assert.AreEqual(expected, response);
        }

        [TestMethod]
        public void GetAuthorizationHaderTest()
        {
            // Arrange
            var url = "http://192.168.1.230/cgi-bin/videoStatServer.cgi?action=attach&channel=1&heartbeat=5";

            var nonce = "395053096";
            var opaq = "cb6e076d2e92dc2e7076e8fccb97239afdb53c08";
            var nc = "00000001";

            // Act
            var header = AuthorizationHelper.GetAuthorizationHader(new Uri(url), nonce: nonce, opaque: opaq, nonceCount: nc);

            // Assert
            var expected = "username=\"admin\", realm=\"Login to 5def96b9ee267a0c743c237c0a10ab3b\", nonce=\"395053096\", uri=\"/cgi-bin/videoStatServer.cgi?action=attach&channel=1&heartbeat=5\", algorithm=\"MD5\", qop=auth, nc=00000001, cnonce=\"21312\", response=\"92e46ea7cdb6b4e4fc8c98c2a4aaafd1\", opaque=\"cb6e076d2e92dc2e7076e8fccb97239afdb53c08\"";
            AssertHelpers.AreEqual(expected, header);
        }
    }
}
