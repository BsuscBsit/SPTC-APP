using System;
using System.Collections.Generic;
using System.Linq;
using MySql.Data.MySqlClient;
using SPTC_APP.Objects;

namespace SPTC_APP.Database
{
    //USAGE: 
    /*
     new Create() for every new object with connection to database
    Insert(fieldname, value) for inserting new values, use the Field static variables
    Save() always to save the progress to database
     */
    public class Upsert
    {
        private readonly string tableName;
        public int id;
        private Dictionary<string, object> fieldValues;

        public Upsert(string tableName, int id)
        {
            this.tableName = tableName;
            this.id = id;

            if (id == -1)
            {
                fieldValues = new Dictionary<string, object>();
                //EventLogger.Post($"DTB :: Attempt to create new Information at ({tableName})");
            }
            else
            {
                fieldValues = LoadDataFromDatabase();
                //EventLogger.Post($"DTB :: Attempt to access ({tableName}) pk: {id}");
            }

        }

        public void Insert(string fieldName, object value)
        {
            fieldValues[fieldName] = value;
        }

        public object Access(string fieldName)
        {
            if (fieldValues.ContainsKey(fieldName))
            {
                return fieldValues[fieldName];
            }

            return null;
        }

        public void Save()
        {
            if (id == -1)
            {
                InsertDataToDatabase();
            }
            else
            {
                UpdateDataInDatabase();

            }
        }

