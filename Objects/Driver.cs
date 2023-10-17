using System;
using System.Linq;
using MySql.Data.MySqlClient;
using SPTC_APP.Database;

namespace SPTC_APP.Objects
{
    public class Driver
    {

        public int id { get; private set; }
        public Name name { get; set; }
        public Address address { get; set; }
        public Image image { get; set; }
        public Image signature { get; set; }
        public string remarks { get; set; }
        public DateTime birthday { get; set; }
        public string emergencyPerson { get; set; }
        public string emergencyContact { get; set; }

        public bool isSuspended {
            get
            {
                EventLogger.Post($"OUT :: {id}");
                return Retrieve.GetDataUsingQuery<bool>(RequestQuery.CHECK_IF_SUSPSENDED(AppState.GetEnumDescription(General.DRIVER), Field.DRIVER_ID, id)).FirstOrDefault();
            } 
            private set{ } 
        }
        private Upsert mDriver;

        public Driver()
        {
            mDriver = new Upsert(Table.DRIVER, -1);
            name = null;
            address = null;
            image = null;
            signature = null;
        }

        public Driver(MySqlDataReader reader)
        {
            name = null;
            address = null;
            image = null;
            signature = null;
            mDriver = null;
            this.id = Retrieve.GetValueOrDefault<int>(reader, Field.ID);
            this.remarks = Retrieve.GetValueOrDefault<string>(reader, Field.REMARKS);
            this.birthday = Retrieve.GetValueOrDefault<DateTime>(reader, Field.DATE_OF_BIRTH);
            this.emergencyPerson = Retrieve.GetValueOrDefault<string>(reader, Field.EM_CONTACT_PERSON);
            this.emergencyContact = Retrieve.GetValueOrDefault<string>(reader, Field.EM_CONTACT_NUMBER);

            Populate(Retrieve.GetValueOrDefault<int>(reader, Field.NAME_ID), Retrieve.GetValueOrDefault<int>(reader, Field.ADDRESS_ID), Retrieve.GetValueOrDefault<int>(reader, Field.IMAGE_ID), Retrieve.GetValueOrDefault<int>(reader, Field.SIGN_ID));
        }

        public bool WriteInto(Name name, Address address, Image image, Image sign, string remarks, DateTime bday, string emergencyPerson, string emergencyContact)
        {
            this.name = name;
            this.address = address;
            this.image = image;
            this.signature = sign;
            this.remarks = remarks;
            this.birthday = bday;
            this.emergencyPerson = emergencyPerson;
            this.emergencyContact = emergencyContact;
            return true;
        }
        private void Populate(int lname, int laddress, int limage, int lsignature)
        {
            if (lname >= 0)
                this.name = (Retrieve.GetData<Name>(Table.NAME, Select.ALL, Where.ID_, new MySqlParameter("id", lname))).FirstOrDefault();
            if (laddress >= 0)
                this.address = (Retrieve.GetData<Address>(Table.ADDRESS, Select.ALL, Where.ID_, new MySqlParameter("id", laddress))).FirstOrDefault();
            if (limage >= 0)
                this.image = (Retrieve.GetData<Image>(Table.IMAGE, Select.ALL, Where.ID_, new MySqlParameter("id", limage))).FirstOrDefault();
            if (lsignature >= 0)
                this.signature = (Retrieve.GetData<Image>(Table.IMAGE, Select.ALL, Where.ID_, new MySqlParameter("id", lsignature))).FirstOrDefault();
        }
        public int Save()
        {
            if (mDriver == null)
            {
                mDriver = new Upsert(Table.DRIVER, id);
            }
            mDriver.Insert(Field.REMARKS, remarks);
            mDriver.Insert(Field.DATE_OF_BIRTH, birthday);
            mDriver.Insert(Field.EM_CONTACT_PERSON, this.emergencyPerson);
            mDriver.Insert(Field.EM_CONTACT_NUMBER, this.emergencyContact);
            if (this.name != null)
            {
                mDriver.Insert(Field.NAME_ID, this.name.Save());
            }
            if (this.address != null)
            {
                mDriver.Insert(Field.ADDRESS_ID, this.address.Save());
            }
            if (this.image != null)
            {
                mDriver.Insert(Field.IMAGE_ID, this.image.Save());
            }
            if (this.signature != null)
            {
                mDriver.Insert(Field.SIGN_ID, this.signature.Save());
            }
            mDriver.Save();
            id = mDriver.id;

            return id;
        }

        public override string ToString()
        {
            if (name != null)
            {
                return name.ToString();
            }
            return "";
        }
        public bool delete()
        {
            if (mDriver == null)
            {
                mDriver = new Upsert(Table.DRIVER, id);
            }
            mDriver.Insert(Field.ISDELETED, true);
            mDriver.Save();
            return true;
        }
    }
}
