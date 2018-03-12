using System;
using System.Collections.Generic;
using System.Text;
using System.Data.OleDb;
using System.Data;
using CommonControls;
using System.IO;

namespace CommonControls.CommonMethods.AccessDB
{
    public class c_AccessDataBase
    {
        public bool Connected = false;
        private OleDbConnection myAccessConn = null;
        c_ErrorDataWork ErrorWork = c_ErrorDataWork.Instance;

        // kecit не использовать, а пользоваться Instance
        public c_AccessDataBase()
        {

        }

        private static c_AccessDataBase ThisInstance = null;
        public static c_AccessDataBase Instance
        {
            get
            {
                if (ThisInstance == null)
                {
                    ThisInstance = new c_AccessDataBase();
                }
                return ThisInstance;
            }
        }

        public string OpenDBDialog(string defaultPath)
        {            
            Stream FileOpened;
            System.Windows.Forms.OpenFileDialog openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            openFileDialog1.Title = "Укажите путь к файлу Базы Данных";
            openFileDialog1.InitialDirectory = defaultPath;
            openFileDialog1.Filter = "DataBase files (*.mdb)|*.mdb| All files (*.*)|*.*";
            openFileDialog1.FilterIndex = 1;
            openFileDialog1.RestoreDirectory = true;

            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                CloseDB();
                if ((FileOpened = openFileDialog1.OpenFile()) != null)
                {
                    FileOpened.Close();
                    try
                    {
                        if (!OpenDB(openFileDialog1.FileName))
                        {
                            throw new Exception("Не удалось подключится к базе данных " + openFileDialog1.FileName);
                        }
                        return openFileDialog1.FileName;
                    }
                    catch (Exception ex)
                    {
                        ErrorWork.ErrorProcess(ex);
                    };
                };
            };

            return "";
        }

