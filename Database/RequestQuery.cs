using System;
using System.ComponentModel;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using MySql.Data.MySqlClient;
using SPTC_APP.Objects;

namespace SPTC_APP.Database
{
    public static class RequestQuery
    {

        public static string LOGIN_EMPLOYEE = $"SELECT * FROM {Table.EMPLOYEE} AS e LEFT JOIN {Table.POSITION} p ON p.{Field.ID} = e.{Field.POSITION_ID} WHERE p.{Field.TITLE} = ? AND e.{Field.PASSWORD} = ? AND e.{Where.ALL_NOTDELETED}";

        public static string GET_CURRENT_CHAIRMAN = $"SELECT * FROM {Table.EMPLOYEE} AS e LEFT JOIN {Table.POSITION} p ON p.{Field.ID} = e.{Field.POSITION_ID} WHERE p.{Field.TITLE} = \"Chairman\" AND e.{Field.ISDELETED} = 0";
        public static string GET_LIST_OF_POSITION = $"SELECT {Field.TITLE} FROM {Table.POSITION} WHERE {Where.ALL_NOTDELETED}";


        public static string CLEAN_NAME = $"DELETE n FROM {Table.NAME} AS n LEFT JOIN {Table.OPERATOR} AS o ON n.{Field.ID} = o.{Field.NAME_ID} LEFT JOIN {Table.DRIVER} AS d ON n.{Field.ID} = d.{Field.NAME_ID} LEFT JOIN {Table.EMPLOYEE} AS e ON n.{Field.ID} = e.{Field.NAME_ID} WHERE o.{Field.NAME_ID} IS NULL AND d.{Field.NAME_ID} IS NULL AND e.{Field.NAME_ID} IS NULL;";

        public static string CLEAN_ADDRESS = $"DELETE a FROM {Table.ADDRESS} AS a LEFT JOIN {Table.OPERATOR} AS o ON a.{Field.ID} = o.{Field.ADDRESS_ID} LEFT JOIN {Table.DRIVER} AS d ON a.{Field.ID} = d.{Field.ADDRESS_ID} LEFT JOIN {Table.EMPLOYEE} AS e ON a.{Field.ID} = e.{Field.ADDRESS_ID} WHERE o.{Field.ADDRESS_ID} IS NULL AND d.{Field.ADDRESS_ID} IS NULL AND e.{Field.ADDRESS_ID} IS NULL;";

        public static string CLEAN_IMAGE = $"DELETE i FROM {Table.IMAGE} AS i LEFT JOIN {Table.OPERATOR} AS o ON i.{Field.ID} = o.{Field.IMAGE_ID} OR i.{Field.ID} = o.{Field.SIGN_ID} LEFT JOIN {Table.DRIVER} AS d ON i.{Field.ID} = d.{Field.IMAGE_ID} OR i.{Field.ID} = d.{Field.SIGN_ID} LEFT JOIN {Table.EMPLOYEE} AS e ON i.{Field.ID} = e.{Field.IMAGE_ID} OR i.{Field.ID} = e.{Field.SIGN_ID} WHERE (o.{Field.IMAGE_ID} IS NULL AND o.{Field.SIGN_ID} IS NULL) AND (d.{Field.IMAGE_ID} IS NULL AND d.{Field.SIGN_ID} IS NULL) AND (e.{Field.IMAGE_ID} IS NULL AND e.{Field.SIGN_ID} IS NULL);";

        public static string GET_DRIVERS_WITHOUT_FRANCHISE = $"SELECT d.* FROM {Table.DRIVER} AS d LEFT JOIN {Table.FRANCHISE} AS f ON f.{Field.DRIVER_ID} = d.{Field.ID} WHERE f.{Field.DRIVER_ID} IS NULL AND d.{Where.ALL_NOTDELETED}";
        public static string GET_FRANCHISE_WITH_DRIVER = $"SELECT * FROM {Table.FRANCHISE} WHERE {Field.DRIVER_ID} <> -1 AND {Where.ALL}";

        public static string GET_TOTAL_SHARES = $"SELECT SUM({Field.DEPOSIT}) FROM {Table.PAYMENT_DETAILS} WHERE {Field.LEDGER_TYPE} = \"SHARECAPITAL\" AND {Where.ALL_NOTDELETED}";

        public static string GET_ALL_EMPLOYEES = $"SELECT * FROM {Table.EMPLOYEE} WHERE {Where.ALL_NOTDELETED}";

