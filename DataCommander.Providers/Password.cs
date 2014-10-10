namespace SqlUtil.Providers
{
    using System;
    using System.Text;
    using System.Security.Cryptography;
    using WAVE.Foundation.Data;
    using WAVE.Foundation.Configuration;

    sealed class Password
    {
        public Password(string connectionString)
        {
            SHA1CryptoServiceProvider sha1 = new SHA1CryptoServiceProvider();
            byte[] bytes = Encoding.UTF8.GetBytes(connectionString);
            byte[] hash = sha1.ComputeHash(bytes);
            folderName = Convert.ToBase64String(hash);
        }

        PropertyFolder Folder
        {
            get
            {
                if (folder == null)
                {
                    folder = Application.Instance.ApplicationData.CurrentType;

                    if (!folder.SubFolders.ContainsKey(folderName))
                        folder = new PropertyFolder(folder, folderName);
                    else
                        folder = folder.SubFolders[folderName];
                }

                return folder;
            }
        }

        public string Value
        {
            get
            {
                string password = null;

                if (Folder.Properties.ContainsKey("Password"))
                {
                    string base64 = folder.Properties.GetString("Password");

                    if (base64 != null)
                    {
                        byte[] bytes = Convert.FromBase64String(base64);
                        password = dataProtector.Decrypt(bytes, null);
                    }
                    else
                    {
                        password = null;
                    }
                }

                return password;
            }
            set
            {
                if (value != null)
                {
                    byte[] bytes = dataProtector.Encrypt(value, null);
                    string base64 = Convert.ToBase64String(bytes);
                    Folder.Properties["Password"] = base64;
                }
                else
                {
                    PropertyFolder folder = Folder;
                    PropertyFolder parent = Folder.Parent;
                    parent.SubFolders.Remove(folder);
                }
            }
        }

        string folderName;
        PropertyFolder folder;
    }
}