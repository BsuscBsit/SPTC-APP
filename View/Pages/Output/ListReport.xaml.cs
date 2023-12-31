﻿using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Printing;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Xps;
using System.Windows.Xps.Packaging;
using MySql.Data.MySqlClient;
using SPTC_APP.Database;
using SPTC_APP.View.Controls;
using Table = SPTC_APP.Database.Table;

namespace SPTC_APP.View.Pages.Output
{
    public partial class ListReport : Window
    {
        //CREATE QUERY HERE
        public static string PAYMENTS = $"SELECT * FROM {Table.PAYMENT_DETAILS}";

        public static string ACTIVE_SHORT = $"SELECT * FROM {Table.LOAN} AS l LEFT JOIN {Table.FRANCHISE} as f ON l.{Field.FRANCHISE_ID} = f.{Field.ID} LEFT JOIN {Table.OPERATOR} AS o ON f.{Field.OPERATOR_ID} = o.{Field.ID} LEFT JOIN {Table.NAME} AS n ON o.{Field.NAME_ID} = n.{Field.ID} WHERE {Field.AMOUNT} > 0 AND l.{Field.IS_FULLY_PAID} = 0 AND l.isDeleted = 0 AND l.details = \"SHORT TERM\"";
        public static string ACTIVE_LONG = $"SELECT * FROM {Table.LONG_TERM_LOAN} AS l LEFT JOIN {Table.FRANCHISE} as f ON l.{Field.FRANCHISE_ID} = f.{Field.ID} LEFT JOIN {Table.OPERATOR} AS o ON f.{Field.OPERATOR_ID} = o.{Field.ID} LEFT JOIN {Table.NAME} AS n ON o.{Field.NAME_ID} = n.{Field.ID} WHERE {Field.AMOUNT} > 0 AND l.{Field.IS_FULLY_PAID} = 0 AND l.isDeleted = 0";
        public static string ACTIVE_EMERGENCY = $"SELECT * FROM {Table.LOAN} AS l LEFT JOIN {Table.FRANCHISE} as f ON l.{Field.FRANCHISE_ID} = f.{Field.ID} LEFT JOIN {Table.OPERATOR} AS o ON f.{Field.OPERATOR_ID} = o.{Field.ID} LEFT JOIN {Table.NAME} AS n ON o.{Field.NAME_ID} = n.{Field.ID} WHERE {Field.AMOUNT} > 0 AND l.{Field.IS_FULLY_PAID} = 0 AND l.isDeleted = 0 AND l.details = \"EMERGENCY\"";

        public static string LIST_FRANCHISE(int month, int year) => $"SELECT * FROM {Table.FRANCHISE} AS f LEFT JOIN {Table.OPERATOR} AS o ON f.{Field.OPERATOR_ID} = o.{Field.ID} LEFT JOIN {Table.NAME} AS n ON o.{Field.NAME_ID} = n.{Field.ID} LEFT JOIN {Table.SHARE_CAPITAL} AS sc ON sc.{Field.FRANCHISE_ID} = f.id WHERE YEAR(f.{Field.BUYING_DATE}) = {year} AND MONTH(f.{Field.BUYING_DATE}) = {month} AND f.isDeleted = 0";
        public static string LIST_OPERATOR(int month, int year) => $"SELECT * FROM {Table.FRANCHISE} AS f LEFT JOIN {Table.OPERATOR} AS o ON f.{Field.OPERATOR_ID} = o.{Field.ID} LEFT JOIN {Table.NAME} AS n ON o.{Field.NAME_ID} = n.{Field.ID} LEFT JOIN {Table.SHARE_CAPITAL} AS sc ON sc.{Field.FRANCHISE_ID} = f.id WHERE YEAR(f.{Field.BUYING_DATE}) = {year} AND MONTH(f.{Field.BUYING_DATE}) = {month} AND f.isDeleted = 0";
        public static string LIST_DRIVER(int month, int year) => $"SELECT * FROM {Table.FRANCHISE} AS f LEFT JOIN {Table.DRIVER} AS o ON f.{Field.DRIVER_ID} = o.{Field.ID} LEFT JOIN {Table.NAME} AS n ON o.{Field.NAME_ID} = n.{Field.ID} LEFT JOIN {Table.SHARE_CAPITAL} AS sc ON sc.{Field.FRANCHISE_ID} = f.id WHERE YEAR(f.{Field.BUYING_DATE}) = {year} AND MONTH(f.{Field.BUYING_DATE}) = {month} AND f.isDeleted = 0";
        public static string LIST_VIOLATION(int month, int year) => $"SELECT * FROM {Table.VIOLATION} AS v LEFT JOIN {Table.FRANCHISE} AS f ON v.{Field.FRANCHISE_ID} = f.{Field.ID} LEFT JOIN {Table.VIOLATION_TYPE} AS vt ON vt.{Field.ID} = v.{Field.VIOLATION_TYPE_ID} LEFT JOIN {Table.DRIVER} AS o ON f.{Field.DRIVER_ID} = o.{Field.ID} LEFT JOIN {Table.NAME} AS n ON o.{Field.NAME_ID} = n.{Field.ID}  WHERE YEAR(v.{Field.DATE}) = {year} AND MONTH(v.{Field.DATE}) = {month} AND v.isDeleted=0";
        public static string LIST_IDHISTORY(int month, int year) => $"SELECT * FROM {Table.IDHISTORY} AS ih LEFT JOIN {Table.FRANCHISE} AS f ON ih.{Field.FRANCHISE_ID} = f.{Field.ID} LEFT JOIN tbl_driver AS d ON ih.owner_id = d.id AND ih.entity_type=\"driver\" LEFT JOIN tbl_operator AS o ON ih.owner_id = o.id AND ih.entity_type=\"operator\" LEFT JOIN tbl_name AS n ON d.name_id = n.id OR o.name_id = n.id WHERE YEAR(ih.{Field.DATE}) = {year} AND MONTH(ih.{Field.DATE}) = {month} AND f.isDeleted = 0";


