using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Imps.Services.CommonV4;

namespace BatchInsertTool
{
    public class DB
    {
        public static Database _proxyDb = DatabaseManager.GetDatabase("db");
        public static Database _testDb = DatabaseManager.GetDatabase("testdb");
        private static DB _instance = new DB();
        public static DB GetInstance
        {
            get { return _instance; }
        }
        private DB()
        {
        }



        public void AddT1(string uid, int locale, string time)
        {
            string[] spParams = {
                                    "@Uid","@Locale","@Time"
                                };

            object[] spValues = { uid, locale, time };
            try
            {
                var dr = _proxyDb.SpExecuteReader("usp_addt1", spParams, spValues);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }


        public void Test()
        {
            var logSql = "insert into apptasklog(taskid,uuid,version,locale,ip,`event`,addtime)values({0},{1},{2},{3},{4},'getapp',{5})";
            logSql = string.Format(logSql, 99, "uuid_009", "2.3", 111, "ip1", DateTime.Now.ToString());
            _testDb.SpExecuteNonQuery("test111", null, null);
            //fmt.Println(logSql)}
        }
    }
}
