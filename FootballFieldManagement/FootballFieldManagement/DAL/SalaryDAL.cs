﻿using FootballFieldManagement.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace FootballFieldManagement.DAL
{
    class SalaryDAL : DataProvider
    {
        private static SalaryDAL instance;

        public static SalaryDAL Instance
        {
            get { if (instance == null) instance = new SalaryDAL(); return SalaryDAL.instance; }
            private set { SalaryDAL.instance = value; }
        }
        public SalaryDAL()
        {

        }
        public List<Salary> GetSalaryByMonth(string month, string year)
        {
            try
            {
                List<Salary> salaries = new List<Salary>();
                OpenConnection();
                string query = @"select * from Salary where month(salaryMonth) = @month and year(salaryMonth) = @year";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@month", month);
                cmd.Parameters.AddWithValue("@year", year);
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    int idSalaryRecord = -1;
                    if (dt.Rows[i].ItemArray[5].ToString() != "")
                    {
                        idSalaryRecord = int.Parse(dt.Rows[i].ItemArray[5].ToString());
                    }
                    Salary salary = new Salary(int.Parse(dt.Rows[i].ItemArray[0].ToString()), int.Parse(dt.Rows[i].ItemArray[1].ToString()),
                        int.Parse(dt.Rows[i].ItemArray[2].ToString()), long.Parse(dt.Rows[i].ItemArray[3].ToString())
                        , DateTime.Parse(dt.Rows[i].ItemArray[4].ToString()), idSalaryRecord);
                    salaries.Add(salary);
                }
                return salaries;
            }
            catch
            {
                return new List<Salary>();
            }
            finally
            {
                CloseConnection();
            }
        }
        public List<Salary> GetPaidSalary(string month, string year)
        {
            try
            {
                List<Salary> salaries = new List<Salary>();
                OpenConnection();
                string query = @"select * from Salary where month(salaryMonth) = @month and year(salaryMonth) = @year and idSalaryRecord is not null";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@month", month);
                cmd.Parameters.AddWithValue("@year", year);
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    int idSalaryRecord = -1;
                    if (dt.Rows[i].ItemArray[5].ToString() != "")
                    {
                        idSalaryRecord = int.Parse(dt.Rows[i].ItemArray[5].ToString());
                    }
                    Salary salary = new Salary(int.Parse(dt.Rows[i].ItemArray[0].ToString()), int.Parse(dt.Rows[i].ItemArray[1].ToString()),
                        int.Parse(dt.Rows[i].ItemArray[2].ToString()), long.Parse(dt.Rows[i].ItemArray[3].ToString())
                        , DateTime.Parse(dt.Rows[i].ItemArray[4].ToString()), idSalaryRecord);
                    salaries.Add(salary);
                }
                return salaries;
            }
            catch
            {
                return new List<Salary>();
            }
            finally
            {
                CloseConnection();
            }
        }
        public List<Salary> GetUnPaidSalary(string idEmployee, string month, string year)
        {
            List<Salary> salaries = new List<Salary>();
            try
            {
                OpenConnection();
                string query = @"select* from Salary where month(salaryMonth) = @month and year(salaryMonth) = @year and idEmployee =@idEmployee and idSalaryRecord is null";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@month", month);
                cmd.Parameters.AddWithValue("@year", year);
                cmd.Parameters.AddWithValue("@idEmployee", idEmployee);
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    int idSalaryRecord = -1;
                    if (dt.Rows[i].ItemArray[5].ToString() != "")
                    {
                        idSalaryRecord = int.Parse(dt.Rows[i].ItemArray[5].ToString());
                    }
                    Salary salary = new Salary(int.Parse(dt.Rows[i].ItemArray[0].ToString()), int.Parse(dt.Rows[i].ItemArray[1].ToString()),
                        int.Parse(dt.Rows[i].ItemArray[2].ToString()), long.Parse(dt.Rows[i].ItemArray[3].ToString())
                        , DateTime.Parse(dt.Rows[i].ItemArray[4].ToString()), idSalaryRecord);
                    salaries.Add(salary);
                }
            }
            catch
            {
                CustomMessageBox.Show("Lỗi!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                CloseConnection();
            }
            return salaries;
        }

        public bool AddIntoDB(Salary salary)
        {
            try
            {
                OpenConnection();
                string query = @"insert into Salary (idEmployee, numOfShift, numOfFault,totalSalary, salaryMonth) values(@idEmployee, @numOfShift, @numOfFault,@totalSalary, @salaryMonth)";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@idEmployee", salary.IdEmployee.ToString());
                cmd.Parameters.AddWithValue("@numOfShift", salary.NumOfShift.ToString());
                cmd.Parameters.AddWithValue("@numOfFault", salary.NumOfFault.ToString());
                cmd.Parameters.AddWithValue("@totalSalary", salary.TotalSalary.ToString());
                cmd.Parameters.AddWithValue("@salaryMonth", salary.SalaryMonth);
                if (cmd.ExecuteNonQuery() < 1)
                    return false;
                else
                    return true;
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

        //update ngày trả lương và người trả lương
        public bool UpdateIdSalaryRecord(int id, int month, int year)
        {
            try
            {
                OpenConnection();
                string query = "update Salary set idSalaryRecord = @idSalaryRecord where month(salaryMonth) = @month and year(salaryMonth) = @year "
                                + "and idEmployee in (select idEmployee from Employee where isDeleted = 0)";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@idSalaryRecord", id.ToString());
                cmd.Parameters.AddWithValue("@month", month.ToString());
                cmd.Parameters.AddWithValue("@year", year.ToString());
                int rs = cmd.ExecuteNonQuery();
                if (rs < 1)
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
        public bool UpdateQuantity(Salary salary)
        {
            try
            {
                OpenConnection();
                string query = "update Salary  set numOfShift=@numOfShift,numOfFault=@numOfFault where idEmployee= @idEmployee and month(salaryMonth) = @month and year(salaryMonth) = @year";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@numOfShift", salary.NumOfShift.ToString());
                cmd.Parameters.AddWithValue("@numOfFault", salary.NumOfFault.ToString());
                cmd.Parameters.AddWithValue("@idEmployee", salary.IdEmployee.ToString());
                cmd.Parameters.AddWithValue("@month", salary.SalaryMonth.Month.ToString());
                cmd.Parameters.AddWithValue("@year", salary.SalaryMonth.Year.ToString());
                int rs = cmd.ExecuteNonQuery();
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
        public bool UpdateTotalSalary(Salary salary)
        {
            try
            {
                OpenConnection();
                string query = "update Salary  set totalSalary=@totalSalary where idEmployee=@idEmployee and month(salaryMonth) = @month and year(salaryMonth) = @year";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@totalSalary", salary.TotalSalary.ToString());
                cmd.Parameters.AddWithValue("@idEmployee", salary.IdEmployee.ToString());
                cmd.Parameters.AddWithValue("@month", salary.SalaryMonth.Month.ToString());
                cmd.Parameters.AddWithValue("@year", salary.SalaryMonth.Year.ToString());
                int rs = cmd.ExecuteNonQuery();
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
        public bool IsExist(string idEmployee, DateTime date)
        {
            try
            {
                OpenConnection();
                string query = @"select * from Salary where idEmployee = @idEmployee and  month(salaryMonth) = @month and year(salaryMonth) = @year";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@idEmployee", idEmployee.ToString());
                cmd.Parameters.AddWithValue("@month", date.Month.ToString());
                cmd.Parameters.AddWithValue("@year", date.Year.ToString());
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                if (dt.Rows.Count < 1)
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
                CustomMessageBox.Show("Lỗi!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            finally
            {
                CloseConnection();
            }
        }
        public bool IsExistIdSalaryRecord(string month, string year)
        {
            try
            {
                OpenConnection();
                string query = @"select distinct(idSalaryRecord) from Salary where month(salaryMonth) = @month and year(salaryMonth) = @year and idSalaryRecord is not null";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@month", month);
                cmd.Parameters.AddWithValue("@year", year);
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                if (dt.Rows.Count < 1)
                    return false;
                else
                {
                    return true;
                }
            }
            catch
            {
                CustomMessageBox.Show("Lỗi!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            finally
            {
                CloseConnection();
            }
        }

        public long GetTotalSalary(string idEmployee, string month, string year)
        {
            try
            {
                OpenConnection();
                string query = "select totalSalary from Salary where idEmployee = @idEmployee and month(salaryMonth) = @month and year(salaryMonth) = @year";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@idEmployee", idEmployee);
                cmd.Parameters.AddWithValue("@month", month);
                cmd.Parameters.AddWithValue("@year", year);
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                if (dt.Rows[0].ItemArray[0].ToString() != "")
                {
                    return long.Parse(dt.Rows[0].ItemArray[0].ToString());
                }
                else
                {
                    return -1;
                }
            }
            catch
            {
                return -1;
            }
            finally
            {
                CloseConnection();
            }
        }
        public List<Salary> GetSalaryOfEmployee(string month, string year) // Lấy lương của những nhân viên đang làm việc
        {
            List<Salary> salaries = new List<Salary>();
            try
            {
                OpenConnection();
                string query = "select Salary.idEmployee, Salary.numOfShift, Salary.numOfFault, Salary.totalSalary , Salary.salaryMonth, Salary.idSalaryRecord from Salary " +
                                "join Employee on Salary.idEmployee = Employee.idEmployee" +
                                 " where Employee.isDeleted = 0 and month(Salary.salaryMonth) = @month and year(Salary.salaryMonth) =@year";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@month", month);
                cmd.Parameters.AddWithValue("@year", year);
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    int idSalaryRecord = -1;
                    if (dt.Rows[i].ItemArray[5].ToString() != "")
                    {
                        idSalaryRecord = int.Parse(dt.Rows[i].ItemArray[5].ToString());
                    }
                    Salary salary = new Salary(int.Parse(dt.Rows[i].ItemArray[0].ToString()), int.Parse(dt.Rows[i].ItemArray[1].ToString()),
                        int.Parse(dt.Rows[i].ItemArray[2].ToString()), long.Parse(dt.Rows[i].ItemArray[3].ToString())
                        , DateTime.Parse(dt.Rows[i].ItemArray[4].ToString()), idSalaryRecord);
                    salaries.Add(salary);
                }
            }
            catch
            {
                CustomMessageBox.Show("Lỗi!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                CloseConnection();
            }
            return salaries;
        }

        public List<Salary> GetSalaryInfoById(string idSalaryRecord)
        {
            List<Salary> listSalaryInfo = new List<Salary>();
            try
            {
                OpenConnection();
                string queryString = "select * from Salary where idSalaryRecord = " + idSalaryRecord;
                SqlCommand command = new SqlCommand(queryString, conn);
                SqlDataAdapter adapter = new SqlDataAdapter(command);
                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);
                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                    Salary salary = new Salary(int.Parse(dataTable.Rows[i].ItemArray[0].ToString()), int.Parse(dataTable.Rows[i].ItemArray[1].ToString()),
                        int.Parse(dataTable.Rows[i].ItemArray[2].ToString()), long.Parse(dataTable.Rows[i].ItemArray[3].ToString()),
                        DateTime.Parse(dataTable.Rows[i].ItemArray[4].ToString()), int.Parse(dataTable.Rows[i].ItemArray[5].ToString()));
                    listSalaryInfo.Add(salary);
                }
                return listSalaryInfo;
            }
            catch
            {
                return new List<Salary>();
            }
            finally
            {
                CloseConnection();
            }
        }
    }
}