        public static string LIST_FRANCHISE_ALL = $"SELECT * FROM {Table.FRANCHISE} AS f LEFT JOIN {Table.OPERATOR} AS o ON f.{Field.OPERATOR_ID} = o.{Field.ID} LEFT JOIN {Table.NAME} AS n ON o.{Field.NAME_ID} = n.{Field.ID} LEFT JOIN {Table.SHARE_CAPITAL} AS sc ON sc.{Field.FRANCHISE_ID} = f.id WHERE f.isDeleted = 0";
        public static string LIST_OPERATOR_ALL = $"SELECT * FROM {Table.FRANCHISE} AS f LEFT JOIN {Table.OPERATOR} AS o ON f.{Field.OPERATOR_ID} = o.{Field.ID} LEFT JOIN {Table.NAME} AS n ON o.{Field.NAME_ID} = n.{Field.ID} LEFT JOIN {Table.SHARE_CAPITAL} AS sc ON sc.{Field.FRANCHISE_ID} = f.id WHERE f.isDeleted = 0";
        public static string LIST_DRIVER_ALL = $"SELECT * FROM {Table.FRANCHISE} AS f LEFT JOIN {Table.DRIVER} AS o ON f.{Field.DRIVER_ID} = o.{Field.ID} LEFT JOIN {Table.NAME} AS n ON o.{Field.NAME_ID} = n.{Field.ID} LEFT JOIN {Table.SHARE_CAPITAL} AS sc ON sc.{Field.FRANCHISE_ID} = f.id WHERE f.isDeleted = 0";
        public static string LIST_VIOLATION_ALL = $"SELECT * FROM {Table.VIOLATION} AS v LEFT JOIN {Table.FRANCHISE} AS f ON v.{Field.FRANCHISE_ID} = f.{Field.ID} LEFT JOIN {Table.VIOLATION_TYPE} AS vt ON vt.{Field.ID} = v.{Field.VIOLATION_TYPE_ID} LEFT JOIN {Table.DRIVER} AS o ON f.{Field.DRIVER_ID} = o.{Field.ID} LEFT JOIN {Table.NAME} AS n ON o.{Field.NAME_ID} = n.{Field.ID}  WHERE v.isDeleted=0";
        public static string LIST_IDHISTORY_ALL = $"SELECT * FROM {Table.IDHISTORY} AS ih LEFT JOIN {Table.FRANCHISE} AS f ON ih.{Field.FRANCHISE_ID} = f.{Field.ID} LEFT JOIN tbl_driver AS d ON ih.owner_id = d.id AND ih.entity_type=\"driver\" LEFT JOIN tbl_operator AS o ON ih.owner_id = o.id AND ih.entity_type=\"operator\" LEFT JOIN tbl_name AS n ON d.name_id = n.id OR o.name_id = n.id WHERE f.isDeleted = 0";

