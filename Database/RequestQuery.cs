using System;
using System.ComponentModel;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using MySql.Data.MySqlClient;

namespace SPTC_APP.Database
{
    public static class RequestQuery
    {

        public static string LOGIN_EMPLOYEE = $"SELECT * FROM {Table.EMPLOYEE} AS e LEFT JOIN {Table.POSITION} p ON p.{Field.ID} = e.{Field.POSITION_ID} WHERE p.{Field.TITLE} = ? AND e.{Field.PASSWORD} = ? AND e.{Field.ISDELETED} = 0";

        public static string GET_CURRENT_CHAIRMAN = $"SELECT * FROM {Table.EMPLOYEE} AS e LEFT JOIN {Table.POSITION} p ON p.{Field.ID} = e.{Field.POSITION_ID} WHERE p.{Field.TITLE} = \"Chairman\" AND e.{Field.ISDELETED} = 0";

        public static string CLEAN_NAME = $"DELETE n FROM {Table.NAME} AS n LEFT JOIN {Table.OPERATOR} AS o ON n.{Field.ID} = o.{Field.NAME_ID} LEFT JOIN {Table.DRIVER} AS d ON n.{Field.ID} = d.{Field.NAME_ID} LEFT JOIN {Table.EMPLOYEE} AS e ON n.{Field.ID} = e.{Field.NAME_ID} WHERE o.{Field.NAME_ID} IS NULL AND d.{Field.NAME_ID} IS NULL AND e.{Field.NAME_ID} IS NULL;";

        public static string CLEAN_ADDRESS = $"DELETE a FROM {Table.ADDRESS} AS a LEFT JOIN {Table.OPERATOR} AS o ON a.{Field.ID} = o.{Field.ADDRESS_ID} LEFT JOIN {Table.DRIVER} AS d ON a.{Field.ID} = d.{Field.ADDRESS_ID} LEFT JOIN {Table.EMPLOYEE} AS e ON a.{Field.ID} = e.{Field.ADDRESS_ID} WHERE o.{Field.ADDRESS_ID} IS NULL AND d.{Field.ADDRESS_ID} IS NULL AND e.{Field.ADDRESS_ID} IS NULL;";

        public static string CLEAN_IMAGE = $"DELETE i FROM {Table.IMAGE} AS i LEFT JOIN {Table.OPERATOR} AS o ON i.{Field.ID} = o.{Field.IMAGE_ID} OR i.{Field.ID} = o.{Field.SIGN_ID} LEFT JOIN {Table.DRIVER} AS d ON i.{Field.ID} = d.{Field.IMAGE_ID} OR i.{Field.ID} = d.{Field.SIGN_ID} LEFT JOIN {Table.EMPLOYEE} AS e ON i.{Field.ID} = e.{Field.IMAGE_ID} OR i.{Field.ID} = e.{Field.SIGN_ID} WHERE (o.{Field.IMAGE_ID} IS NULL AND o.{Field.SIGN_ID} IS NULL) AND (d.{Field.IMAGE_ID} IS NULL AND d.{Field.SIGN_ID} IS NULL) AND (e.{Field.IMAGE_ID} IS NULL AND e.{Field.SIGN_ID} IS NULL);";

        public static string SEARCH(string text) =>
            $"SELECT * FROM {Table.FRANCHISE} f LEFT JOIN {Table.OPERATOR} O ON f.{Field.OPERATOR_ID}=O.{Field.ID} LEFT JOIN {Table.DRIVER} D ON f.{Field.DRIVER_ID}=D.{Field.ID} " +
            $"LEFT JOIN {Table.NAME} OName ON O.{Field.NAME_ID}=OName.{Field.ID} LEFT JOIN {Table.NAME} DName ON D.{Field.NAME_ID}=DName.{Field.ID} " +
            $"WHERE f.{Field.BODY_NUMBER} LIKE \"%{text}%\" OR OName.{Field.LASTNAME} LIKE \"%{text}%\" OR DName.{Field.LASTNAME} LIKE \"%{text}%\" AND f.{Field.ISDELETED} = 0 LIMIT 0, 10";

