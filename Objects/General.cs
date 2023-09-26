using System;
using System.Drawing.Drawing2D;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using MySql.Data.MySqlClient;
using SPTC_APP.Database;

namespace SPTC_APP.Objects
{
    public enum General
    {
        FRANCHISE,
        OPERATOR,
        DRIVER,


        NEW_DRIVER,
        NEW_OPERATOR,
        NEW_FRANCHISE,

        LOGIN_EMPLOYEE,

        FETCH_DRIVER_USING_ID,
        FETCH_DRIVER_USING_BODYNUMBER,

        FETCH_OPERATOR_USING_ID,
        FETCH_OPERATOR_USING_BODYNUMBER,

        FETCH_FRANCHISE_USING_ID,
        FETCH_FRANCHISE_USING_BODYNUMBER,
        FETCH_PAYMENT_DETAILS_USING_ID,
        NEW_PAYMENT_DETAILS,
    }


    public class Name
    {
        public int id { get; private set; }
        public bool sex;
        public string firstname;
        public string middlename;
        public string lastname;
        public string suffix;
        public string wholename
        {
            get
            {
                if (!string.IsNullOrEmpty(middlename))
                {
                    string middleInitials = string.Join("", middlename.Split(' ').Select(part => part[0]));
                    return $"{lastname}, {firstname} {middleInitials}. {suffix}".Trim();
                }

                return $"{lastname}, {firstname} {suffix}".Trim();
            }
            private set { }
        }


        private Upsert name;

        public Name()
        {
            name = new Upsert(Table.NAME, -1);
        }

        public Name(bool prefix, string firstname, string middlename, string lastname, string suffix)
        {
            this.sex = prefix;
            this.firstname = firstname;
            this.middlename = middlename;
            this.lastname = lastname;
            this.suffix = suffix;
            name = new Upsert("tbl_name", -1);
        }

        public Name(MySqlDataReader reader)
        {
            name = null;
            this.id = Retrieve.GetValueOrDefault<int>(reader, Field.ID);
            this.sex = Retrieve.GetValueOrDefault<bool>(reader, Field.PREFIX);
            this.firstname = Retrieve.GetValueOrDefault<string>(reader, Field.FIRSTNAME);
            this.middlename = Retrieve.GetValueOrDefault<string>(reader, Field.MIDDLENAME);
            this.lastname = Retrieve.GetValueOrDefault<string>(reader, Field.LASTNAME);
            this.suffix = Retrieve.GetValueOrDefault<string>(reader, Field.SUFFIX);
        }

        public int Save()
        {
            if (name == null)
            {
                name = new Upsert(Table.NAME, id);
            }
            name.Insert("sex", sex);
            name.Insert("first_name", firstname);
            name.Insert("middle_name", middlename);
            name.Insert("last_name", lastname);
            name.Insert("suffix", suffix);
            name.Save();
            id = name.id;

            return id;
        }

        public override string ToString()
        {
            return wholename ?? string.Empty;
        }

        public bool Delete()
        {
            if (name == null)
            {
                name = new Upsert(Table.NAME, id);
                name.Insert(Field.ISDELETED, true);
                name.Save();
                return true;
            }
            return false;
        }
    }

    public class Address
    {
        public int id { get; private set; }
        public string houseNo;
        public string streetname;
        public string barangay;

        public string city;
        public string province;
        public string zipcode;
        public string country;

        public string addressline1;
        public string addressline2;

        private Upsert address;

        public Address()
        {
            address = new Upsert(Table.ADDRESS, -1);
        }
        public Address(string houseNo, string streetName, string barangay, string city, string zipcode, string province, string country)
        {
            this.houseNo = houseNo;
            this.streetname = streetName;
            this.barangay = barangay;
            this.city = city;
            this.zipcode = zipcode;
            this.province = province;
            this.country = country;
            address = new Upsert(Table.ADDRESS, -1);
        }