        // May date filtering ito (month and year) however pwede din wala.
        public static string PAYMENT_SHORT(int month, int year) => $"SELECT * FROM tbl_payment_details AS pd LEFT JOIN {Table.LOAN} AS l ON pd.ledger_id = l.id AND pd.ledger_type = \"LOAN\" LEFT JOIN {Table.FRANCHISE} AS f ON f.{Field.ID} = l.{Field.FRANCHISE_ID} WHERE YEAR(pd.{Field.DATE}) = {year} AND MONTH(pd.{Field.DATE}) = {month} AND pd.ledger_id <> -1 AND pd.ledger_type = \"LOAN\" AND l.details = \"SHORT TERM\" AND (l.isDeleted = 0 OR pd.isDeleted = 0)";
        public static string PAYMENT_LONG(int month, int year) => $"SELECT * FROM tbl_payment_details AS pd LEFT JOIN {Table.LONG_TERM_LOAN} AS l ON pd.ledger_id = l.id AND pd.ledger_type = \"LONGTERMLOAN\" LEFT JOIN {Table.FRANCHISE} AS f ON f.{Field.ID} = l.{Field.FRANCHISE_ID} WHERE YEAR(pd.{Field.DATE}) = {year} AND MONTH(pd.{Field.DATE}) = {month} AND pd.ledger_id <> -1 AND pd.ledger_type = \"LONGTERMLOAN\" AND l.details = \"LONG TERM\" AND (l.isDeleted = 0 OR pd.isDeleted = 0)";
        public static string PAYMENT_EMERGENCY(int month, int year) => $"SELECT * FROM tbl_payment_details AS pd LEFT JOIN tbl_loan_ledger AS l ON pd.ledger_id = l.id AND pd.ledger_type = \"LOAN\" LEFT JOIN {Table.FRANCHISE} AS f ON f.{Field.ID} = l.{Field.FRANCHISE_ID} WHERE YEAR(pd.{Field.DATE}) = {year} AND MONTH(pd.{Field.DATE}) = {month} AND pd.ledger_id <> -1 AND pd.ledger_type = \"LOAN\" AND l.details = \"EMERGENCY\" AND (l.isDeleted = 0 OR pd.isDeleted = 0)";
        public static string PAYMENT_SHARECAPITAL(int month, int year) => $"SELECT * FROM tbl_payment_details AS pd LEFT JOIN tbl_share_capital_ledger AS l ON pd.ledger_id = l.id AND pd.ledger_type = \"SHARECAPITAl\" LEFT JOIN {Table.FRANCHISE} AS f ON f.{Field.ID} = l.{Field.FRANCHISE_ID} WHERE YEAR(pd.{Field.DATE}) = {year} AND MONTH(pd.{Field.DATE}) = {month} AND pd.ledger_id <> -1 AND pd.ledger_type = \"SHARECAPITAL\" AND (l.isDeleted = 0 OR pd.isDeleted = 0)";

        public static string PAYMENT_SHORT_ALL = $"SELECT * FROM tbl_payment_details AS pd LEFT JOIN {Table.LOAN} AS l ON pd.ledger_id = l.id AND pd.ledger_type = \"LOAN\" LEFT JOIN {Table.FRANCHISE} AS f ON f.{Field.ID} = l.{Field.FRANCHISE_ID} WHERE pd.ledger_id <> -1 AND pd.ledger_type = \"LOAN\" AND l.details = \"SHORT TERM\" AND (l.isDeleted = 0 OR pd.isDeleted = 0)";
        public static string PAYMENT_LONG_ALL = $"SELECT * FROM tbl_payment_details AS pd LEFT JOIN {Table.LONG_TERM_LOAN} AS l ON pd.ledger_id = l.id AND pd.ledger_type = \"LONGTERMLOAN\" LEFT JOIN {Table.FRANCHISE} AS f ON f.{Field.ID} = l.{Field.FRANCHISE_ID} WHERE pd.ledger_id <> -1 AND pd.ledger_type = \"LONGTERMLOAN\" AND l.details = \"LONG TERM\" AND (l.isDeleted = 0 OR pd.isDeleted = 0)";
        public static string PAYMENT_EMERGENCY_ALL = $"SELECT * FROM tbl_payment_details AS pd LEFT JOIN tbl_loan_ledger AS l ON pd.ledger_id = l.id AND pd.ledger_type = \"LOAN\" LEFT JOIN {Table.FRANCHISE} AS f ON f.{Field.ID} = l.{Field.FRANCHISE_ID} WHERE pd.ledger_id <> -1 AND pd.ledger_type = \"LOAN\" AND l.details = \"EMERGENCY\" AND (l.isDeleted = 0 OR pd.isDeleted = 0)";
        public static string PAYMENT_SHARECAPITAL_ALL = $"SELECT * FROM tbl_payment_details AS pd LEFT JOIN tbl_share_capital_ledger AS l ON pd.ledger_id = l.id AND pd.ledger_type = \"SHARECAPITAl\" LEFT JOIN {Table.FRANCHISE} AS f ON f.{Field.ID} = l.{Field.FRANCHISE_ID} WHERE pd.ledger_id <> -1 AND pd.ledger_type = \"SHARECAPITAL\" AND (l.isDeleted = 0 OR pd.isDeleted = 0)";


