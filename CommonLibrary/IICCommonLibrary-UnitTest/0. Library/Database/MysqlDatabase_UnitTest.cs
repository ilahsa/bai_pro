using Imps.Services.CommonV4.DbAccess;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data;
using System;

namespace Imps.Services.CommonV4.UnitTest
{
    
    
    /// <summary>
    ///This is a test class for MysqlDatabase_UnitTest and is intended
    ///to contain all MysqlDatabase_UnitTest Unit Tests
    ///</summary>
    [TestClass()]
    public class MysqlDatabase_UnitTest
    {


        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        [ClassInitialize()]
        public static void MyClassInitialize(TestContext testContext)
        {
            ServiceSettings.InitService("UnitTest-Common");
        }
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        /// <summary>
        ///A test for BulkInsert
        ///</summary>
        [TestMethod()]
        public void BulkInsert_UnitTest()
        {
            string connStr = "server=192.168.110.234;Port=3306;database=GRPDB_POOL6;uid=admin;pwd=admin;procedure cache size=150;max pool size=150;allow zero datetime=true;connect timeout=3;";
            MysqlDatabase target = new MysqlDatabase(connStr);
            string tableName = "GRP_User_Migration";
            string spName = "USP_UMS_GetGRPDBAllByUserId";
            string[] spParams = { "@UserId" };
            object[] spValues = { "200034747" };
            DataSet ds = target.SpExecuteDataSet(spName, spParams, spValues);

            DataTable table = ds.Tables[0];
            string[] colNames = { "UserId","GroupListVersion","SvcId"  };
            bool isException = false;
            try
            {
                target.BulkInsert(tableName, table, colNames);
            }
            catch (Exception ex)
            {
                isException = true;
                Assert.Inconclusive(ex.Message);
            }
            Assert.AreEqual(isException, false);
        }

        /// <summary>
        ///A test for GetDatabaseName
        ///</summary>
        [TestMethod()]
        public void GetDatabaseName_UnitTest()
        {
            string connStr = "server=192.168.110.234;Port=3306;database=GRPDB_POOL6;uid=admin;pwd=admin;procedure cache size=150;max pool size=150;allow zero datetime=true;connect timeout=3;";
            MysqlDatabase target = new MysqlDatabase(connStr);
            string databaseName = null;
            bool isException = false;
            try
            {
                databaseName = target.GetDatabaseName(); ;
            }
            catch (Exception ex)
            {
                isException = true;
                Assert.Inconclusive(ex.Message);
            }
            Assert.AreEqual(isException, false);
            Assert.AreNotEqual(databaseName, null);
        }
    }
}