        public static string GET_ALL_PAYMENT_IN_MONTH(int month, int year) =>
            $"SELECT SUM({Field.DEPOSIT}) FROM {Table.PAYMENT_DETAILS} WHERE YEAR({Field.DATE}) = {year} AND MONTH({Field.DATE}) = {month} AND {Field.LEDGER_ID} <> -1 AND {Field.ISDELETED} = 0";
        

        public static string GetEnumDescription(CRUDControl value)
        {
            FieldInfo fieldInfo = value.GetType().GetField(value.ToString());

            DescriptionAttribute[] attributes = (DescriptionAttribute[])fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);

            return attributes.Length > 0 ? attributes[0].Description : value.ToString();
        }
        public static string Protect(string input)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                StringBuilder stringBuilder = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    stringBuilder.Append(hashBytes[i].ToString("x2"));
                }

                return stringBuilder.ToString();
            }
        }

    }

    public static class Table
    {
        public static string FRANCHISE = "tbl_franchise";
        public static string NAME = "tbl_name";
        public static string ADDRESS = "tbl_address";
        public static string IMAGE = "tbl_image";
        public static string EMPLOYEE = "tbl_employee";
        public static string DRIVER = "tbl_driver";
        public static string POSITION = "tbl_position";
        public static string OPERATOR = "tbl_operator";
        public static string PAYMENT_DETAILS = "tbl_payment_details";
        public static string LOAN = "tbl_loan_ledger";
        public static string SHARE_CAPITAL = "tbl_share_capital_ledger";
        public static string LONG_TERM_LOAN = "tbl_long_term_loan_ledger";
        public static string VIOLATION_TYPE = "tbl_violation_type";
        public static string VIOLATION = "tbl_violation";
        public static string IDHISTORY = "tbl_id_history";

    }
    public static class Select
    {
        public static string ALL = "*";

        public static string BODY_NUMBER = "body_number";
    }
    public static class Where
    {
        public static string ALL = "1";
        public static string ALL_NOTDELETED = "isDeleted=0";
        public static string ALL_DELETED = "isDeleted=1";
        public static string ID_ = "id=?";
        public static string ID_NOTDELETED = "id=? AND isDeleted=0";
        public static string ID_DELETED = "id=? AND isDeleted=1";

        public static string THEREIS_OPERATOR = "operator_id != -1 AND isDeleted = 0";
        public static string THEREIS_DRIVER = "driver_id != -1 AND isDeleted = 0";


        
    }
    public static class Field
    {
        // ALL
        public static string ISDELETED = "isDeleted";

        // SOME (Fields that are used in multiple classes)
        public static string REMARKS = "remarks";
        public static string DATE_OF_BIRTH = "date_of_birth";
        public static string CONTACT_NO = "contact_no";
        public static string DATE = "date";
        public static string DETAILS = "details";
        public static string START_DATE = "start_date";
        public static string END_DATE = "end_date";
        public static string TITLE = "title";
        public static string ENTITY_TYPE = "entity_type";

        // Foreign Keys
        public static string EMPLOYEE_ID = "user_id";
        public static string NAME_ID = "name_id";
        public static string ADDRESS_ID = "address_id";
        public static string IMAGE_ID = "image_id";
        public static string SIGN_ID = "sign_id";
        public static string POSITION_ID = "position_id";
        public static string OPERATOR_ID = "operator_id";
        public static string DRIVER_ID = "driver_id";
        public static string OWNER_ID = "owner_id";
        public static string LAST_FRANCHISE_ID = "last_franchise_id";
        public static string PAYMENT_ID = "payment_id";
        public static string FRANCHISE_ID = "franchise_id";
        public static string VIOLATION_TYPE_ID = "violation_type_id";
        public static string ID = "id";

        // Employee
        public static string PASSWORD = "password";

        // FRANCHISE
        public static string BODY_NUMBER = "body_number";
        public static string BUYING_DATE = "buying_date";
        public static string MTOP_NUMBER = "mtop_no";
        public static string LICENSE_NO = "license_no";
        public static string VOTERS_ID_NUMBER = "voters_id_number";
        public static string TIN_NUMBER = "tin_number";

        // Name
        public static string PREFIX = "sex";
        public static string FIRSTNAME = "first_name";
        public static string MIDDLENAME = "middle_name";
        public static string LASTNAME = "last_name";
        public static string SUFFIX = "suffix";

        // Address
        public static string HOUSENO = "house_no";
        public static string STREETNAME = "street_name";
        public static string BARANGAY = "barangay_subdivision";
        public static string CITY = "city_municipality";
        public static string ZIPCODE = "postal_code";
        public static string PROVINCE = "province";
        public static string COUNTRY = "country";
        public static string ADDRESSLINE1 = "address_line1";
        public static string ADDRESSLINE2 = "address_line2";

        // Image
        public static string IMAGE_SOURCE = "image_source_bin";
        public static string IMAGE_NAME = "image_name";

        // DRIVER
        public static string EM_CONTACT_PERSON = "emergency_person";
        public static string EM_CONTACT_NUMBER = "emergency_number";
        public static string ISDAYSHIFT = "isDayShift";

        // POSITION
        public static string CAN_CREATE = "can_create";
        public static string CAN_EDIT = "can_edit";
        public static string CAN_DELETE = "can_delete";

        // PAYMENT DETAILS
        public static string LEDGER_ID = "ledger_id";
        public static string IS_DOWN_PAYMENT = "is_down_payment";
        public static string IS_DIV_PAT = "is_div_pat";
        public static string LEDGER_TYPE = "ledger_type";
        public static string REFERENCE_NO = "reference_no";
        public static string DEPOSIT = "deposit";
        public static string PENALTIES = "penalties";

        // LOAN
        public static string AMOUNT = "amount";
        public static string MONTHLY_INTEREST = "monthly_interest";
        public static string MONTHLY_PRINCIPAL = "monthly_principal";
        public static string PAYMENT_DUES = "payment_dues";

        // SHARECAPITAL 
        public static string BEGINNING_BALANCE = "beginning_balance";
        public static string LAST_BALANCE = "last_balance";

        // LONGTERMLOAN
        public static string TERMS_OF_PAYMENT_MONTH = "terms_of_payment_month";
        public static string AMOUNT_LOANED = "amount_loaned";
        public static string PROCESSING_FEE = "processing_fee";
        public static string CAPITAL_BUILDUP = "capital_buildup";

        //VIOLATION_TYPES
        public static string NUM_OF_DAYS = "num_of_days";

        //VIOLATION
        public static string VIOLATION_LEVEL_COUNT = "violation_level_count";
        public static string SUSPENSION_START = "suspension_start";
        public static string SUSPENSION_END = "suspension_end";


        //IDHISTORY
    }

    public enum CRUDControl
    {

        [Description("LOGIN FAILED")]
        LOGIN_FAILED,

        [Description("WRONG PASSWORD")]
        WRONG_PASSWORD,

        [Description("TRY AGAIN")]
        TRY_AGAIN,

    }

    public class Clean
    {
        private string CLEANER;
        /// <summary>
        /// CLean only if IS_ADMIN
        /// </summary>
        /// <param name="table"></param>
        /// <param name="cleaner"></param>
        public Clean(string cleaner)
        {
            CLEANER = cleaner;
        }

        public bool Start()
        {
            if (AppState.IS_ADMIN)
            {
                using (MySqlConnection connection = DatabaseConnection.GetConnection())
                {
                    connection.Open();
                    MySqlCommand command = new MySqlCommand(CLEANER, connection);
                    int affectedRows = command.ExecuteNonQuery();
                    return affectedRows > 0;
                }
            }
            return false;
        }

    }
}