        public static string LOANS_SHORT(int month, int year) => $"SELECT * FROM {Table.LOAN} AS l LEFT JOIN {Table.FRANCHISE} as f ON l.{Field.FRANCHISE_ID} = f.{Field.ID} LEFT JOIN {Table.OPERATOR} AS o ON f.{Field.OPERATOR_ID} = o.{Field.ID} LEFT JOIN {Table.NAME} AS n ON o.{Field.NAME_ID} = n.{Field.ID} WHERE YEAR(l.{Field.DATE}) = {year} AND MONTH(l.{Field.DATE}) = {month} AND l.isDeleted = 0 AND l.details = \"SHORT TERM\"";
        public static string LOANS_LONG(int month, int year) => $"SELECT * FROM {Table.LONG_TERM_LOAN} AS l LEFT JOIN {Table.FRANCHISE} as f ON l.{Field.FRANCHISE_ID} = f.{Field.ID} LEFT JOIN {Table.OPERATOR} AS o ON f.{Field.OPERATOR_ID} = o.{Field.ID} LEFT JOIN {Table.NAME} AS n ON o.{Field.NAME_ID} = n.{Field.ID} WHERE YEAR(l.{Field.DATE}) = {year} AND MONTH(l.{Field.DATE}) = {month} AND l.isDeleted = 0";
        public static string LOANS_EMERGENCY(int month, int year) => $"SELECT * FROM {Table.LOAN} AS l LEFT JOIN {Table.FRANCHISE} as f ON l.{Field.FRANCHISE_ID} = f.{Field.ID} LEFT JOIN {Table.OPERATOR} AS o ON f.{Field.OPERATOR_ID} = o.{Field.ID} LEFT JOIN {Table.NAME} AS n ON o.{Field.NAME_ID} = n.{Field.ID} WHERE YEAR(l.{Field.DATE}) = {year} AND MONTH(l.{Field.DATE}) = {month} AND l.isDeleted = 0 AND l.details = \"EMERGENCY\"";


        public static string LOANS_SHORT_ALL = $"SELECT * FROM {Table.LOAN} AS l LEFT JOIN {Table.FRANCHISE} as f ON l.{Field.FRANCHISE_ID} = f.{Field.ID} LEFT JOIN {Table.OPERATOR} AS o ON f.{Field.OPERATOR_ID} = o.{Field.ID} LEFT JOIN {Table.NAME} AS n ON o.{Field.NAME_ID} = n.{Field.ID} WHERE l.isDeleted = 0 AND l.details = \"SHORT TERM\"";
        public static string LOANS_LONG_ALL = $"SELECT * FROM {Table.LONG_TERM_LOAN} AS l LEFT JOIN {Table.FRANCHISE} as f ON l.{Field.FRANCHISE_ID} = f.{Field.ID} LEFT JOIN {Table.OPERATOR} AS o ON f.{Field.OPERATOR_ID} = o.{Field.ID} LEFT JOIN {Table.NAME} AS n ON o.{Field.NAME_ID} = n.{Field.ID} WHERE l.isDeleted = 0";
        public static string LOANS_EMERGENCY_ALL = $"SELECT * FROM {Table.LOAN} AS l LEFT JOIN {Table.FRANCHISE} as f ON l.{Field.FRANCHISE_ID} = f.{Field.ID} LEFT JOIN {Table.OPERATOR} AS o ON f.{Field.OPERATOR_ID} = o.{Field.ID} LEFT JOIN {Table.NAME} AS n ON o.{Field.NAME_ID} = n.{Field.ID} WHERE l.isDeleted = 0 AND l.details = \"EMERGENCY\"";
        /*public static string DUE_SHORT;
        public static string DUE_LONG;
        public static string DUE_EMERGENCY;*/

        //USAGE
        /*  ListReport reports = new ListReport(ListReport.PAYMENTS); // PAYMENTS is the MYSQL query
               
        //  Create this columnconfigurations, @ diero for every buttons and reports, complete with the Fields
            List<ColumnConfiguration> columns = new List<ColumnConfiguration>
            {
                new ColumnConfiguration(Field.ID, "ID", minWidth: 50, isNumeric: true, maxWidth:50),
                new ColumnConfiguration(Field.DEPOSIT, "DEPOSIT", minWidth: 50, isNumeric: true, isCenter:true, haspeso:true),
            };
        //  This starts printing all the data
            reports.StartPrint("All Payments", "Here lies payment report", columns);*/