        private Dictionary<string, object> LoadDataFromDatabase()
        {
            Dictionary<string, object> result = new Dictionary<string, object>();
            try
            {
                using (MySqlConnection connection = DatabaseConnection.GetConnection())
                {
                    connection.Open();
                    string query = $"SELECT * FROM {tableName} WHERE id = @id";
                    MySqlCommand command = new MySqlCommand(query, connection);
                    command.Parameters.AddWithValue("@id", id);

                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                string fieldName = reader.GetName(i);
                                object fieldValue = reader.GetValue(i);
                                result[fieldName] = fieldValue;
                            }
                        }
                    }
                }
            }
            catch (MySqlException ex)
            {
                EventLogger.Post($"DTB :: Exception : {ex.Message}");
            }

            return result;
        }

        private void InsertDataToDatabase()
        {
            try
            {
                using (MySqlConnection connection = DatabaseConnection.GetConnection())
                {
                    connection.Open();
                    string columns = string.Join(", ", fieldValues.Keys);
                    string values = string.Join(", ", fieldValues.Keys.Select(key => "@" + key));

                    string query = $"INSERT INTO {tableName} ({columns}) VALUES ({values})";
                    MySqlCommand command = new MySqlCommand(query, connection);

                    foreach (var entry in fieldValues)
                    {
                        command.Parameters.AddWithValue("@" + entry.Key, entry.Value);
                    }

                    try
                    {
                        command.ExecuteNonQuery();
                        id = (int)command.LastInsertedId;
                        EventLogger.Post($"DTB :: Insert into ({tableName}) pk: {id}");
                    }
                    catch (MySqlException ex)
                    {
                        if (ex.Number == 1062)
                        { 
                            EventLogger.Post($"DTB :: Insert Exception: Existing Duplicate: {ex.Message}");
                            if (tableName == Table.NAME)
                            {
                                id = GetExistingRecordId(fieldValues);
                            } 
                            else if (tableName == Table.IMAGE)
                            {
                                Dictionary<string, object> uniqueAttributes = new Dictionary<string, object>
                                {
                                    { Field.IMAGE_NAME, fieldValues.TryGetValue(Field.IMAGE_NAME, out var temp) ? temp : "" },
                                };

                                id = GetExistingRecordId(uniqueAttributes);
                            } 
                            else if (tableName == Table.ADDRESS)
                            {
                                Dictionary<string, object> uniqueAttributes = new Dictionary<string, object>
                                {
                                    { Field.ADDRESSLINE1, fieldValues.TryGetValue(Field.ADDRESSLINE1, out var temp) ? temp : "" },
                                    { Field.ADDRESSLINE2, fieldValues.TryGetValue(Field.ADDRESSLINE2, out temp) ? temp : "" }
                                };

                                id = GetExistingRecordId(uniqueAttributes);
                            }
                            else if (tableName == Table.FRANCHISE)
                            {
                                Dictionary<string, object> uniqueAttributes = new Dictionary<string, object>
                                {
                                    { Field.BODY_NUMBER, fieldValues.TryGetValue(Field.BODY_NUMBER, out var temp) ? temp : -1 },
                                    { Field.LAST_FRANCHISE_ID, fieldValues.TryGetValue(Field.LAST_FRANCHISE_ID, out temp) ? temp : -1 },
                                };

                                id = GetExistingRecordId(uniqueAttributes);
                            }
                            else if (tableName == Table.DRIVER || tableName == Table.OPERATOR)
                            {
                                Dictionary<string, object> uniqueAttributes = new Dictionary<string, object>
                                {
                                    { Field.NAME_ID, fieldValues.TryGetValue(Field.NAME_ID, out var temp) ? temp : -1 },
                                    { Field.ADDRESS_ID, fieldValues.TryGetValue(Field.ADDRESS_ID, out temp) ? temp : -1 },
                                };

                                id = GetExistingRecordId(uniqueAttributes);
                            }
                            else if(tableName == Table.IDHISTORY)
                            {
                                Dictionary<string, object> uniqueAttributes = new Dictionary<string, object>
                                {
                                    { Field.FRANCHISE_ID, fieldValues.TryGetValue(Field.FRANCHISE_ID, out var temp) ? temp : -1 },
                                    { Field.OWNER_ID, fieldValues.TryGetValue(Field.OWNER_ID, out temp) ? temp : -1 },
                                    { Field.ENTITY_TYPE, fieldValues.TryGetValue(Field.ENTITY_TYPE, out temp) ? temp : "" },
                                };
                                id = GetExistingRecordId(uniqueAttributes);
                            }
                            else if(tableName == Table.PAYMENT_DETAILS)
                            {
                                Dictionary<string, object> uniqueAttributes = new Dictionary<string, object>
                                {
                                    { Field.REFERENCE_NO, fieldValues.TryGetValue(Field.REFERENCE_NO, out var temp) ? temp : -1 },
                                };
                                id = GetExistingRecordId(uniqueAttributes);
                            }
                            else if (tableName == Table.VIOLATION_TYPE)
                            {
                                Dictionary<string, object> uniqueAttributes = new Dictionary<string, object>
                                {
                                    { Field.TITLE, fieldValues.TryGetValue(Field.TITLE, out var temp) ? temp : "" },
                                    { Field.NUM_OF_DAYS, fieldValues.TryGetValue(Field.NUM_OF_DAYS, out temp) ? temp : 0 },
                                    {Field.ENTITY_TYPE, fieldValues.TryGetValue(Field.ENTITY_TYPE, out temp)? temp: "DRIVER" }
                                };
                                id = GetExistingRecordId(uniqueAttributes);
                            }

                            else
                            {
                                id = GetExistingRecordId();
                            }
                            UpdateDataInDatabase();
                        }
                        else
                        {
                            EventLogger.Post($"DTB :: Insert Exception: {ex.Message}");
                        }
                    }


                }
            }
            catch (MySqlException ex)
            {
                EventLogger.Post($"DTB :: Insert Main Exception : {ex.Message}");
            }
        }

        private void UpdateDataInDatabase()
        {
            try
            {
                using (MySqlConnection connection = DatabaseConnection.GetConnection())
                {
                    connection.Open();

                    string setValues = string.Join(", ", fieldValues.Keys.Select(key => $"{key} = @{key}"));

                    string query = $"UPDATE {tableName} SET {setValues} WHERE id=@idkey";
                    MySqlCommand command = new MySqlCommand(query, connection);
                    command.Parameters.AddWithValue("@idkey", id);

                    foreach (var entry in fieldValues)
                    {
                        command.Parameters.AddWithValue("@" + entry.Key, entry.Value);
                    }

                    command.ExecuteNonQuery();
                    EventLogger.Post($"DTB :: Update ({tableName}) pk: {id}");
                }
            }
            catch (MySqlException ex)
            {
                EventLogger.Post($"DTB :: Update Main Exception : {ex.Message}");
            }
        }
        private int GetExistingRecordId(Dictionary<string, object> uniqueAttributes = null)
        {
            try
            {
                using (MySqlConnection connection = DatabaseConnection.GetConnection())
                {
                    connection.Open();
                    MySqlCommand command;
                    if (uniqueAttributes != null && uniqueAttributes.Count > 0)
                    {
                        // Handle tables with unique sets
                        string whereClause = string.Join(" AND ", uniqueAttributes.Keys.Select(key => $"{key} = @{key}"));
                        string query = $"SELECT id FROM {tableName} WHERE {whereClause}";
                        command = new MySqlCommand(query, connection);

                        // Add parameters for each field in the unique set
                        foreach (var kvp in uniqueAttributes)
                        {
                            command.Parameters.AddWithValue($"@{kvp.Key}", kvp.Value);
                        }
                    }
                    else
                    {
                        // Handle tables without unique sets
                        string uniqueAttribute = GetUniqueAttributeName();
                        string query = $"SELECT id FROM {tableName} WHERE {uniqueAttribute} = @{uniqueAttribute}";
                        command = new MySqlCommand(query, connection);
                        command.Parameters.AddWithValue($"@{uniqueAttribute}", fieldValues[uniqueAttribute]);
                    }

                    object result = command.ExecuteScalar();

                    return (result != null && result != DBNull.Value) ? Convert.ToInt32(result) : -1;
                }
            }
            catch (MySqlException ex)
            {
                EventLogger.Post($"DTB :: Existing Record Exception : {ex.Message}");
            }
            return -1;
        }

        private string GetUniqueAttributeName()
        {
            if (tableName == Table.FRANCHISE)
            {
                return Field.BODY_NUMBER;
            }

            return fieldValues.Keys.FirstOrDefault();
        }

    }
}
