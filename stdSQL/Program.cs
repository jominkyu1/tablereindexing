using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace stdSQL
{
    static class Program
    {
        /// <summary>
        /// 해당 응용 프로그램의 주 진입점입니다.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Form2 form2 = new Form2();
            form2.Show();

            Application.Run();

            //ConnectionStrings 암호화
            //EncryptConStr();
            
            //ConnectionStrings 복호화
            //DecryptConStr();
        }

        //Encrypt ConnectionStrings
        public static void EncryptConStr()
        {
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var configSection = config.GetSection("connectionStrings");

            if (configSection != null && !configSection.SectionInformation.IsProtected)
            {
                //Encrypt
                configSection.SectionInformation.ProtectSection("DataProtectionConfigurationProvider");
                //Save App.config
                config.Save(ConfigurationSaveMode.Full);
                ConfigurationManager.RefreshSection("connectionStrings");
                Debug.WriteLine("ConnectionStrings Encrypted");
            }
            else
            {
                Debug.WriteLine("ConnectionStrings is already encrypted or could not find");
            }
        }

        public static void DecryptConStr()
        {
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var configSection = config.GetSection("connectionStrings");

            if (configSection != null && configSection.SectionInformation.IsProtected)
            {
                //Decrypt
                configSection.SectionInformation.UnprotectSection();
                //Save App.config
                config.Save(ConfigurationSaveMode.Full);
                Debug.WriteLine("ConnectionStrings Decrypted");
            }
            else
            {
                Debug.WriteLine("ConnectionStrings is not encrypted or could not find");
            }
        }

        public static bool HasConfigFile()
        {
            return ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None).HasFile;
        }
    }
}
