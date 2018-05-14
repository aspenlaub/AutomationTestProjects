using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Aspenlaub.Net.GitHub.CSharp.OneOutOfTwoInOne.Test {
    [TestClass]
    public class TestTest {
        [TestMethod]
        public void Succeeds() {
        }

        [TestMethod]
        public void Fails() {
            Assert.IsTrue(false, "false expected to be true");
        }
    }
}
