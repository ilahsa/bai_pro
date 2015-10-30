//using System;
//using System.Text;
//using System.Collections.Generic;
//using System.Linq;
//using Microsoft.VisualStudio.TestTools.UnitTesting;

//using Imps.Services.CommonV4;

//namespace UnitTest
//{
//    /// <summary>
//    /// Summary description for TestClipRegion
//    /// </summary>
//    [TestClass]
//    public class TestClipRegion
//    {
//        public TestClipRegion()
//        {
//            //
//            // TODO: Add constructor logic here
//            //
//        }

//        private TestContext testContextInstance;

//        /// <summary>
//        ///Gets or sets the test context which provides
//        ///information about and functionality for the current test run.
//        ///</summary>
//        public TestContext TestContext
//        {
//            get
//            {
//                return testContextInstance;
//            }
//            set
//            {
//                testContextInstance = value;
//            }
//        }

//        #region Additional test attributes
//        //
//        // You can use the following additional attributes as you write your tests:
//        //
//        // Use ClassInitialize to run code before running the first test in the class
//        // [ClassInitialize()]
//        // public static void MyClassInitialize(TestContext testContext) { }
//        //
//        // Use ClassCleanup to run code after all tests in a class have run
//        // [ClassCleanup()]
//        // public static void MyClassCleanup() { }
//        //
//        // Use TestInitialize to run code before running each test 
//        // [TestInitialize()]
//        // public void MyTestInitialize() { }
//        //
//        // Use TestCleanup to run code after each test has run
//        // [TestCleanup()]
//        // public void MyTestCleanup() { }
//        //
//        #endregion

//        [TestMethod]
//        public void ClipRegion_Test()
//        {
//            Assert.AreEqual("CN.bj.", QMHelper.ClipRegion("CN.bj.10."));
//            Assert.AreEqual("CN.bj.", QMHelper.ClipRegion("CN.bj."));
//            Assert.AreEqual("CN.bj.", QMHelper.ClipRegion("CN.bj."));
//            Assert.AreEqual("CN.", QMHelper.ClipRegion("CN."));
//            Assert.AreEqual("HK.", QMHelper.ClipRegion("HK.aabb"));
//            Assert.AreEqual(QMHelper.ClipRegion("HK."), "HK.");
//            Assert.AreEqual(QMHelper.ClipRegion("HK."), "HK.");
//            Assert.AreEqual("XX.", QMHelper.ClipRegion("JP.AC.AB."));
//            Assert.AreEqual(QMHelper.ClipRegion(""), ".");
//            Assert.AreEqual(QMHelper.ClipRegion("."), ".");
//            Assert.AreEqual(QMHelper.ClipRegion(".."), ".");
//            Assert.AreEqual(QMHelper.ClipRegion("..."), ".");
//            Assert.AreEqual(".", QMHelper.ClipRegion("???"));
//            Assert.AreEqual(QMHelper.ClipRegion(null), ".");
//        }
//    }
//}