        public Address(string addressline1, string addressline2)
        {
            this.addressline1 = addressline1;
            this.addressline2 = addressline2;
            address = new Upsert(Table.ADDRESS, -1);
        }
        public Address(MySqlDataReader reader)
        {
            address = null;
            this.id = Retrieve.GetValueOrDefault<int>(reader, Field.ID);
            this.houseNo = Retrieve.GetValueOrDefault<string>(reader, Field.HOUSENO);
            this.streetname = Retrieve.GetValueOrDefault<string>(reader, Field.STREETNAME);
            this.barangay = Retrieve.GetValueOrDefault<string>(reader, Field.BARANGAY);
            this.city = Retrieve.GetValueOrDefault<string>(reader, Field.CITY);
            this.zipcode = Retrieve.GetValueOrDefault<string>(reader, Field.ZIPCODE);
            this.province = Retrieve.GetValueOrDefault<string>(reader, Field.PROVINCE);
            this.country = Retrieve.GetValueOrDefault<string>(reader, Field.COUNTRY);
            this.addressline1 = Retrieve.GetValueOrDefault<string>(reader, Field.ADDRESSLINE1);
            this.addressline2 = Retrieve.GetValueOrDefault<string>(reader, Field.ADDRESSLINE2);

        }



        public int Save()
        {
            if (address == null)
            {
                address = new Upsert(Table.ADDRESS, id);
            }

            address.Insert(Field.ADDRESSLINE1, this.addressline1);
            address.Insert(Field.ADDRESSLINE2, this.addressline2);
            address.Insert(Field.HOUSENO, houseNo);
            address.Insert(Field.STREETNAME, streetname);
            address.Insert(Field.BARANGAY, barangay);
            address.Insert(Field.CITY, city);
            address.Insert(Field.ZIPCODE, zipcode);
            address.Insert(Field.PROVINCE, province);
            address.Insert(Field.COUNTRY, country);
            address.Save();
            id = address.id;

            return id;
        }
        public override string ToString()
        {
            if (!string.IsNullOrEmpty(addressline1) && !string.IsNullOrEmpty(addressline2))
            {
                return $"{addressline1} {addressline2}";
            }
            else if (!string.IsNullOrEmpty(houseNo) || !string.IsNullOrEmpty(streetname) || !string.IsNullOrEmpty(barangay) || !string.IsNullOrEmpty(city) || !string.IsNullOrEmpty(province) || !string.IsNullOrEmpty(zipcode) || !string.IsNullOrEmpty(country))
            {
                return $"{houseNo} {streetname}, {barangay}, {city}, {province}, {country}";
            }
            else
            {
                return string.Empty;
            }
        }

        public bool Delete()
        {
            if (address == null)
            {
                address = new Upsert(Table.ADDRESS, id);
                address.Insert(Field.ISDELETED, true);
                address.Save();
                return true;
            }
            return false;
        }

    }

    public class Image
    {
        public int id { get; private set; }
        public byte[] picture;
        public string name;

        private Upsert image;

        public Image()
        {
            image = new Upsert(Table.IMAGE, -1);
        }
        public Image(byte[] imagebitmap, string name)
        {
            this.name = name;
            this.picture = imagebitmap;
            image = new Upsert(Table.IMAGE, -1);
        }

        public Image(ImageSource source, string name)
        {
            this.name = name;
            this.picture = ImageSourceToByte(source);
            image = new Upsert(Table.IMAGE, -1);
        }

        public Image(MySqlDataReader reader)
        {
            image = null;
            this.id = Retrieve.GetValueOrDefault<int>(reader, Field.ID);

            int imageOrdinal = reader.GetOrdinal(Field.IMAGE_SOURCE);
            if (!reader.IsDBNull(imageOrdinal))
            {
                long byteLength = reader.GetBytes(imageOrdinal, 0, null, 0, 0);
                byte[] buffer = new byte[byteLength];
                reader.GetBytes(imageOrdinal, 0, buffer, 0, buffer.Length);
                this.picture = buffer;
            }
            else
            {
                this.picture = null;
            }
            this.name = Retrieve.GetValueOrDefault<string>(reader, Field.IMAGE_NAME);
        }

