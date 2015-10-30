using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace Imps.Services.CommonV4.Dtc
{
    public class TccDatabasePersister : TccPersister
    {
        protected Database _db;
        public TccDatabasePersister(string transName, Database db, TccPersisterMode mode)
            : base(transName, mode)
        {
            _db = db;
        }

        /// <summary>
        /// 查询出所有没有Confirmed的Transaction
        /// </summary>
        /// <returns></returns>
        public override IList<TccTransactionData> LoadFailedTransaction()
        {
            string spName = "USP_TCC_GetTransactionLog";
            string[] spParams = { "@TxSchema", "@TxState" };
            object[] spValues = { _transName, TccTransactionState.Confirmed };
            DataTable dt = _db.SpExecuteTable(spName, spParams, spValues);
            if (dt != null && dt.Rows.Count > 0)
            {
                List<TccTransactionData> list = new List<TccTransactionData>();
                foreach (DataRow dr in dt.Rows)
                {
                    TccTransactionData data = ConvertDataRowToTccTransactionData(dr);
                    list.Add(data);
                }
                return list;
            }
            else
                return null;
        }

        private TccTransactionData ConvertDataRowToTccTransactionData(DataRow dr)
        {
            TccTransactionData data = new TccTransactionData();
            data.TxId = dr["TxId"].ToString();
            data.ServiceAtComputer = dr["ServiceAtComputer"].ToString();
            data.SchemaName = dr["TxSchema"].ToString();
            data.BeginTime = Convert.ToDateTime(dr["BeginTime"]);
            data.TxState = (TccTransactionState)dr["TxState"];
            string temp = dr["WorkState"].ToString();
            string[] strs = temp.Split(new string[] { "," }, StringSplitOptions.None);
            TccWorkState[] states = new TccWorkState[strs.Length];
            for (int i = 0; i < strs.Length; i++)
            {
                states[i] = (TccWorkState)Convert.ToInt32(strs[i]);
            }
            data.WorkStates = states;
            data.ContextKey = dr["ContextKey"].ToString();
            data.ContextData = (byte[])dr["ContextData"];
            if (dr["Error"] != DBNull.Value)
                data.Error = Convert.ToString(dr["Error"]);
            return data;
        }

        public override void NewTransaction(TccTransactionData data)
        {
            if ((int)_mode >= (int)TccPersisterMode.ActiveAndAllFailed)
            {
                string spName = "USP_TCC_NewTransaction";
                string[] spParams = { "@TxId", "@ServiceAtComputer", "@TxSchema", "@BeginTime", "@LastUpdateTime", "@TxState", "@WorkState", "@ContextKey", "@ContextData" };
                string states = WorkStatesToString(data.WorkStates);
                object[] spValues = { data.TxId, data.ServiceAtComputer, data.SchemaName, data.BeginTime, DateTime.Now, (Int16)data.TxState, states, data.ContextKey, data.ContextData };
                _db.SpExecuteNonQuery(spName, spParams, spValues);
            }
        }

        public override void UpdateTransaction(TccTransactionData data)
        {
            if ((int)_mode >= (int)TccPersisterMode.ActiveAndAllFailed)
            {
                string spName = "USP_TCC_UpdateTransaction";
                string[] spParams = { "@TxId", "@LastUpdateTime", "@TxState", "@WorkState", "@ContextData" };
                string states = WorkStatesToString(data.WorkStates);
                object[] spValues = { data.TxId, DateTime.Now, (Int16)data.TxState, states, data.ContextData };
                _db.SpExecuteNonQuery(spName, spParams, spValues);
            }
        }

        public override void CloseTransaction(TccTransactionData data)
        {
            switch (_mode)
            {
                case TccPersisterMode.None:
                    break;
                case TccPersisterMode.CloseFailed:
                    if (data.TxState == TccTransactionState.CancelFailed || data.TxState == TccTransactionState.ConfirmFailed)
                    {
                        //插入TransactionLog表
                        InsertTransactionLog(data);
                    }
                    break;
                case TccPersisterMode.AllFailed:
                    if (data.TxState != TccTransactionState.Confirmed)
                    {
                        //插入TransactionLog表
                        InsertTransactionLog(data);
                    }
                    break;
                case TccPersisterMode.ActiveAndAllFailed:
                    //删除ActiveTransaction表中内容
                    DeleteActiveTransaction(data.TxId);
                    if (data.TxState != TccTransactionState.Confirmed)
                    {
                        //插入TransactionLog表
                        InsertTransactionLog(data);
                    }
                    break;
                case TccPersisterMode.All:
                    //删除ActiveTransaction表中内容
                    DeleteActiveTransaction(data.TxId);
                    //插入TransactionLog表
                    InsertTransactionLog(data);
                    break;
            }
        }

        private void DeleteActiveTransaction(string txId)
        {
            string spName = "USP_TCC_DeleteActiveTransaction";
            string[] spParams = { "@TxId" };
            object[] spValues = { txId };
            _db.SpExecuteNonQuery(spName, spParams, spValues);
        }

        private void InsertTransactionLog(TccTransactionData data)
        {
            string spName = "USP_TCC_InsertTransactionLog";
            string[] spParams = { "@TxId", "@ServiceAtComputer", "@TxSchema", "@BeginTime", "@LastUpdateTime", "@TxState", "@WorkState", "@ContextKey", "@ContextData", "Error" };
            string states = WorkStatesToString(data.WorkStates);
            object[] spValues = { data.TxId, data.ServiceAtComputer, data.SchemaName, data.BeginTime, DateTime.Now, (Int16)data.TxState, states, data.ContextKey, data.ContextData, data.Error };
            _db.SpExecuteNonQuery(spName, spParams, spValues);
        }

        private string WorkStatesToString(TccWorkState[] states)
        {
            StringBuilder workstates = new StringBuilder();
            for (int i = 0; i < states.Length; i++)
            {
                workstates.Append((int)states[i]);
                if (i != states.Length - 1)
                    workstates.Append(",");
            }
            return workstates.ToString();
        }

    }
}