        public static string GET_TOTAL_EXPENSES(int month, int year) => $"SELECT SUM({Field.DEPOSIT}) FROM {Table.PAYMENT_DETAILS} WHERE YEAR({Field.DATE}) = {year} AND MONTH({Field.DATE}) = {month} AND {Field.DEPOSIT} < 0 AND {Where.ALL_NOTDELETED}";

        public static string SEARCH(string text) =>
            $"SELECT f.* FROM {Table.FRANCHISE} f LEFT JOIN {Table.OPERATOR} O ON f.{Field.OPERATOR_ID}=O.{Field.ID} LEFT JOIN {Table.DRIVER} D ON f.{Field.DRIVER_ID}=D.{Field.ID} " +
            $"LEFT JOIN {Table.NAME} OName ON O.{Field.NAME_ID}=OName.{Field.ID} LEFT JOIN {Table.NAME} DName ON D.{Field.NAME_ID}=DName.{Field.ID} " +
            $"WHERE f.{Field.BODY_NUMBER} LIKE \"%{text}%\" OR OName.{Field.LASTNAME} LIKE \"%{text}%\" OR DName.{Field.LASTNAME} LIKE \"%{text}%\" AND f.id = ( SELECT MAX(id) FROM tbl_franchise WHERE body_number = f.body_number ) AND f.{Where.ALL_NOTDELETED} LIMIT 0, 5";

        public static string GET_ALL_PAYMENT_IN_MONTH(int month, int year) =>
            $"SELECT SUM({Field.DEPOSIT}) FROM {Table.PAYMENT_DETAILS} WHERE YEAR({Field.DATE}) = {year} AND MONTH({Field.DATE}) = {month} AND {Field.LEDGER_ID} <> -1 AND {Field.DEPOSIT} > 0 AND {Where.ALL_NOTDELETED}";
        public static string GET_ALL_RECAP_IN_MONTH(int month, int year) =>
            $"SELECT {Field.ID}, {Field.RECAP_TEXT}, {Field.RECAP_VALUE}, {Field.DATE} FROM {Table.PAYMENT_DETAILS} WHERE YEAR({Field.DATE}) = {year} AND MONTH({Field.DATE}) = {month} AND {Field.LEDGER_ID} = -1 AND {Field.DEPOSIT} > 0 AND {Field.LEDGER_TYPE} = \"RECAP\" AND {Where.ALL_NOTDELETED}";
        public static string GET_ALL_PAYMENT_IN_MONTH(string table, int month, int year) =>
            $"SELECT SUM({Field.DEPOSIT}) FROM {Table.PAYMENT_DETAILS} WHERE YEAR({Field.DATE}) = {year} AND MONTH({Field.DATE}) = {month} AND {Field.LEDGER_TYPE} = \"{table}\" AND {Field.LEDGER_ID} <> -1 AND {Where.ALL_NOTDELETED}";
        public static string CHECK_IF_SUSPENDED(string entity, string field, int id) =>
            $"SELECT CASE WHEN EXISTS ( SELECT 1 FROM {Table.VIOLATION} AS v LEFT JOIN {Table.FRANCHISE} AS f ON f.{Field.ID} = v.{Field.FRANCHISE_ID} LEFT JOIN {Table.VIOLATION_TYPE} AS vt ON v.{Field.VIOLATION_TYPE_ID} = vt.{Field.ID} WHERE vt.{Field.ENTITY_TYPE} = \"{entity}\" AND f.{field} = {id} AND CURDATE() BETWEEN v.{Field.SUSPENSION_START} AND v.{Field.SUSPENSION_END} ) THEN TRUE ELSE FALSE END AS IsSuspended;";
        public static string GET_SHARE_LEDGER_LIST(int id) =>
            $"SELECT * FROM {Table.SHARE_CAPITAL} WHERE {Field.FRANCHISE_ID} = {id} AND {Where.ALL_NOTDELETED} ORDER BY {Field.DATE} DESC";
        public static string GET_LEDGER_LIST(string table, int id) =>
            $"SELECT * FROM {table} WHERE {Field.FRANCHISE_ID} = {id} AND {Field.IS_FULLY_PAID}=0 AND {Where.ALL_NOTDELETED} ORDER BY {Field.DATE} DESC";

