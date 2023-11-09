using System;
using System.Linq;
using MySql.Data.MySqlClient;
using SPTC_APP.Database;
using SPTC_APP.View;

namespace SPTC_APP.Objects
{
    public class Employee
    {
        public int id;
        public Name name { get; set; }
        public Address address { get; set; }
        public Image image { get; set; }
        public Image sign { get; set; }
        public Position position { get; set; }
        public DateTime startDate { get; set; }
        public DateTime endDate { get; set; }
        public DateTime birthday { get; set; } = DateTime.Now;
        public string contactNo { get; set; }

        private Upsert employee;

        public Employee()
        {
            employee = new Upsert(Table.EMPLOYEE, -1);
            //employee.Insert(Field.PASSWORD, RequestQuery.Protect(AppState.DEFAULT_PASSWORD));
            name = null;
            address = null;
            image = null;
            sign = null;
            position = null;
        }
        public Employee(MySqlDataReader reader)
        {
            name = null;
            address = null;
            image = null;
            sign = null;
            position = null;
            employee = null;
            this.id = Retrieve.GetValueOrDefault<int>(reader, Field.ID);
            this.startDate = Retrieve.GetValueOrDefault<DateTime>(reader, Field.START_DATE);
            this.endDate = Retrieve.GetValueOrDefault<DateTime>(reader, Field.END_DATE);
            this.birthday = Retrieve.GetValueOrDefault<DateTime>(reader, Field.DATE_OF_BIRTH);
            this.contactNo = Retrieve.GetValueOrDefault<string>(reader, Field.CONTACT_NO);

            Populate(Retrieve.GetValueOrDefault<int>(reader, Field.NAME_ID), Retrieve.GetValueOrDefault<int>(reader, Field.ADDRESS_ID), Retrieve.GetValueOrDefault<int>(reader, Field.IMAGE_ID), Retrieve.GetValueOrDefault<int>(reader, Field.SIGN_ID), Retrieve.GetValueOrDefault<int>(reader, Field.POSITION_ID));
        }
        private void Populate(int name, int address, int image, int sign, int position)
        {
            this.name = (Retrieve.GetData<Name>(Table.NAME, Select.ALL, Where.ID_, new MySqlParameter(Field.ID, name))).FirstOrDefault();
            this.address = (Retrieve.GetData<Address>(Table.ADDRESS, Select.ALL, Where.ID_, new MySqlParameter(Field.ID, address))).FirstOrDefault();
            this.image = (Retrieve.GetData<Image>(Table.IMAGE, Select.ALL, Where.ID_, new MySqlParameter(Field.ID, image))).FirstOrDefault();
            this.sign = (Retrieve.GetData<Image>(Table.IMAGE, Select.ALL, Where.ID_, new MySqlParameter(Field.ID, sign))).FirstOrDefault();
            this.position = (Retrieve.GetData<Position>(Table.POSITION, Select.ALL, Where.ID_, new MySqlParameter(Field.ID, position))).FirstOrDefault();
        }
        public int Save()
        {
            if (employee == null)
            {
                employee = new Upsert(Table.EMPLOYEE, id);
            }
            employee.Insert(Field.START_DATE, startDate);
            employee.Insert(Field.END_DATE, endDate);
            employee.Insert(Field.DATE_OF_BIRTH, birthday);
            employee.Insert(Field.CONTACT_NO, contactNo);
            if (this.name != null)
            {
                employee.Insert(Field.NAME_ID, this.name.Save());
            }
            if (this.address != null)
            {
                employee.Insert(Field.ADDRESS_ID, this.address.Save());
            }
            if (this.image != null)
            {
                employee.Insert(Field.IMAGE_ID, this.image.Save());
            }
            if (this.sign != null)
            {
                employee.Insert(Field.SIGN_ID, this.sign.Save());
            }
            if (this.position != null)
            {
                employee.Insert(Field.POSITION_ID, this.position.Save());
            }
            employee.Save();
            id = employee.id;

            return id;
        }

        public bool delete()
        {
            if (employee == null)
            {
                employee = new Upsert(Table.EMPLOYEE, id);
            }
            employee.Insert(Field.ISDELETED, true);
            employee.Save();
            return true;
        }

        public void updatePass(string password, string newpassword)
        {
            if (employee != null)
            {
                if (Retrieve.GetDataUsingQuery<string>(RequestQuery.GET_PASS(id))?.FirstOrDefault()?.Equals(RequestQuery.Protect(password)) ?? false){
                    employee.Insert(Field.PASSWORD, RequestQuery.Protect(newpassword));
                    employee.Save();
                } else
                {
                    ControlWindow.ShowStatic("Password Incorrect", "Password doesnt match", Icons.ERROR);
                }
            }
        }
    }
}
