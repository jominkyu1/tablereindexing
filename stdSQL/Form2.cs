using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace stdSQL
{
    public partial class Form2 : Form
    {
        private const string ConStrName = "local";
        private const string ProviderName = "Microsoft.Data.SqlClient";
        private static Form1 form1;

        public event EventHandler conStrChanged;
        public Form2()
        {
            InitializeComponent();
            this.CenterToScreen();
        }
        private void Form2_Load(object sender, EventArgs e)
        {
            var conStr = ConfigurationManager.ConnectionStrings?[ConStrName];
            if (conStr != null && form1 == null) // start with config
            {
                Form1Show();
                this.Close();
            }

            if (conStr != null) //when modify
            {
                LoadConfig();
            }

            txtID.Focus();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string conStr = $"Server={txtServer.Text};Database={txtDB.Text};User Id={txtID.Text};Password={txtPW.Text};";
            if (ConnectionTest(conStr) == false)
            {
                MessageBox.Show("연결 정보가 올바르지 않습니다.", "ERROR", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            AddOrUpdateConfig(ConStrName, conStr); //with event
            Program.EncryptConStr();
            //Program.DecryptConStr();

            Form1Show();
            this.Close();
        }

        private void AddOrUpdateConfig(string name, string connectionStrings)
        {
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var conSec = (ConnectionStringsSection)config.GetSection("connectionStrings");

            var configConStr = conSec.ConnectionStrings[name];

            if (configConStr == null)
            {
                conSec.ConnectionStrings.Add(new ConnectionStringSettings(name, connectionStrings, ProviderName));
            }
            else
            {
                configConStr.ConnectionString = connectionStrings;
                configConStr.ProviderName = ProviderName;
            }

            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("connectionStrings");
            conStrChanged?.Invoke(this, EventArgs.Empty); //connectingStrings change EVENT

            MessageBox.Show("연결 정보가 저장되었습니다.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        private void LoadConfig()
        {
            var config = ConfigurationManager.ConnectionStrings[ConStrName]?.ToString();
            var builder = new SqlConnectionStringBuilder(config);

            txtID.Text = builder.UserID;
            txtPW.Text = builder.Password;
            txtDB.Text = builder.InitialCatalog;
            txtServer.Text = builder.DataSource;
        }

        private void Form2_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (form1 == null) Application.Exit();
        }

        private void Form1Show()
        {
            if (form1 == null) form1 = new Form1();
            
            form1.Show();
        }

        private bool ConnectionTest(string connectionStrings)
        {
            try
            {
                using (var con = new SqlConnection(connectionStrings))
                {
                    con.Open();
                    return true;
                }
            }
            catch (SqlException)
            {
                return false;
            }
        }
    }
}
