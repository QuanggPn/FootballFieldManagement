﻿using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using FootballFieldManagement.Views;
using FootballFieldManagement.Resources.UserControls;
using System.Windows.Media;
using FootballFieldManagement.Models;
using FootballFieldManagement.DAL;
using System.Linq;
using System;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Windows.Threading;
using Microsoft.Win32;
using System.Data.OleDb;

namespace FootballFieldManagement.ViewModels
{
    class HomeViewModel : BaseViewModel
    {
        public ICommand LogOutCommand { get; set; }
        public ICommand SwitchTabCommand { get; set; }

        public ICommand E_LoadCommand { get; set; }
        public ICommand E_AddCommand { get; set; }
        public ICommand E_SetSalaryCommand { get; set; }
        public ICommand E_CalculateSalaryCommand { get; set; }

        public ICommand GetUidCommand { get; set; }

        public ICommand S_SaveBtnFieldInfoCommand { get; set; }
        public ICommand S_SaveFieldInfoCommand { get; set; }
        public ICommand S_EnableBtnSavePassCommand { get; set; }
        public ICommand S_SaveNewPasswordCommand { get; set; }
        public ICommand OpenCheckAttendanceWindowCommand { get; set; }
        public StackPanel Stack { get => stack; set => stack = value; }

        private StackPanel stack = new StackPanel();
        private string uid;
        public HomeViewModel()
        {
            LogOutCommand = new RelayCommand<Window>((parameter) => true, (parameter) => parameter.Close());
            SwitchTabCommand = new RelayCommand<HomeWindow>((parameter) => true, (parameter) => SwitchTab(parameter));
            GetUidCommand = new RelayCommand<Button>((parameter) => true, (parameter) => uid = parameter.Uid);

            E_LoadCommand = new RelayCommand<HomeWindow>((parameter) => true, (parameter) => LoadTabEmployee(parameter));
            E_AddCommand = new RelayCommand<HomeWindow>((parameter) => true, (parameter) => AddEmployee(parameter));
            E_SetSalaryCommand = new RelayCommand<Window>((parameter) => true, (parameter) => OpenSetSalaryWindow());
            E_CalculateSalaryCommand = new RelayCommand<HomeWindow>((parameter) => true, (parameter) => CalculateSalary(parameter));

            S_SaveBtnFieldInfoCommand = new RelayCommand<HomeWindow>((parameter) => true, (parameter) => EnableSaveButtonFieldInfo(parameter));
            S_EnableBtnSavePassCommand = new RelayCommand<HomeWindow>((parameter) => true, (parameter) => EnableButtonSavePass(parameter));
            S_SaveFieldInfoCommand = new RelayCommand<HomeWindow>((parameter) => true, (parameter) => SaveFieldInfo(parameter));
            S_SaveNewPasswordCommand = new RelayCommand<HomeWindow>((parameter) => true, (parameter) => SaveNewPassword(parameter));

            OpenCheckAttendanceWindowCommand = new RelayCommand<Window>((parameter) => true, (parameter) => OpenCheckAttendanceWindow(parameter));
        }