        private readonly string _query;

        int pagenumber;
        int itemcount = 25;

        public ListReport(string query)
        {
            InitializeComponent();
            _query = query;
            
        }
        /// <summary>
        /// e
        /// </summary>
        /// <param name="filename">used in both title and printxps</param>
        /// <param name="description">Use "(page)" to substitute for pagenumber</param>
        /// <param name="columns"></param>
        public void StartPrint(string filename,string description, List<ColumnConfiguration> columns)
        {
            pagenumber = 0;
            int counter = 0;
            while (true)
            {
                DataTable dataSource = GetYourData(_query, pagenumber, itemcount);

                if (dataSource.Rows.Count == 0)
                {
                    if (counter > 0)
                    {
                        ControlWindow.ShowStatic("Print Success", $"{filename} is printed", Icons.NOTIFY);
                    } else
                    {
                        ControlWindow.ShowStatic("Print Warning", $"{filename} is empty", Icons.DEFAULT);
                    }
                    this.Close();
                    break;
                }
                counter++;
                this.Show();

                //HEADER
                sptcTitle.Content = (AppState.SPTC).ToUpper();
                sptcAddress.Content = AppState.ADDRESS;
                sptcEmail.Content = "Email address:   " + AppState.EMAIL;
                sptcCDA.Content = "CDA RED No.:   " + AppState.CDA;
                sptcCIN.Content = "CIN:   " + AppState.CIN;
                sptcTIN.Content = "TIN No.: " + AppState.TIN;


                lblDescription.Content = $"Description: {description}".Replace("(page)", (pagenumber+1).ToString());
                lblTitle.Content = "SPTC REPORT: " + filename.ToUpper();
                lblProfile.Content = $"Generated by: {AppState.USER?.name?.ToString() ?? ""} (SPTC {AppState.USER?.position?.title?.ToString() ?? ""})";
                lblFooter.Content = $"SPTC REPORT: {DateTime.Now.ToString("MMMM dd, yyyy")}       Page: {pagenumber + 1}";

                listGrid.ItemsSource = dataSource.DefaultView;
                new DataGridHelper<Object>(listGrid, columns, true);

                if (!ControlWindow.ShowTwoway("Printing", $"Do you want to continue printing \nPage: {pagenumber + 1} of {filename}", Icons.NOTIFY))
                {
                    this.Close();
                    break;
                }
                
                Print(filename);

                pagenumber++;
            }
        }


        private DataTable GetYourData(string query, int page, int count)
        {
            DataTable dataTable = new DataTable();

            int offset = page * count;

            query = $"{query} LIMIT {offset}, {count};";

            using (MySqlConnection connection = DatabaseConnection.GetConnection())
            {
                connection.Open();

                using (MySqlDataAdapter adapter = new MySqlDataAdapter(query, connection))
                {
                    adapter.Fill(dataTable);
                }
            }

            return dataTable;
        }


        private void Print(string filename)
        {
            string outputPath = AppState.OUTPUT_PATH + filename + ".xps";

            if (!Directory.Exists(AppState.OUTPUT_PATH))
            {
                Directory.CreateDirectory(AppState.OUTPUT_PATH);
            }

            using (XpsDocument xpsDoc = new XpsDocument(outputPath, FileAccess.Write))
            {
                XpsDocumentWriter xpsWriter = XpsDocument.CreateXpsDocumentWriter(xpsDoc);

                PrintTicket printTicket = new PrintTicket();
                printTicket.PageOrientation = PageOrientation.Landscape;
                xpsWriter.Write(reportpage, printTicket);
            }
            this.Hide();

            System.Windows.Controls.PrintDialog printDialog = new System.Windows.Controls.PrintDialog();
            if (printDialog.ShowDialog() == true)
            {
                XpsDocument xpsDocToPrint = new XpsDocument(outputPath, FileAccess.Read);
                FixedDocumentSequence fixedDocumentSequence = xpsDocToPrint.GetFixedDocumentSequence();

                // Get a paginator for the document sequence
                DocumentPaginator paginator = fixedDocumentSequence.DocumentPaginator;

                // Print the paginator
                printDialog.PrintDocument(paginator, "Printing SPTC Report");
                xpsDocToPrint.Close();
            }
        }


    }
}
