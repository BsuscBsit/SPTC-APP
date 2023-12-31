﻿using SPTC_APP.Objects;
using SPTC_APP.View.Controls;
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
    /// Interaction logic for AddRecap.xaml
    /// </summary>
    public partial class AddRecap : Window
    {
        private Recapitulations recaps;
        private string closingMSG;
        public AddRecap(Recapitulations recap)
        {
            InitializeComponent();
            this.recaps = recap;
            ContentRendered += (sender, e) => { AppState.WindowsCounter(true, sender); recaps?.Hide(); };
            Closed += (sender, e) => { AppState.WindowsCounter(false, sender); };
            cbTitle.ItemsSource = AppState.LIST_RECAP;
            DraggingHelper.DragWindow(topBar);
        }
        protected override void OnClosing(CancelEventArgs e)
        {
            recaps?.Show();
            if(!string.IsNullOrEmpty(closingMSG))
                recaps.displayToast(closingMSG);

            base.OnClosing(e);
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            double amount = Double.Parse(tbContent.Text);
            string txt = cbTitle.SelectedValue?.ToString();
            if (txt?.Length > 0)
            {
                Recap recap = new Recap(txt, amount);
                recap.Save();
                recaps.UpdateRecap();
                closingMSG = "Record successfully added.";
                this.Close();
            } else
            {
                ControlWindow.ShowStatic("Found Empty Fields", "Some required fields are empty.\n Please fill them up before continuing.", Icons.NOTIFY);
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            closingMSG = "No record was added.\n Action canceled.";
            this.Close();
        }
    }
}
