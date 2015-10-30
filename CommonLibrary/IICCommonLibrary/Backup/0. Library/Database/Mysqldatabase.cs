//using System;
//using System.IO;
//using System.Data;
//using System.Reflection;
//using System.Collections.Generic;
//using System.Diagnostics;
//using System.Text;

//using Imps.Services.CommonV4;
//using MySql.Data.MySqlClient;
//using MySql.Data.Types;

//namespace Imps.Services.CommonV4.DbAccess
//{
//    public class MysqlDatabase: DatabaseOperation
//    {
//        internal MysqlDatabase(string connStr, int commandTimeout)
//            : base(connStr, commandTimeout)
//        {
//        }

//        #region DatabaseOperation Methods
//        public override int SpExecuteNonQuery(string spName, string[] paramters, params object[] values)
//        {
//            int result = 0;
//            MySqlConnection cnn = GetConnection();
//            MySqlCommand cmd = new MySqlCommand(spName, cnn);

//            cmd.CommandTimeout = CommandTimeout;
//            cmd.CommandType = CommandType.StoredProcedure;
//            SpFillParameters(cmd, paramters, values);
//            Stopwatch watch = Stopwatch.StartNew();
//            try {
//                result = cmd.ExecuteNonQuery();
//                PerfCounters.CommandExecutedTotal.Increment();
//                PerfCounters.CommandExecutedPerSec.Increment();
//            } catch (Exception) {
//                PerfCounters.CommandFailedTotal.Increment();
//                throw;
//            } finally {
//                cnn.Close();
//                PerfCounters.AvgExecuteMs.IncrementBy(watch.ElapsedMilliseconds);
//            }
//            return result;
//        }

//        public override DataReader SpExecuteReader(string spName, string[] paramters, params object[] values)
//        {
//            DataReader result = null;
//            MySqlConnection cnn = GetConnection();
//            MySqlCommand cmd = new MySqlCommand(spName, cnn);

//            cmd.CommandTimeout = CommandTimeout;
//            cmd.CommandType = CommandType.StoredProcedure;
//            SpFillParameters(cmd, paramters, values);

//            Stopwatch watch = Stopwatch.StartNew();
//            try {
//                result = new DataReader((IDataReader)cmd.ExecuteReader(CommandBehavior.CloseConnection), cmd);
//                PerfCounters.CommandExecutedTotal.Increment();
//                PerfCounters.CommandExecutedPerSec.Increment();
//            } catch (Exception) {
//                PerfCounters.CommandFailedTotal.Increment();
//                throw;
//            } finally {
//                cnn.Close();
//                PerfCounters.AvgExecuteMs.IncrementBy(watch.ElapsedMilliseconds);
//            }
//            return result;
//        }

//        public override T SpExecuteScalar<T>(string spName, string[] parameters, params object[] values)
//        {
//            T result = default(T);

//            MySqlConnection cnn = GetConnection();
//            MySqlCommand cmd = new MySqlCommand(spName, cnn);

//            cmd.CommandTimeout = CommandTimeout;
//            cmd.CommandType = CommandType.StoredProcedure;
//            SpFillParameters(cmd, parameters, values);

//            Stopwatch watch = Stopwatch.StartNew();
//            try {
//                result = (T)Convert.ChangeType(cmd.ExecuteScalar(), typeof(T));
//                PerfCounters.CommandExecutedTotal.Increment();
//                PerfCounters.CommandExecutedPerSec.Increment();
//            } catch (Exception) {
//                PerfCounters.CommandFailedTotal.Increment();
//                throw;
//            } finally {
//                cnn.Close();
//                PerfCounters.AvgExecuteMs.IncrementBy(watch.ElapsedMilliseconds);
//            }
//            return result;
//        }

//        public override DataTable SpExecuteTable(string spName, string[] paramters, params object[] values)
//        {
//            DataTable result = new DataTable();
//            MySqlConnection cnn = GetConnection();
//            MySqlCommand cmd = new MySqlCommand(spName, cnn);

//            cmd.CommandTimeout = CommandTimeout;
//            cmd.CommandType = CommandType.StoredProcedure;
//            SpFillParameters(cmd, paramters, values);

//            Stopwatch watch = Stopwatch.StartNew();
//            try {
//                MySqlDataAdapter adp = new MySqlDataAdapter(cmd);
//                adp.Fill(result);
//                PerfCounters.CommandExecutedTotal.Increment();
//                PerfCounters.CommandExecutedPerSec.Increment();
//            } catch (Exception) {
//                PerfCounters.CommandFailedTotal.Increment();
//                throw;
//            } finally {
//                cnn.Close();
//                PerfCounters.AvgExecuteMs.IncrementBy(watch.ElapsedMilliseconds);
//            }

