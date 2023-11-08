﻿using SPTC_APP.Objects;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SPTC_APP.View.Pages.Input
{
    /// <summary>
    /// Interaction logic for AddViolationType.xaml
    /// </summary>
    public partial class AddViolationType : Window
    {
        public AddViolationType()
        {
            InitializeComponent();
            ContentRendered += (sender, e) => { AppState.WindowsCounter(true, sender); AppState.mainwindow?.Hide(); };
            Closed += (sender, e) => { AppState.WindowsCounter(false, sender); };
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            AppState.mainwindow?.Show();

            base.OnClosing(e);
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }


        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if(tbTitle.Text.Length > 0 && tbNumDays.Text.Length > 0)
            {
                int numofdays = 0;
                if (Int32.TryParse(tbNumDays.Text, out numofdays))
                {
                    ViolationType violationType = new ViolationType();
                    violationType.WriteInto(tbTitle.Text, tbDescription.Text, numofdays, General.DRIVER);
                    violationType.Save();
                    this.Close();
                } else
                {
                    ControlWindow.ShowStatic("Input Error", "Some required fields in empty", Icons.DEFAULT);
                }
            } else
            {
                ControlWindow.ShowStatic("Input Incomplete", "Some required fields in empty", Icons.DEFAULT);
            }
        }
    }
}