        public void SaveNewPassword(HomeWindow parameter)
        {

            if (MD5Hash(parameter.pwbOldPassword.Password) == CurrentAccount.Password)
            {
                MessageBoxResult result = CustomMessageBox.Show("Xác nhận đổi mật khẩu?", "Thông báo", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    if (parameter.pwbNewPassword.Password == parameter.pwbConfirmedPassword.Password)
                    {
                        if (AccountDAL.Instance.UpdatePassword(CurrentAccount.IdAccount.ToString(), MD5Hash(parameter.pwbNewPassword.Password)))
                        {
                            CurrentAccount.Password = MD5Hash(parameter.pwbNewPassword.Password);
                            CustomMessageBox.Show("Đổi mật khẩu thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                            parameter.pwbOldPassword.Password = null;
                            parameter.pwbNewPassword.Password = null;
                            parameter.pwbConfirmedPassword.Password = null;
                        }
                        else
                        {
                            CustomMessageBox.Show("Đổi mật khẩu thất bại!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                    else
                    {
                        CustomMessageBox.Show("Nhập mật khẩu xác thực không khớp!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
            }
            else
            {
                CustomMessageBox.Show("Nhập mật khẩu hiện tại không đúng!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

        }
        public void SaveFieldInfo(HomeWindow homeWindow)
        {
            MessageBoxResult result = CustomMessageBox.Show("Xác nhận sửa tên sân?", "Thông báo", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                homeWindow.txbFieldName.Text = homeWindow.txtFieldName.Text;
                SQLConnection connection = new SQLConnection();
                try
                {
                    connection.conn.Open();

                    string queryString = "update Information set fieldName = @fieldName, address = @address, phoneNumber = @phoneNumber";
                    SqlCommand command = new SqlCommand(queryString, connection.conn);
                    command.Parameters.AddWithValue("@fieldName", homeWindow.txtFieldName.Text);
                    command.Parameters.AddWithValue("@address", homeWindow.txtAdressInfo.Text);
                    command.Parameters.AddWithValue("@phoneNumber", homeWindow.txtPhoneNumberInfo.Text);
                    int rs = command.ExecuteNonQuery();
                    if (rs == 1)
                    {
                        CustomMessageBox.Show("Sửa thông tin sân thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                    }
                    else
                    {
                        CustomMessageBox.Show("Thực hiện thất bại!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                catch
                {
                    CustomMessageBox.Show("Thực hiện thất bại!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                finally
                {
                    connection.conn.Close();
                }
            }
        }
        public void EnableSaveButtonFieldInfo(HomeWindow parameter)
        {
            bool isEnable = string.IsNullOrEmpty(parameter.txtFieldName.Text) || string.IsNullOrEmpty(parameter.txtPhoneNumberInfo.Text) || string.IsNullOrEmpty(parameter.txtAdressInfo.Text);
            parameter.btnSaveFieldInfo.IsEnabled = !isEnable;
        }
        public void EnableButtonSavePass(HomeWindow parameter)
        {
            bool isEnable = string.IsNullOrEmpty(parameter.pwbOldPassword.Password) || string.IsNullOrEmpty(parameter.pwbNewPassword.Password) || string.IsNullOrEmpty(parameter.pwbConfirmedPassword.Password);
            parameter.btnSavePassword.IsEnabled = !isEnable;
        }
        public void SwitchTab(HomeWindow parameter)
        {
            int index = int.Parse(uid);

            parameter.grdCursor.Margin = new Thickness(0, (172 + 65 * index), 40, 0);

            parameter.grdBody_Goods.Visibility = Visibility.Hidden;
            parameter.grdBody_Business.Visibility = Visibility.Hidden;
            parameter.grdBody_Home.Visibility = Visibility.Hidden;
            parameter.grdBody_Employee.Visibility = Visibility.Hidden;
            parameter.grdBody_Report.Visibility = Visibility.Hidden;
            parameter.grdBody_Field.Visibility = Visibility.Hidden;
            parameter.grdBody_Setting.Visibility = Visibility.Hidden;

            parameter.btnHome.Foreground = (Brush)new BrushConverter().ConvertFrom("#FF282828");
            parameter.btnBusiness.Foreground = (Brush)new BrushConverter().ConvertFrom("#FF282828");
            parameter.btnGoods.Foreground = (Brush)new BrushConverter().ConvertFrom("#FF282828");
            parameter.btnEmployee.Foreground = (Brush)new BrushConverter().ConvertFrom("#FF282828");
            parameter.btnField.Foreground = (Brush)new BrushConverter().ConvertFrom("#FF282828");
            parameter.btnReport.Foreground = (Brush)new BrushConverter().ConvertFrom("#FF282828");
            parameter.btnSetting.Foreground = (Brush)new BrushConverter().ConvertFrom("#FF282828");

            parameter.icnHome.Foreground = (Brush)new BrushConverter().ConvertFrom("#FF282828");
            parameter.icnBusiness.Foreground = (Brush)new BrushConverter().ConvertFrom("#FF282828");
            parameter.icnGoods.Foreground = (Brush)new BrushConverter().ConvertFrom("#FF282828");
            parameter.icnEmployee.Foreground = (Brush)new BrushConverter().ConvertFrom("#FF282828");
            parameter.icnField.Foreground = (Brush)new BrushConverter().ConvertFrom("#FF282828");
            parameter.icnReport.Foreground = (Brush)new BrushConverter().ConvertFrom("#FF282828");
            parameter.icnSetting.Foreground = (Brush)new BrushConverter().ConvertFrom("#FF282828");

            switch (index)
            {
                case 0:
                    ReportViewModel reportViewModel = new ReportViewModel(parameter);
                    parameter.grdBody_Home.Visibility = Visibility.Visible;
                    parameter.btnHome.Foreground = (Brush)new BrushConverter().ConvertFrom("#FF1976D2");
                    parameter.icnHome.Foreground = (Brush)new BrushConverter().ConvertFrom("#FF1976D2");
                    break;
                case 1:
                    BusinessViewModel businessViewModel = new BusinessViewModel(parameter);
                    parameter.grdBody_Business.Visibility = Visibility.Visible;
                    parameter.btnBusiness.Foreground = (Brush)new BrushConverter().ConvertFrom("#FF1976D2");
                    parameter.icnBusiness.Foreground = (Brush)new BrushConverter().ConvertFrom("#FF1976D2");
                    break;
                case 2:
                    parameter.cboViews.SelectedIndex = -1;
                    DispatcherTimer timer_F = new DispatcherTimer
                    {
                        Interval = TimeSpan.FromMilliseconds(10)
                    };
                    timer_F.Tick += (s, e) =>
                    {
                        parameter.cboViews.SelectedIndex = 1;
                        timer_F.Stop();
                    };
                    timer_F.Start();
                    parameter.grdBody_Field.Visibility = Visibility.Visible;
                    parameter.btnField.Foreground = (Brush)new BrushConverter().ConvertFrom("#FF1976D2");
                    parameter.icnField.Foreground = (Brush)new BrushConverter().ConvertFrom("#FF1976D2");
                    break;
                case 3:
                    GoodsViewModel goodsViewModel = new GoodsViewModel();
                    goodsViewModel.LoadStkGoods(parameter);
                    parameter.grdBody_Goods.Visibility = Visibility.Visible;
                    parameter.btnGoods.Foreground = (Brush)new BrushConverter().ConvertFrom("#FF1976D2");
                    parameter.icnGoods.Foreground = (Brush)new BrushConverter().ConvertFrom("#FF1976D2");
                    break;
                case 4:
                    parameter.grdBody_Employee.Visibility = Visibility.Visible;
                    parameter.btnEmployee.Foreground = (Brush)new BrushConverter().ConvertFrom("#FF1976D2");
                    parameter.icnEmployee.Foreground = (Brush)new BrushConverter().ConvertFrom("#FF1976D2");
                    break;
                case 5:
                    parameter.cboSelectPeriodRp.SelectedIndex = -1;
                    parameter.cboSelectTimeRp.SelectedIndex = -1;
                    parameter.cboSelectViewMode.SelectedIndex = -1;
                    parameter.cboSelectViewModeStR.SelectedIndex = -1;
                    parameter.cboSelectYearSalaryRecord.SelectedIndex = -1;
                    DispatcherTimer timer = new DispatcherTimer
                    {
                        Interval = TimeSpan.FromMilliseconds(5)
                    };
                    timer.Tick += (s, e) =>
                    {
                        parameter.cboSelectPeriodRp.SelectedIndex = 0;
                        parameter.cboSelectTimeRp.SelectedIndex = DateTime.Now.Month - 1;
                        timer.Stop();
                    };
                    timer.Start();
                    parameter.grdBody_Report.Visibility = Visibility.Visible;
                    parameter.btnReport.Foreground = (Brush)new BrushConverter().ConvertFrom("#FF1976D2");
                    parameter.icnReport.Foreground = (Brush)new BrushConverter().ConvertFrom("#FF1976D2");
                    break;
                case 6:
                    parameter.grdBody_Setting.Visibility = Visibility.Visible;
                    parameter.btnSetting.Foreground = (Brush)new BrushConverter().ConvertFrom("#FF1976D2");
                    parameter.icnSetting.Foreground = (Brush)new BrushConverter().ConvertFrom("#FF1976D2");
                    break;
                default:
                    break;
            }
        }
        public void OpenCheckAttendanceWindow(Window parameter)
        {
            CheckAttendanceWindow wdCheckAttendance = new CheckAttendanceWindow();
            wdCheckAttendance.ShowDialog();
            parameter.Show();
        }
        //Tab employee
        public void CalculateSalary(HomeWindow home)
        {
            foreach (string item in EmployeeDAL.Instance.GetAllPosition())
            {
                SalarySetting salarySetting = SalarySettingDAL.Instance.GetSalarySettings(item);
                if (salarySetting == null)
                {
                    CustomMessageBox.Show("Vui lòng thiết lập lương cho '" + item + "'!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    SetSalaryWindow wdSetSalary = new SetSalaryWindow();
                    wdSetSalary.cboTypeEmployee.Text = item;
                    wdSetSalary.ShowDialog();
                    return;
                }
            }
            bool success = true;
            foreach (var salary in SalaryDAL.Instance.GetSalaryByMonth(DateTime.Now.Month.ToString(), DateTime.Now.Year.ToString()))
            {
                int workdays = AttendanceDAL.Instance.GetCount(salary.IdEmployee.ToString());
                string positionEmployee = EmployeeDAL.Instance.GetPosition(salary.IdEmployee.ToString());
                SalarySetting salarySetting = SalarySettingDAL.Instance.GetSalarySettings(positionEmployee);
                if (workdays < 0)
                {
                    return;
                }
                //Lấy ra cái salary setting có loại nhân viên trùng khớp với idEmployee
                if (workdays <= salarySetting.StandardWorkDays)
                {
                    salary.TotalSalary = (salarySetting.SalaryBase / salarySetting.StandardWorkDays) * workdays + salary.NumOfShift * salarySetting.MoneyPerShift - salary.NumOfFault * salarySetting.MoneyPerFault;
                }
                else
                {
                    salary.TotalSalary = salarySetting.SalaryBase + salary.NumOfShift * salarySetting.MoneyPerShift - salary.NumOfFault * salarySetting.MoneyPerFault;
                }
                if (salary.TotalSalary < 0)
                {
                    salary.TotalSalary = 0;
                }
                if (!SalaryDAL.Instance.UpdateTotalSalary(salary))
                {
                    success = false;
                }
            }
            if (success)
            {
                CustomMessageBox.Show("Tính lương thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                for (int i = 0; i < home.stkEmployee.Children.Count; i++)
                {
                    EmployeeControl control = (EmployeeControl)home.stkEmployee.Children[i];
                    control.txbTotalSalary.Text = string.Format("{0:N0}", SalaryDAL.Instance.GetTotalSalary(control.txbId.Text, DateTime.Now.Month.ToString(), DateTime.Now.Year.ToString()));
                }
            }
            else
            {
                CustomMessageBox.Show("Tính lương thất bại!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        public void OpenSetSalaryWindow()
        {
            SetSalaryWindow setSalaryWindow = new SetSalaryWindow();
            setSalaryWindow.ShowDialog();
        }

        public void AddEmployee(HomeWindow parameter)
        {
            stack = parameter.stkEmployee;
            AddEmployeeWindow addEmployee = new AddEmployeeWindow();
            try
            {
                addEmployee.txtIDEmployee.Text = (EmployeeDAL.Instance.GetMaxIdEmployee() + 1).ToString();
            }
            catch
            {
                addEmployee.txtIDEmployee.Text = "1";
            }
            addEmployee.btnSave.Content = "Thêm";
            if (CurrentAccount.Type == 1)
                addEmployee.cboPositionManage.IsEnabled = false;

            addEmployee.txtName.Text = null;
            addEmployee.txtAddress.Text = null;
            addEmployee.txtTelephoneNumber.Text = null;
            addEmployee.ShowDialog();
            if (addEmployee.isAdded.Text == "1")
            {
                LoadEmployeesToView(parameter);
            }
        }
        public void LoadTabEmployee(HomeWindow home)
        {
            LoadEmployeesToView(home);
            if (SalaryDAL.Instance.IsExistIdSalaryRecord(DateTime.Now.Month.ToString(), DateTime.Now.Year.ToString()))
            {
                home.btnCalculateSalary.IsEnabled = false;
            }
        }
        public void LoadEmployeesToView(HomeWindow homeWindow)
        {

            int i = 1;
            homeWindow.stkEmployee.Children.Clear();
            bool flag = false;
            List<Salary> salaries = SalaryDAL.Instance.GetSalaryByMonth(DateTime.Now.Month.ToString(), DateTime.Now.Year.ToString());
            foreach (var employee in EmployeeDAL.Instance.ConvertDBToList())
            {
                EmployeeControl temp = new EmployeeControl();
                flag = !flag;
                if (flag)
                {
                    temp.grdMain.Background = (Brush)new BrushConverter().ConvertFromString("#FFFFFF");
                }
                temp.txbSerial.Text = i.ToString();
                i++;
                temp.nsNumOfFault.IsEnabled = false;
                temp.nsNumOfShift.IsEnabled = false;
                // load number fault and overtime and salary
                if (salaries.Count < 1)
                {
                    Salary tmp = new Salary();
                    tmp.IdEmployee = employee.IdEmployee;
                    tmp.NumOfFault = 0;
                    tmp.NumOfShift = 0;
                    tmp.TotalSalary = -1;
                    tmp.SalaryMonth = DateTime.Now.Date;
                    SalaryDAL.Instance.AddIntoDB(tmp);
                    temp.nsNumOfShift.Text = 0;
                    temp.nsNumOfFault.Text = 0;
                    temp.txbTotalSalary.Text = "0";
                }
                foreach (var salary in SalaryDAL.Instance.GetUnPaidSalary(employee.IdEmployee.ToString(), DateTime.Now.Month.ToString(), DateTime.Now.Year.ToString()))
                {
                    if (employee.IdEmployee == salary.IdEmployee && salary.IdSalaryRecord == -1)
                    {
                        temp.nsNumOfShift.IsEnabled = true;
                        temp.nsNumOfFault.IsEnabled = true;
                        temp.nsNumOfShift.Text = decimal.Parse(salary.NumOfShift.ToString());
                        temp.nsNumOfFault.Text = decimal.Parse(salary.NumOfFault.ToString());
                        if (salary.TotalSalary == -1)
                        {
                            temp.txbTotalSalary.Text = "0";
                        }
                        else
                        {
                            temp.txbTotalSalary.Text = string.Format("{0:n0}", salary.TotalSalary);
                        }
                        break;
                    }
                }
                temp.txbId.Text = employee.IdEmployee.ToString();
                temp.txbName.Text = employee.Name.ToString();
                temp.txbPosition.Text = employee.Position.ToString();
                if (CurrentAccount.Type == 1)
                {
                    if (employee.Position == "Nhân viên quản lý")
                    {
                        temp.btnEditEmployee.IsEnabled = false;
                    }
                }
                homeWindow.stkEmployee.Children.Add(temp);
            }
        }
    }
}