        public static string GET_LEDGER_PAYMENT(string table, string type, int id) =>
            $"SELECT * FROM {Table.PAYMENT_DETAILS} AS pd LEFT JOIN {table} AS scl ON pd.{Field.LEDGER_ID} = scl.{Field.ID} AND pd.{Field.LEDGER_TYPE} = \"{type}\" WHERE scl.{Field.FRANCHISE_ID} = {id} AND pd.{Where.ALL_NOTDELETED} ORDER BY pd.{Field.DATE} DESC";
        public static string GET_FRANCHISE_OF(string table, string field, int id) => $"SELECT * FROM {Table.FRANCHISE} JOIN {table} AS o ON o.{Field.ID} = {field} WHERE o.{Field.ID} = {id}";
        public static string GET_VIOLATION_COUNT_OF(int id) => $"SELECT COUNT(*) FROM {Table.VIOLATION} WHERE {Field.NAME_ID} = {id} AND {Where.ALL_NOTDELETED}";
        public static string GET_VIOLATION_LIST_OF(int id, int name_id) => $"SELECT * FROM {Table.VIOLATION} WHERE {Field.FRANCHISE_ID} = {id} AND {Field.NAME_ID} = {name_id} AND {Field.ISDELETED} = 0";
        public static string GET_TOTAL(string table) => $"SELECT COUNT(*) FROM {table} WHERE {Where.ALL_NOTDELETED}";
        public static string GET_ALL_FRANCHISE_WITH_BODYNUM(string bodyNumber) => $"SELECT * FROM {Table.FRANCHISE} WHERE {Field.BODY_NUMBER} = {bodyNumber} AND {Where.ALL_NOTDELETED}";
        public static string GET_ACCESSES(int id) => $"SELECT * FROM {Table.POSITION} WHERE {Field.ID} = {id}";

        internal static string GET_ALL_PAYMENTS_ID(int id) => $"SELECT pd.* FROM tbl_payment_details pd LEFT JOIN tbl_share_capital_ledger sh ON sh.id = pd.ledger_id AND pd.ledger_type = \"{Ledger.SHARE_CAPITAL}\" LEFT JOIN tbl_loan_ledger l ON l.id = pd.ledger_id AND pd.ledger_type = \"{Ledger.LOAN}\" LEFT JOIN tbl_long_term_loan_ledger ltl ON ltl.id = pd.ledger_id AND pd.ledger_type = \"{Ledger.LT_LOAN}\" LEFT JOIN tbl_loan_ledger al ON al.id = pd.ledger_id AND pd.ledger_type = \"{Ledger.APPLY_LOAN}\" LEFT JOIN tbl_long_term_loan_ledger altl ON altl.id = pd.ledger_id AND pd.ledger_type = \"{Ledger.APPLY_LT_LOAN}\" WHERE sh.franchise_id = {id} OR l.franchise_id = {id} OR ltl.franchise_id = {id} OR al.franchise_id = {id} OR altl.franchise_id = {id} AND pd.{Where.ALL_NOTDELETED} ORDER BY pd.date DESC";
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
        public const string FRANCHISE = "tbl_franchise";
        public const string NAME = "tbl_name";
        public const string ADDRESS = "tbl_address";
        public const string IMAGE = "tbl_image";
        public const string EMPLOYEE = "tbl_employee";
        public const string DRIVER = "tbl_driver";
        public const string POSITION = "tbl_position";
        public const string OPERATOR = "tbl_operator";
        public const string PAYMENT_DETAILS = "tbl_payment_details";
        public const string LOAN = "tbl_loan_ledger";
        public const string SHARE_CAPITAL = "tbl_share_capital_ledger";
        public const string LONG_TERM_LOAN = "tbl_long_term_loan_ledger";
        public const string VIOLATION_TYPE = "tbl_violation_type";
        public const string VIOLATION = "tbl_violation";
        public const string IDHISTORY = "tbl_id_history";

        public const string FRANCHISE_S = "tbl_franchise f";

    }
    public static class Select
    {
        public const string ALL = "*";
        public const string F = "f.*";
        public const string BODY_NUMBER = "body_number";
    }
    public static class Where
    {
        public const string ALL = "1";
        public const string ALL_NOTDELETED = "isDeleted=0";
        public const string ALL_DELETED = "isDeleted=1";
        public const string ID_ = "id=?";
        public const string ID_NOTDELETED = "id=? AND isDeleted=0";
        public const string ID_DELETED = "id=? AND isDeleted=1";
        public const string LATEST_FRANCHISE = "f.id = ( SELECT MAX(id) FROM tbl_franchise WHERE body_number = f.body_number ) AND f.isDeleted=0";
        public const string HAS_OPERATOR = "operator_id <> -1 AND body_number = (SELECT MAX(body)) AND isDeleted=0";
        public static string DRIVER_AND_OPERATOR = $"({Field.DRIVER_ID} <> -1 OR {Field.OPERATOR_ID} <> -1) AND {Field.ISDELETED}=0";

    }
    public static class Field
    {
        // ALL
        public static string ISDELETED = "isDeleted";