//            return result;
//        }

//        public override DataSet SpExecuteDataSet(string spName, string[] paramters, params object[] values)
//        {
//            DataSet result = new DataSet();
//            MySqlConnection cnn = GetConnection();
//            MySqlCommand cmd = new MySqlCommand(spName, cnn);

//            cmd.CommandTimeout = CommandTimeout;
//            cmd.CommandType = CommandType.StoredProcedure;
//            SpFillParameters(cmd, paramters, values);

//            Stopwatch watch = Stopwatch.StartNew();
//            try {
//                MySqlDataAdapter adp = new MySqlDataAdapter(cmd);
//                adp.Fill(result);
//                PerfCounters.CommandExecutedTotal.Increment();
//                PerfCounters.CommandExecutedPerSec.Increment();
//            } catch (Exception) {
//                PerfCounters.CommandFailedTotal.Increment();
//                throw;
//            } finally {
//                cnn.Close();
//                PerfCounters.AvgExecuteMs.IncrementBy(watch.ElapsedMilliseconds);
//            }
//            return result;
//        }

//        public override int BulkInsert<T>(string tableName, IEnumerable<T> values)
//        {
//            Type type = typeof(T);
//            ComboClass<string, string[], List<TableFieldAttribute>> cache;
//            if (!_bulkCaches.TryGetValue(type, out cache)) {
//                cache = BuildBulkCache(tableName, type);
//                _bulkCaches.Add(type, cache);
//            }

//            string sql = cache.V1;
//            string[] parameters = cache.V2;
//            List<TableFieldAttribute> attrs = cache.V3;

//            int ret = 0;
//            MySqlConnection cnn = null;
//            MySqlTransaction trans = null;

//            try {
//                cnn = GetConnection();
//                trans = cnn.BeginTransaction(IsolationLevel.RepeatableRead);
//                foreach (T value in values) {
//                    object[] objs = new object[attrs.Count];
//                    for (int i = 0; i < attrs.Count; i++) {
//                        objs[i] = attrs[i].Field.GetValue(value);

//                    }
//                    MySqlCommand cmd = new MySqlCommand(sql, cnn, trans);
//                    cmd.CommandTimeout = CommandTimeout;
//                    cmd.CommandType = CommandType.Text;
//                    SpFillParameters(cmd, parameters, objs);
//                    cmd.ExecuteNonQuery();
//                    ret++;
//                }
//                trans.Commit();
//            } catch (Exception ex) {
//                if (trans != null)
//                    trans.Rollback();
//                throw ex;
//            } finally {
//                if (cnn != null)
//                    cnn.Close();
//            }

//            return ret;

//            //string seed = Guid.NewGuid().ToString("N");
//            //string path = "C:\\" + seed + ".txt";
//            //string tableSplitter = string.Format("\t[{0}]\t", seed);
//            //string lineSplitter = string.Format("\t<{0}>\r\n", seed);

//            //using (StreamWriter writer = new StreamWriter(path, false, Encoding.UTF8)) {
//            //    foreach (T value in values) {
//            //        foreach (TableFieldAttribute attr in cache.V2) {
//            //            object obj = attr.Field.GetValue(value);
//            //            if (attr.FieldType != null)
//            //                obj = Convert.ChangeType(obj, attr.FieldType);

//            //            writer.Write(FormatData(obj, attr.FieldType));
//            //            writer.Write(tableSplitter);
//            //        }
//            //        writer.Write(lineSplitter);
//            //    }
//            //}

//            //int ret;
//            //MySqlConnection cnn = null;
//            //try {
//            //    cnn = GetConnection();
//            //    MySqlBulkLoader bulkLoader = new MySqlBulkLoader(cnn);

//            //    bulkLoader.TableName = tableName;
//            //    bulkLoader.FileName = path;
//            //    bulkLoader.ConflictOption = MySqlBulkLoaderConflictOption.Replace;
//            //    bulkLoader.FieldTerminator = tableSplitter;
//            //    bulkLoader.LineTerminator = lineSplitter;
//            //    bulkLoader.Priority = MySqlBulkLoaderPriority.Concurrent;

//            //    ret = bulkLoader.Load();
//            //} catch (Exception) {
//            //    throw;
//            //} finally {
//            //    if (cnn != null)
//            //        cnn.Close();

