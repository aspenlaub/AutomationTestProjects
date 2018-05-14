using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Aspenlaub.Net.GitHub.CSharp.OneOutOfTwoInTwo.MoreTest {
    [TestClass]
    public class TestTest {
        [TestMethod]
        public void Fails() {
            Assert.IsTrue(false, "false expected to be true");
        }
    }
}
