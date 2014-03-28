using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Data;
using System.Collections;
using System.Web;

namespace RY.Common
{
    public class ExcelToSQL
    {
        //string SqlConnectionString = "Server=(local);Initial Catalog=Test;Integrated Security=True";
        public SqlConnection sqlcon = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["ImageOfTaiWan"].ConnectionString);    //创建SQL连接
        public SqlCommand sqlcom;          //创建SQL命令对象


        public ExcelToSQL()
        {
            //DataOperation dataOperation = new DataOperation(); //用到平台的函数，就是初始化SqlConnection对象
            //DBUnit dbUnit = dataOperation.GetDbUnit();
            //sqlcon = (SqlConnection)dbUnit.cnt;
            if (sqlcon.State.ToString() == "Open")
                sqlcon.Close();
        }
        public int[] ImportUser2Sql(string excelPath, string tableName,int enterpriseId)  //导入的Excel的路径，数据库里的表名
        {
            int successedData=0;
            int errorData=0;
            int lgnore = 0;
            int[] result = new int[4];
            if (!TableExist(tableName)) //表名是否存在
                result[0]=(int)ImportState.tableNameError;

            DataTable dt = ExcelToDataTable(excelPath);
            if (dt == null)
            {
                result[0] = (int)ImportState.excelFormatError;
            }
            ArrayList tableField = GetTableField(tableName);   //表格的列名称

            string columnName = "Id,"; //Excel里的列名，增加一个ID列
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                columnName += dt.Columns[i].ColumnName + ",";
                string currentColumn = dt.Columns[i].ToString().ToUpper(); //当前列名
                for (int j = 0; j < tableField.Count; j++)
                {
                    if (tableField[j].ToString().ToUpper() == dt.Columns[i].ToString().ToUpper())
                        break;   //跳出本层和上一层循环，continue是跳出本层循环，如果用continue，会继续执行j++
                    //Excel里的字段必须在Sql中都有
                    if ((tableField[j].ToString().ToUpper() != dt.Columns[i].ToString().ToUpper()) && j == tableField.Count - 1)
                        result[0] = (int)ImportState.fieldMatchError;
                }
            }
            int m = columnName.LastIndexOf(',');
            columnName = columnName.Remove(m);  //移除最后一个逗号

            
            sqlcom = new SqlCommand();
            sqlcom.Connection = sqlcon;
            sqlcon.Open();
            sqlcom.CommandType = CommandType.Text;

            for (int h = 0; h < dt.Rows.Count; h++)
            {
                if (string.IsNullOrEmpty(dt.Rows[h][0].ToString().Trim()) || string.IsNullOrEmpty(dt.Rows[h][1].ToString().Trim()))
                {
                    lgnore++;
                }
                else
                {
                    string userInfor = "";
                    userInfor += "'" + dt.Rows[h][0].ToString().Trim() + "' ,'" + ASE.EncryptCode(dt.Rows[h][1].ToString().Trim()) + "'";
                    try
                    {
                        //string sql = "insert into " + tableName + "(" + columnName + ") values('" + value + "')";
                        string sql = "insert into [" + tableName + "] values(" + userInfor + ", '" + System.DateTime.Now + "', " + enterpriseId + " " + ")";

                        sqlcom.CommandText = sql;
                        string sss = sqlcom.ExecuteNonQuery().ToString();
                        successedData++;
                    }
                    catch (Exception err)
                    {
                        string erroe = err.Message;
                        errorData++;
                        //return (int)ImportState.dataTypeError;
                    }
                }
            }
            sqlcon.Close();
            sqlcom.Dispose();
            result[1] = successedData;
            result[2] = errorData;
            result[3] = lgnore;
            return result;
        }
        public DataTable ExcelToDataTable(string excelPath)  //把Excel里的数据转换为DataTable，并返回DataTable
        {
            string strCon = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + excelPath + ";Extended Properties='Excel 8.0;IMEX=1'";
            System.Data.OleDb.OleDbConnection Conn = new System.Data.OleDb.OleDbConnection(strCon);
            string strCom = "SELECT * FROM [Sheet1$]";
            DataTable dt;
            try
            {
                Conn.Open();
                System.Data.OleDb.OleDbDataAdapter myCommand = new System.Data.OleDb.OleDbDataAdapter(strCom, Conn);
                DataSet ds = new DataSet();
                myCommand.Fill(ds, "[Sheet1$]");
                Conn.Close();
                dt = ds.Tables[0];
            }
            catch (Exception err)
            {
                return null;
            }
            return dt;
        }
        public bool TableExist(string tableName) //查看数据库里是否有此表名
        {
            sqlcom = new SqlCommand();
            sqlcom.Connection = sqlcon;
            sqlcom.CommandType = CommandType.Text;
            try
            {
                sqlcon.Open();
                string sql = "select name from sysobjects where type='u'";
                sqlcom.CommandText = sql;
                SqlDataReader sqldr = sqlcom.ExecuteReader();
                while (sqldr.Read())
                {
                    if (sqldr.GetString(0).ToUpper() == tableName.ToUpper())
                        return true;
                }
            }
            catch { return false; }
            finally
            {
                sqlcon.Close();
            }
            return false;
        }
        public ArrayList GetTableField(string tableName)  //得到数据库某一个表中的所有字段
        {
            ArrayList al = new ArrayList();
            sqlcom = new SqlCommand();
            sqlcom.Connection = sqlcon;
            sqlcom.CommandType = CommandType.Text;
            try
            {
                sqlcon.Open();
                string sql = "SELECT b.name FROM sysobjects a INNER JOIN syscolumns b ON a.id = b.id WHERE (a.name = '" + tableName + "')";
                sqlcom.CommandText = sql;
                SqlDataReader sqldr = sqlcom.ExecuteReader();
                while (sqldr.Read())
                {
                    al.Add(sqldr.GetString(0));
                }
            }
            finally
            {
                sqlcon.Close();
            }
            return al; //返回的是表中的字段
        }
        public enum ImportState
        {
            right = 1, //成功
            tableNameError = 2,//表名不存在
            fieldMatchError = 3,//excel里的字段和数据库表里的字段不匹配
            dataTypeError = 4, //转换数据类型时发生错误
            excelFormatError = 5,//Excel格式不能读取
        }
        public void Alert(string str)
        {
            HttpContext.Current.Response.Write("<script language='javascript'>alert('" + str + "');</script>");
        }
    }
}