        public ImageSource GetSource()
        {
            if (picture == null || picture.Length == 0)
                return null;

            BitmapImage image = new BitmapImage();
            using (MemoryStream memoryStream = new MemoryStream(picture))
            {
                memoryStream.Position = 0;

                image.BeginInit();
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.StreamSource = memoryStream;
                image.EndInit();
            }

            return image;
        }

        private byte[] ImageSourceToByte(ImageSource imageSource)
        {
            if (imageSource is BitmapSource bitmapSource)
            {
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    BitmapEncoder encoder = new JpegBitmapEncoder(); // Use JPEG compression
                    encoder.Frames.Add(BitmapFrame.Create(bitmapSource));

                    // Define a compression level (adjust quality as needed)
                    ((JpegBitmapEncoder)encoder).QualityLevel = 80; // Adjust quality from 1 to 100

                    encoder.Save(memoryStream);

                    // Check if the resulting byte array is larger than the maximum allowed size for MEDIUMBLOB
                    if (memoryStream.Length <= 16777215) // 16MB in bytes
                    {
                        return memoryStream.ToArray();
                    }
                    else
                    {
                        // Compress the image to fit within the 16MB limit
                        memoryStream.Position = 0;
                        using (MemoryStream compressedStream = new MemoryStream())
                        {
                            var buffer = new byte[4096];
                            int bytesRead;
                            while ((bytesRead = memoryStream.Read(buffer, 0, buffer.Length)) > 0)
                            {
                                compressedStream.Write(buffer, 0, bytesRead);
                                if (compressedStream.Length > 16777215) // Check size limit
                                {
                                    EventLogger.Post($"ERR :: Image size still exceeds the maximum allowed for MEDIUMBLOB after compression. {compressedStream.Length}");
                                }
                            }
                            return compressedStream.ToArray();
                        }
                    }
                }
            }
            else if (imageSource is BitmapImage bitmapImage)
            {
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    BitmapEncoder encoder = new PngBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create(bitmapImage));
                    // Define a compression level (adjust quality as needed)
                    ((JpegBitmapEncoder)encoder).QualityLevel = 80; // Adjust quality from 1 to 100

                    encoder.Save(memoryStream);

