﻿using FootballFieldManagement.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FootballFieldManagement.DAL
{
    class FootballFieldDAL : DataProvider
    {
        private static FootballFieldDAL instance;
        public static FootballFieldDAL Instance
        {
            get
            {
                if (instance == null)
                    instance = new FootballFieldDAL();
                return FootballFieldDAL.instance;
            }
            private set
            {
                FootballFieldDAL.instance = value;
            }
        }

        FootballFieldDAL()
        {

        }
        public List<FootballField> ConvertDBToList()
        {
            DataTable dataTable = new DataTable();
            List<FootballField> footballFields = new List<FootballField>();
            try
            {
                OpenConnection();
                string queryString = @"Select * from FootballField
                                       Where isDeleted=0";

                SqlCommand command = new SqlCommand(queryString, conn);
                command.ExecuteNonQuery();
                SqlDataAdapter adapter = new SqlDataAdapter(command);
                adapter.Fill(dataTable);
                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                    FootballField footballField = new FootballField(int.Parse(dataTable.Rows[i].ItemArray[0].ToString()), dataTable.Rows[i].ItemArray[1].ToString(),
                    int.Parse(dataTable.Rows[i].ItemArray[2].ToString()), int.Parse(dataTable.Rows[i].ItemArray[3].ToString()),
                    dataTable.Rows[i].ItemArray[4].ToString(), int.Parse(dataTable.Rows[i].ItemArray[5].ToString()));
                    footballFields.Add(footballField);
                }
            }
            catch
            {

            }
            finally
            {
                CloseConnection();
            }

            return footballFields;
        }
        public bool AddIntoDB(FootballField footballField)
        {
            try
            {
                OpenConnection();
                string query = "insert into FootballField(idField, name, type, status, note,isDeleted) values (@idField, @name, @type, @status, @note,@isDeleted)";
                SqlCommand command = new SqlCommand(query, conn);
                command.Parameters.AddWithValue("@idField", footballField.IdField.ToString());
                command.Parameters.AddWithValue("@name", footballField.Name);
                command.Parameters.AddWithValue("@type", footballField.Type.ToString());
                command.Parameters.AddWithValue("@status", footballField.Status.ToString());
                command.Parameters.AddWithValue("@note", footballField.Note);
                command.Parameters.AddWithValue("@isDeleted", footballField.IsDeleted);
                int rs = command.ExecuteNonQuery();
                if (rs != 1)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            catch
            {
                return false;
            }
            finally
            {
                CloseConnection();
            }
        }
        public bool DeleteField(string idField)
        {
            try
            {
                OpenConnection();
                string query = @"Update FootballField 
                                 Set isDeleted=1
                                 Where idField = " + idField;
                SqlCommand command = new SqlCommand(query, conn);
                int rs = command.ExecuteNonQuery();
                if (rs == 1)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
            finally
            {
                CloseConnection();
            }
        }
        public bool UpdateField(FootballField footballField)
        {
            try
            {
                OpenConnection();
                string query = @"update FootballField set idField = @idField, name = @name, type = @type, status = @status,isDeleted=@isDeleted where idField = " + footballField.IdField.ToString();
                SqlCommand command = new SqlCommand(query, conn);
                command.Parameters.AddWithValue("@idField", footballField.IdField.ToString());
                command.Parameters.AddWithValue("@name", footballField.Name);
                command.Parameters.AddWithValue("@type", footballField.Type.ToString());
                command.Parameters.AddWithValue("@status", footballField.Status.ToString());
                command.Parameters.AddWithValue("@isDeleted", footballField.IsDeleted);
                int rs = command.ExecuteNonQuery();
                if (rs == 1)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
            finally
            {
                CloseConnection();
            }
        }

        public List<string> GetFieldType()
        {
            try
            {
                OpenConnection();
                string query = @"select distinct(type) from FootballField where isDeleted=0 order by type ASC";
                SqlCommand cmd = new SqlCommand(query, conn);
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);
                List<string> listTmp = new List<string>();
                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                    listTmp.Add(dataTable.Rows[i].ItemArray[0].ToString());
                }
                return listTmp;
            }
            catch
            {
                return null;
            }
            finally
            {
                CloseConnection();
            }
        }
        public bool isExistFieldName(string fieldName)
        {
            try
            {
                OpenConnection();
                string query = @"select * from FootballField where isDeleted=0 and name = '" + fieldName + "'";
                SqlCommand cmd = new SqlCommand(query, conn);
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);
                if (dataTable.Rows.Count == 0)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            catch
            {
                return true;
            }
            finally
            {
                CloseConnection();
            }

        }
        public FootballField GetFootballFieldById(string idField)
        {
            try
            {
                OpenConnection();
                string queryString = "select * from FootballField where idField = " + idField;

                SqlCommand command = new SqlCommand(queryString, conn);
                SqlDataAdapter adapter = new SqlDataAdapter(command);

                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);

                FootballField res = new FootballField(int.Parse(idField), dataTable.Rows[0].ItemArray[1].ToString(),
                    int.Parse(dataTable.Rows[0].ItemArray[2].ToString()), int.Parse(dataTable.Rows[0].ItemArray[3].ToString()),
                    dataTable.Rows[0].ItemArray[4].ToString(), int.Parse(dataTable.Rows[0].ItemArray[5].ToString()));
                return res;
            }
            catch
            {
                return new FootballField();
            }
            finally
            {
                CloseConnection();
            }
        }
        public List<FootballField> GetNamesPerType(string type)
        {
            List<FootballField> res = new List<FootballField>();
            try
            {
                OpenConnection();
                string queryString = @"Select *
                                       From FootballField
                                       Where type =@type and isDeleted=0
                                       Order by type ASC ";
                SqlCommand command = new SqlCommand(queryString, conn);
                command.Parameters.AddWithValue("@type", type);
                command.ExecuteNonQuery();
                SqlDataAdapter adapter = new SqlDataAdapter(command);
                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);
                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                    FootballField footballField = new FootballField(int.Parse(dataTable.Rows[i].ItemArray[0].ToString()),
                                                  dataTable.Rows[i].ItemArray[1].ToString(), int.Parse(dataTable.Rows[i].ItemArray[2].ToString()),
                                                  int.Parse(dataTable.Rows[i].ItemArray[3].ToString()), dataTable.Rows[i].ItemArray[4].ToString(),
                                                  int.Parse(dataTable.Rows[i].ItemArray[5].ToString()));
                    res.Add(footballField);
                }
            }
            catch
            {

            }
            finally
            {
                CloseConnection();
            }
            return res;
        }
        public List<FootballField> GetEmptyField(string type, string day, string startTime, string endTime)
        {
            List<FootballField> footballFields = new List<FootballField>();
            try
            {
                OpenConnection();
                string query = @"Select idField,name from FootballField
                                 Where FootballField.type=@type
                                 Except
                                 Select FieldInfo.idField,FootballField.name from FieldInfo
                                 Join FootballField on FieldInfo.idField=FootballField.idField
                                 Where convert(varchar(10), startingTime, 103)=@day and convert(varchar(5), startingTime, 108)=@startTime and convert(varchar(5), endingTime, 108) =@endTime and FootballField.type=@type and FootballField.isDeleted=0";
                SqlCommand command = new SqlCommand(query, conn);
                command.Parameters.AddWithValue("@day", day);
                command.Parameters.AddWithValue("@startTime", startTime);
                command.Parameters.AddWithValue("@endTime", endTime);
                command.Parameters.AddWithValue("@type", type);
                command.ExecuteNonQuery();
                SqlDataAdapter adapter = new SqlDataAdapter(command);
                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);
                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                    FootballField footballField = new FootballField(int.Parse(dataTable.Rows[i].ItemArray[0].ToString()), dataTable.Rows[i].ItemArray[1].ToString(),
                                                                    int.Parse(type), 0, " ", 0);
                    footballFields.Add(footballField);
                }
            }
            catch
            {

            }
            finally
            {
                CloseConnection();
            }
            return footballFields;
        }
        public List<FootballField> GetGoodFields()
        {
            List<FootballField> footballFields = new List<FootballField>();
            try
            {
                OpenConnection();
                string query = @"select * from FootballField where isDeleted=0 and status=1 order by idField ASC";
                SqlCommand cmd = new SqlCommand(query, conn);
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);
                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                    FootballField footballField = new FootballField(int.Parse(dataTable.Rows[i].ItemArray[0].ToString()),
                                                  dataTable.Rows[i].ItemArray[1].ToString(), int.Parse(dataTable.Rows[i].ItemArray[2].ToString()),
                                                  int.Parse(dataTable.Rows[i].ItemArray[3].ToString()), dataTable.Rows[i].ItemArray[4].ToString(),
                                                  int.Parse(dataTable.Rows[i].ItemArray[5].ToString()));
                    footballFields.Add(footballField);
                }
            }
            catch
            {

            }
            finally
            {
                CloseConnection();
            }
            return footballFields;
        }
    }
}