//            //    try {
//            //        if (File.Exists(path)) {
//            //            File.Delete(path);
//            //        }
//            //    } catch (Exception ex) {
//            //        Trace.Write(ex.ToString());
//            //    }
//            //}
//            //return ret;
//        }

//        #endregion

//        #region Private Methods
//        private MySqlConnection GetConnection()
//        {
//            MySqlConnection cnn = new MySqlConnection(ConnectionString);
//            try {
//                cnn.Open();
//                PerfCounters.ConnectionOpenedPerSec.Increment();
//                PerfCounters.ConnectionOpenedTotal.Increment();
//            } catch (Exception) {
//                cnn.Close();
//                PerfCounters.ConnectionFailedTotal.Increment();
//                throw;
//            }
//            return cnn;
//        }

//        //private ComboClass<DataTable, List<TableFieldAttribute>> BuildBulkCache(Type t)
//        //{
//        //    DataTable table = new DataTable();
//        //    FieldInfo[] fields = t.GetFields();

//        //    List<TableFieldAttribute> attrs = new List<TableFieldAttribute>();

//        //    foreach (FieldInfo field in fields) {
//        //        TableFieldAttribute fieldAttr = AttributeHelper.TryGetAttribute<TableFieldAttribute>(field);
//        //        if (fieldAttr != null) {
//        //            DataColumn column = new DataColumn();
//        //            column.ColumnName = fieldAttr.ColumnName;
//        //            column.DataType = fieldAttr.FieldType ?? field.FieldType;
//        //            table.Columns.Add(column);

//        //            fieldAttr.Field = field;
//        //            fieldAttr.Column = column;
//        //            attrs.Add(fieldAttr);
//        //        }
//        //    }

//        //    ComboClass<DataTable, List<TableFieldAttribute>> cache = new ComboClass<DataTable, List<TableFieldAttribute>>();
//        //    cache.V1 = table;
//        //    cache.V2 = attrs;

//        //    return cache;
//        //}

//        private ComboClass<string, string[], List<TableFieldAttribute>> BuildBulkCache(string tableName, Type t)
//        {
//            //DataTable table = new DataTable();
//            StringBuilder sql = new StringBuilder();
//            StringBuilder sql2 = new StringBuilder();
//            List<string> param = new List<string>();

//            FieldInfo[] fields = t.GetFields();
//            sql.AppendFormat("INSERT INTO {0} (", tableName);

//            List<TableFieldAttribute> attrs = new List<TableFieldAttribute>();

//            foreach (FieldInfo field in fields) {
//                TableFieldAttribute fieldAttr = AttributeHelper.TryGetAttribute<TableFieldAttribute>(field);
//                if (fieldAttr != null) {
//                    attrs.Add(fieldAttr);
//                    fieldAttr.Field = field;
//                    sql.AppendFormat("{0},", fieldAttr.ColumnName);
//                    sql2.AppendFormat("@v_{0},", fieldAttr.ColumnName);
//                    param.Add("@" + fieldAttr.ColumnName);
//                }
//            }
//            sql.Remove(sql.Length - 1, 1);		// Trim End ','
//            sql2.Remove(sql2.Length - 1, 1);		// Trim End ','
//            sql.Append(") VALUES (");
//            sql.Append(sql2.ToString());
//            sql.Append(")");

//            ComboClass<string, string[], List<TableFieldAttribute>> cache = new ComboClass<string, string[], List<TableFieldAttribute>>();
//            cache.V1 = sql.ToString(); ;
//            cache.V2 = param.ToArray();
//            cache.V3 = attrs;

//            return cache;
//        }

//        private string FormatData(object obj, Type fieldType)
//        {
//            if (fieldType == typeof(byte[])) {
//                return "0x" + StrUtils.ToHexString((byte[])obj);
//            } else {
//                return obj.ToString();
//            }
//        }

//        private void SpFillParameters(MySqlCommand command, string[] parameters, object[] values)
//        {
//            if (values == null || values.Length == 0)
//                return;

//            for (int i = 0; i < parameters.Length; i++) {
//                string parameter = parameters[i];
//                object value = values[i];
//                if (value != null)
//                    command.Parameters.AddWithValue(parameter.Insert(1, "v_"), value);
//                else
//                    command.Parameters.AddWithValue(parameter.Insert(1, "v_"), DBNull.Value);
//            }
//        }
//        #endregion

//        private SafeDictionary<Type, ComboClass<string, string[], List<TableFieldAttribute>>> _bulkCaches = new SafeDictionary<Type, ComboClass<string, string[], List<TableFieldAttribute>>>();
//    }
//}