                    // Check if the resulting byte array is larger than the maximum allowed size for MEDIUMBLOB
                    if (memoryStream.Length <= 16777215) // 16MB in bytes
                    {
                        return memoryStream.ToArray();
                    }
                    else
                    {
                        // Compress the image to fit within the 16MB limit
                        memoryStream.Position = 0;
                        using (MemoryStream compressedStream = new MemoryStream())
                        {
                            var buffer = new byte[4096];
                            int bytesRead;
                            while ((bytesRead = memoryStream.Read(buffer, 0, buffer.Length)) > 0)
                            {
                                compressedStream.Write(buffer, 0, bytesRead);
                                if (compressedStream.Length > 16777215) // Check size limit
                                {
                                    EventLogger.Post($"ERR :: Image size still exceeds the maximum allowed for MEDIUMBLOB after compression. {compressedStream.Length}");
                                }
                            }
                            return compressedStream.ToArray();
                        }
                    }
                }
            }

            return null;
        }


        public int Save()
        {
            if (image == null)
            {
                image = new Upsert(Table.IMAGE, id);
            }

            image.Insert(Field.IMAGE_NAME, name);
            image.Insert(Field.IMAGE_SOURCE, picture);
            image.Save();
            id = image.id;

            return id;
        }
        public override string ToString()
        {
            return name ?? string.Empty;
        }
        public bool Delete()
        {
            if (image == null)
            {
                image = new Upsert(Table.IMAGE, id);
                image.Insert(Field.ISDELETED, true);
                image.Save();
                return true;
            }
            return false;
        }

    }

    public class Position
    {
        public int id { get; private set; }
        public string title;
        public bool canCreate;
        public bool canEdit;
        public bool canDelete;
        private Upsert position;
        public Position()
        {
            position = new Upsert(Table.POSITION, -1);
        }

        public Position(string title, bool canCreate, bool canEdit, bool canDelete)
        {
            this.title = title;
            this.canCreate = canCreate;
            this.canEdit = canEdit;
            this.canDelete = canDelete;
            this.position = new Upsert(Table.POSITION, -1);
        }

        public Position(MySqlDataReader reader)
        {
            this.id = Retrieve.GetValueOrDefault<int>(reader, Field.ID);
            this.title = Retrieve.GetValueOrDefault<string>(reader, Field.TITLE);
            this.canCreate = Retrieve.GetValueOrDefault<bool>(reader, Field.CAN_CREATE);
            this.canEdit = Retrieve.GetValueOrDefault<bool>(reader, Field.CAN_EDIT);
            this.canDelete = Retrieve.GetValueOrDefault<bool>(reader, Field.CAN_DELETE);
        }

        public int Save()
        {
            if (position == null)
            {
                position = new Upsert(Table.POSITION, id);
            }

            position.Insert(Field.TITLE, title);
            position.Insert(Field.CAN_CREATE, canCreate);
            position.Insert(Field.CAN_EDIT, canEdit);
            position.Insert(Field.CAN_DELETE, canDelete);
            position.Save();
            id = position.id;

            return id;
        }

        public override string ToString()
        {
            return title ?? string.Empty;
        }

        public bool Delete()
        {
            if (position == null)
            {
                position = new Upsert(Table.POSITION, id);
                position.Insert(Field.ISDELETED, true);
                position.Save();
                return true;
            }
            return false;
        }
    }

    public class ViolationType
    {
        public int id { get; private set; }
        public string title;
        public string details;
        public int numOfDays;
        public General entityType;

        private Upsert violationType;
        public ViolationType()
        {
            violationType = new Upsert(Table.VIOLATION_TYPE, -1);
        }
        public void WriteInto(string title, string details, int numOfDays, General entity)
        {
            this.title = title;
            this.details = details;
            this.numOfDays = numOfDays;
            this.entityType = entity;
        }

        public ViolationType(MySqlDataReader reader)
        {
            this.id = Retrieve.GetValueOrDefault<int>(reader, Field.ID);
            this.title = Retrieve.GetValueOrDefault<string>(reader, Field.TITLE);
            this.details = Retrieve.GetValueOrDefault<string>(reader, Field.DETAILS);
            this.numOfDays = Retrieve.GetValueOrDefault<int>(reader, Field.NUM_OF_DAYS);
            this.entityType = (Retrieve.GetValueOrDefault<string>(reader, Field.ENTITY_TYPE).Equals("OPERATOR")) ? General.OPERATOR : General.DRIVER;
        }

        public int Save()
        {
            if (violationType == null)
            {
                violationType = new Upsert(Table.VIOLATION_TYPE, id);
            }

            violationType.Insert(Field.TITLE, title);
            violationType.Insert(Field.DETAILS, details);
            violationType.Insert(Field.NUM_OF_DAYS, numOfDays);
            violationType.Insert(Field.ENTITY_TYPE, (entityType == General.OPERATOR) ? "OPERATOR" : "DRIVER");
            violationType.Save();
            id = violationType.id;

            return id;
        }

        public override string ToString()
        {
            return title ?? string.Empty;
        }

        public bool Delete()
        {
            if (violationType == null)
            {
                violationType = new Upsert(Table.VIOLATION_TYPE, id);
                violationType.Insert(Field.ISDELETED, true);
                violationType.Save();
                return true;
            }
            return false;
        }
    }
}
