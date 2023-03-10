using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using ShopOrder.Entities;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace ShopOrder.Utils
{
    public class DatabaseUtils
    {
        private static SqlConnection InitConnection()
        {
            try
            {
                ShopOrderEntities db = new ShopOrderEntities();
                return new SqlConnection(db.Database.Connection.ConnectionString);
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public static int? GetFirstFieldInt(string query, Dictionary<string, object> attrs = null)
        {
            string error;
            return GetFirstFieldInt(query, attrs, out error);
        }
        public static int? GetFirstFieldInt(string query, Dictionary<string, object> attrs, out string error)
        {
            object data = GetFirstField(query, attrs, out error);
            if (data == null) return null;
            else return int.Parse(data.ToString());
        }
        public static string GetFirstFieldString(string query, Dictionary<string, object> attrs = null)
        {
            string error;
            return GetFirstFieldString(query, attrs, out error);
        }
        public static string GetFirstFieldString(string query, Dictionary<string, object> attrs, out string error)
        {
            object data = GetFirstField(query, attrs, out error);
            if (data == null) return null;
            else return data.ToString();
        }
        public static object GetFirstField(string query, Dictionary<string, object> attrs = null)
        {
            string error;
            return GetFirstField(query, attrs, out error);
        }
        public static object GetFirstField(string query, Dictionary<string, object> attrs, out string error)
        {
            DataTable dt = GetTable(query, attrs, out error);
            if (dt == null || dt.Rows.Count == 0) return null;
            else return dt.Rows[0][0];
        }

        public static DataRow GetFirstRow(string query, Dictionary<string, object> attrs = null)
        {
            string error;
            return GetFirstRow(query, attrs, out error);
        }

        public static DataRow GetFirstRow(string query, Dictionary<string, object> attrs, out string error)
        {
            DataTable dt = GetTable(query, attrs, out error);
            if (dt == null || dt.Rows.Count == 0) return null;
            else return dt.Rows[0];
        }
        public static DataTable GetTable(string query, Dictionary<string, object> attrs = null)
        {
            string error;
            return GetTable(query, attrs, out error);
        }

        public static DataTable GetTable(string query, Dictionary<string, object> attrs, out string error)
        {
            error = "";
            DataTable dt = new DataTable();
            SqlConnection conn = InitConnection();
            if (conn == null)
            {
                error = "Không thể kết nối đến dữ liệu";
                return dt;
            }

            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(query, conn);
                BuilderParameter(cmd, attrs);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
                dt.Columns.Add("STT", typeof(string));

                int counter = 0;
                foreach (DataRow row in dt.Rows)
                {
                    counter++;
                    row["STT"] = counter.ToString();
                }
            }
            catch (Exception ex)
            {
                error = ex.ToString();
            }
            finally
            {
                if (conn != null && conn.State == ConnectionState.Open)
                {
                    conn.Close();
                    conn.Dispose();
                }
            }
            return dt;
        }
        public static bool ExcuteQuery(string query, Dictionary<string, object> attrs = null)
        {
            string error;
            return ExcuteQuery(query, attrs, out error);
        }
        public static bool ExcuteQuery(string query, Dictionary<string, object> attrs, out string error)
        {
            error = "";
            SqlConnection conn = InitConnection();
            if (conn == null)
            {
                error = "Không thể kết nối đến dữ liệu";
                return false;
            }

            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(query, conn);
                BuilderParameter(cmd, attrs);
                cmd.ExecuteNonQuery();
                return true;
            }
            catch (Exception ex)
            {
                error = ex.ToString();
                return false;
            }
            finally
            {
                if (conn != null && conn.State == ConnectionState.Open)
                {
                    conn.Close();
                    conn.Dispose();
                }
            }
        }
        private static void BuilderParameter(SqlCommand cmd, Dictionary<string, object> attrs = null)
        {
            if (attrs != null && attrs.Count > 0)
            {
                foreach (string key in attrs.Keys)
                {
                    object value = attrs[key];
                    if (value == null) continue;
                    Type typeData = value.GetType();
                    if (typeData == typeof(int)) cmd.Parameters.Add(key, SqlDbType.Int).Value = attrs[key];
                    else if (typeData == typeof(decimal)) cmd.Parameters.Add(key, SqlDbType.Decimal).Value = attrs[key];
                    else if (typeData == typeof(string)) cmd.Parameters.Add(key, SqlDbType.VarChar).Value = attrs[key];
                    else if (typeData == typeof(DateTime)) cmd.Parameters.Add(key, SqlDbType.Date).Value = attrs[key];
                }
            }
        }

        public static string GenCode(string field, string table, DNHANVIEN dNHANVIEN = null, int loai = 0)
        {
            bool includeDate = false;
            int maxLength = 8;
            string code = "";
            if (table == "DMATHANG")
            {
                code = "MH";
                if (dNHANVIEN != null && dNHANVIEN.CODEMATHANG != null && dNHANVIEN.CODEMATHANG.Length > 0)
                {
                    code = dNHANVIEN.CODEMATHANG;
                    maxLength += code.Length - 2;
                }
            }
            else if (table == "DKHACHHANG") code = "KH";
            else if (table == "TDONHANG")
            {
                includeDate = true;
                if (loai == 0) code = "DH";
                else if (loai == 1) code = "PN";

                code += DateTime.Now.ToString("yyMM");
                maxLength = 12;
            }
            else if (table == "TTHANHTOAN")
            {
                includeDate = true;
                code = "TT" + DateTime.Now.ToString("yyMM");
                maxLength = 12;
            }
            else if (table == "TGIAOHANG")
            {
                includeDate = true;
                code = "GH" + DateTime.Now.ToString("yyMM");
                maxLength = 12;
            }

            int stt = 0;
            string query = "SELECT MAX({0}) FROM {1}";
            if (table == "TDONHANG")
            {
                query += " WHERE LOAI = " + loai;
                if (includeDate)
                {
                    query += " AND {0} LIKE '{2}%'";
                }
            }
            else if (includeDate)
            {
                query += " WHERE {0} LIKE '{2}%'";
            }
            string data = GetFirstFieldString(string.Format(query, field, table, code), null);
            if (data != null && data.Length > 0)
            {
                data = data.Replace(code, "");
                stt = int.Parse(data);
            }
            stt++;

            int length = maxLength - code.Length - stt.ToString().Length;
            for (int i = 0; i < length; i++) code += "0";
            code += stt.ToString();

            return code;
        }

        public static void Log(TDONHANG dhRow, string NOTE)
        {
            ShopOrderEntities db = new ShopOrderEntities();
            TLUUVET t = new TLUUVET();
            t.ID = Guid.NewGuid().ToString();
            t.TIMECREATED = DateTime.Now;
            t.TDONHANGID = dhRow.ID;
            t.NOTE = NOTE;
            db.TLUUVETs.Add(t);
            db.SaveChanges();
        }

        public static void Log(TDONHANGCHITIET ctRow, string NOTE)
        {
            ShopOrderEntities db = new ShopOrderEntities();
            TLUUVET t = new TLUUVET();
            t.ID = Guid.NewGuid().ToString();
            t.TIMECREATED = DateTime.Now;
            t.TDONHANGCHITIETID = ctRow.ID;
            t.NOTE = NOTE;
            db.TLUUVETs.Add(t);
            db.SaveChanges();
        }
    }
}