        public bool OpenDB(String SourceBasePath)
        {
            CloseDB();
            if (!System.IO.File.Exists(SourceBasePath)) return false;
            try
            {
                myAccessConn = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + SourceBasePath + ";");
                myAccessConn.Open();
            }
            catch (Exception ex)
            {                
                ex.HelpLink = SourceBasePath;
                ErrorWork.ErrorProcess(ex);
                Connected = false;
                return false;
            }
            Connected = true;
            return true;
        }

        public void CloseDB()
        {
            if (myAccessConn != null)
            {
                myAccessConn.Close();
                myAccessConn.Dispose();
                myAccessConn = null;
            }
            Connected = false;
        }

        public DataSet GetDataBaseTable(string TableName)
        {
            return GetDataBaseData("SELECT * FROM " + TableName);
        }

        public DataSet GetDataBaseData(string strAccessSelect)
        {
            DataSet myDataSet = new DataSet();

            try
            {
                OleDbDataAdapter myDataAdapter = new OleDbDataAdapter(strAccessSelect, myAccessConn);
                myDataAdapter.Fill(myDataSet);
            }
            catch (Exception ex)
            {
                ex.HelpLink = strAccessSelect;
                ErrorWork.ErrorProcess(ex);
            }

            return myDataSet;
        }

        public DataSet GetDataBaseData(string TableName, string Columns, string Condition)
        {
            string strAccessSelect = "SELECT ";
            if (Columns != "") strAccessSelect += Columns;
            strAccessSelect += " FROM " + TableName;
            if (Condition != "") strAccessSelect += " WHERE " + Condition;
            DataSet myDataSet = new DataSet();

            try
            {
                OleDbDataAdapter myDataAdapter = new OleDbDataAdapter(strAccessSelect, myAccessConn);
                myDataAdapter.Fill(myDataSet);
            }
            catch (Exception ex)
            {
                ex.HelpLink = strAccessSelect;
                ErrorWork.ErrorProcess(ex);
            }

            return myDataSet;
        }

        public string[] GetDataBaseDataStringRowColumn(string TableName, string Columns, string Condition)
        {
            string strAccessSelect = "SELECT ";
            if (Columns != "") strAccessSelect += Columns;
            strAccessSelect += " FROM " + TableName;
            if (Condition != "") strAccessSelect += " WHERE " + Condition;
            return GetDataBaseDataStringRowColumn(strAccessSelect);
        }

        public string[] GetDataBaseDataStringRowColumn(string strAccessSelect)
        {
            DataSet myDataSet = GetDataBaseData(strAccessSelect);
            string[] ReturnValue = new string[0];
            try
            {
                if (myDataSet.Tables.Count > 0)
                {
                    if (myDataSet.Tables[0].Rows.Count > 0)
                    {
                        if (myDataSet.Tables[0].Rows.Count > 1)
                        {
                            ReturnValue = new string[myDataSet.Tables[0].Rows.Count];
                            for (int i = 0; i < ReturnValue.Length; i++)
                            {
                                ReturnValue[i] = myDataSet.Tables[0].Rows[i].ItemArray[0].ToString();
                            }
                        }
                        else if (myDataSet.Tables[0].Columns.Count >= 1)
                        {
                            ReturnValue = new string[myDataSet.Tables[0].Columns.Count];
                            for (int i = 0; i < ReturnValue.Length; i++)
                            {
                                ReturnValue[i] = myDataSet.Tables[0].Rows[0].ItemArray[i].ToString();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ex.HelpLink = strAccessSelect;
                ErrorWork.ErrorProcess(ex);
            }

            return ReturnValue;
        }

        public string[,] GetDataBaseDataStringTable(string strAccessSelect)
        {
            DataSet myDataSet = GetDataBaseData(strAccessSelect);
            string[,] ReturnValue = new string[0,0];
            try
            {
                if (myDataSet.Tables.Count > 0)
                {
                    if (myDataSet.Tables[0].Rows.Count > 0 && myDataSet.Tables[0].Columns.Count > 0)
                    {
                        ReturnValue = new string[myDataSet.Tables[0].Rows.Count, myDataSet.Tables[0].Columns.Count];
                        for (int i = 0; i < ReturnValue.GetLength(0); i++)
                        {
                            for (int j = 0; j < ReturnValue.GetLength(1); j++)
                            {
                                ReturnValue[i,j] = myDataSet.Tables[0].Rows[i].ItemArray[j].ToString();
                            }
                        }
                    }                   
                }
            }
            catch (Exception ex)
            {
                ex.HelpLink = strAccessSelect;
                ErrorWork.ErrorProcess(ex);
            }

            return ReturnValue;
        }

        public string GetDataBaseDataStringCell(string strAccessSelect)
        {
            string[] Result = GetDataBaseDataStringRowColumn(strAccessSelect);
            if (Result.Length > 0)
            {
                if (Result[0] == null) return "";
                return Result[0];
            }
            else return "";
        }

        public object GetDataBaseDataCell(string strAccessSelect)
        {
            DataSet TempData = GetDataBaseData(strAccessSelect);
            if (TempData.Tables.Count == 0) return null;
            return TempData.Tables[0].Rows[0].ItemArray[0];
        }

        public DataSet GetDataBaseDataTable(string strAccessSelect, string TableName)
        {
            DataSet myDataSet = new DataSet();

            try
            {
                OleDbDataAdapter myDataAdapter = new OleDbDataAdapter(strAccessSelect, myAccessConn);
                myDataAdapter.Fill(myDataSet, TableName);
            }
            catch (Exception ex)
            {
                ex.HelpLink = strAccessSelect;
                ErrorWork.ErrorProcess(ex);
            }
            return myDataSet;
        }

        public void Insert(string strAccessInsert)
        {
            try
            {
                OleDbCommand myAccessCommand = new OleDbCommand(strAccessInsert, myAccessConn);
                myAccessCommand.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                ex.HelpLink = strAccessInsert;
                ErrorWork.ErrorProcess(ex);
            }
        }

        public void Insert(string TableName, string[] Data)
        {
            string strAccessInsert = "INSERT INTO " + TableName + " VALUES (";
            for (int i = 0; i < Data.Length; i++)
            {
                strAccessInsert += "'" + Data[i];
                if ((i + 1) < Data.Length) strAccessInsert += "',";
            }
            strAccessInsert += "')";
            Insert(strAccessInsert);
        }

        public void Insert(string TableName, string Data)
        {
            Insert(TableName, new string[] { Data });
        }

        public void Insert(string TableName, string Values, byte[] Data, string AdditionalValues)
        {
            string strAccessInsert = "INSERT INTO " + TableName + "(" + Values + ")" + " VALUES (@Image," + AdditionalValues + ")";
            try
            {
                OleDbCommand myAccessCommand = new OleDbCommand(strAccessInsert, myAccessConn);
                OleDbParameter TempParameter = new OleDbParameter("@Image", OleDbType.VarBinary, Data.Length);
                TempParameter.Value = Data;
                myAccessCommand.Parameters.Add(TempParameter);
                myAccessCommand.ExecuteNonQuery();

            }
            catch (Exception ex)
            {
                ex.HelpLink = strAccessInsert;
                ErrorWork.ErrorProcess(ex);
            }
        }

        public void Update(object[] myDataSet, string strAccessUpdate)
        {
            try
            {
                OleDbCommand myAccessCommand = new OleDbCommand(strAccessUpdate, myAccessConn);
                myAccessCommand.Parameters.AddRange(myDataSet);
                OleDbDataAdapter myDataAdapter = new OleDbDataAdapter(myAccessCommand);
                //myDataAdapter.Update(myDataSet);
            }
            catch (Exception ex)
            {
                ErrorWork.ErrorProcess(ex);
            }
        }

        public void Update(string TableName, string Values, string Condition)
        {
            string strAccessUpdate = "UPDATE " + TableName;
            if (Values != null)
            {
                strAccessUpdate += " SET " + Values;
            }
            if (Condition != null)
            {
                strAccessUpdate += " WHERE " + Condition;
            }

            try
            {
                OleDbCommand myAccessCommand = new OleDbCommand(strAccessUpdate, myAccessConn);
                myAccessCommand.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                ex.HelpLink = strAccessUpdate;
                ErrorWork.ErrorProcess(ex);
            }
        }

        public void Delete(string TableName, string Condition)
        {
            string strAccessDelete = "DELETE FROM " + TableName;
            if (Condition != null)
            {
                strAccessDelete += " WHERE " + Condition;
            }

            try
            {
                OleDbCommand myAccessCommand = new OleDbCommand(strAccessDelete, myAccessConn);
                myAccessCommand.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                ex.HelpLink = strAccessDelete;
                ErrorWork.ErrorProcess(ex);
            }
        }
    }
}