        // SOME (Fields that are used in multiple classes)
        public const string REMARKS = "remarks";
        public const string DATE_OF_BIRTH = "date_of_birth";
        public const string CONTACT_NO = "contact_no";
        public const string DATE = "date";
        public const string DETAILS = "details";
        public const string START_DATE = "start_date";
        public const string END_DATE = "end_date";
        public const string TITLE = "title";
        public const string ENTITY_TYPE = "entity_type";
        public const string IS_FULLY_PAID = "isFullyPaid";

        // Foreign Keys
        public const string EMPLOYEE_ID = "user_id";
        public const string NAME_ID = "name_id";
        public const string ADDRESS_ID = "address_id";
        public const string IMAGE_ID = "image_id";
        public const string SIGN_ID = "sign_id";
        public const string POSITION_ID = "position_id";
        public const string OPERATOR_ID = "operator_id";
        public const string DRIVER_ID = "driver_id";
        public const string OWNER_ID = "owner_id";
        public const string LAST_FRANCHISE_ID = "last_franchise_id";
        public const string PAYMENT_ID = "payment_id";
        public const string FRANCHISE_ID = "franchise_id";
        public const string VIOLATION_TYPE_ID = "violation_type_id";
        public const string ID = "id";

        // Employee
        public const string PASSWORD = "password";

        // FRANCHISE
        public const string BODY_NUMBER = "body_number";
        public const string BUYING_DATE = "buying_date";
        public const string MTOP_NUMBER = "mtop_no";
        public const string LICENSE_NO = "license_no";
        public const string VOTERS_ID_NUMBER = "voters_id_number";
        public const string TIN_NUMBER = "tin_number";

        // Name
        public const string PREFIX = "sex";
        public const string FIRSTNAME = "first_name";
        public const string MIDDLENAME = "middle_name";
        public const string LASTNAME = "last_name";
        public const string SUFFIX = "suffix";

        // Address
        public const string HOUSENO = "house_no";
        public const string STREETNAME = "street_name";
        public const string BARANGAY = "barangay_subdivision";
        public const string CITY = "city_municipality";
        public const string ZIPCODE = "postal_code";
        public const string PROVINCE = "province";
        public const string COUNTRY = "country";
        public const string ADDRESSLINE1 = "address_line1";
        public const string ADDRESSLINE2 = "address_line2";

        // Image
        public const string IMAGE_SOURCE = "image_source_bin";
        public const string IMAGE_NAME = "image_name";

        // DRIVER
        public const string EM_CONTACT_PERSON = "emergency_person";
        public const string EM_CONTACT_NUMBER = "emergency_number";

        // POSITION
        public const string CAN_CREATE = "can_create";
        public const string CAN_EDIT = "can_edit";
        public const string CAN_DELETE = "can_delete";

        // PAYMENT DETAILS
        public const string LEDGER_ID = "ledger_id";
        public const string IS_DOWN_PAYMENT = "isDownPayment";
        public const string IS_DIV_PAT = "isDivPat";
        public const string LEDGER_TYPE = "ledger_type";
        public const string REFERENCE_NO = "reference_no";
        public const string DEPOSIT = "deposit";
        public const string PENALTIES = "penalties";
        public const string BALANCE = "balance";

        // LOAN
        public const string AMOUNT = "amount";
        public const string MONTHLY_INTEREST = "monthly_interest";
        public const string MONTHLY_PRINCIPAL = "monthly_principal";
        public const string PAYMENT_DUES = "payment_dues";

        // SHARECAPITAL 
        public const string BEGINNING_BALANCE = "beginning_balance";
        public const string LAST_BALANCE = "last_balance";

        // LONGTERMLOAN
        public const string TERMS_OF_PAYMENT_MONTH = "terms_of_payment_month";
        public const string AMOUNT_LOANED = "amount_loaned";
        public const string PROCESSING_FEE = "processing_fee";
        public const string CAPITAL_BUILDUP = "capital_buildup";

        //VIOLATION_TYPES
        public const string NUM_OF_DAYS = "num_of_days";

        //VIOLATION
        public const string VIOLATION_LEVEL_COUNT = "violation_level_count";
        public const string SUSPENSION_START = "suspension_start";
        public const string SUSPENSION_END = "suspension_end";


        //IDHISTORY
        public const string IS_PRINTED = "is_printed";

        public const string DATE_OF_MEM = "date_of_mem";

        //RECAP
        public const string RECAP_VALUE = "deposit";
        public const string RECAP_TEXT = "remarks";
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
