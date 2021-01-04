﻿using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Windows;

namespace FootballFieldManagement.DAL
{
    public class SQLConnection
    {
        private string strConn;
        public SqlConnection conn;
        public SQLConnection()
        {
            try
            {
                strConn = ConfigurationManager.ConnectionStrings["FFM"].ConnectionString;
            }
            catch
            {
                CustomMessageBox.Show("Mất kết nối đến cơ sở dữ liệu!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            conn = new SqlConnection(strConn);
        }
        public void OpenConnection()
        {
            try
            {
                if (conn.State != System.Data.ConnectionState.Open)
                {
                    conn.ConnectionString = ConfigurationManager.ConnectionStrings["FFM"].ConnectionString;
                    conn.Open();
                }
            }
            catch (Exception ex)
            {
                CustomMessageBox.Show("Mất kết nối đến cơ sở dữ liệu!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Error);
                throw ex;
            }
        }
        public void CloseConnection()
        {
            conn.Close();
        }
    }
}
