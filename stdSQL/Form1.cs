using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Channels;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace stdSQL
{
    public partial class Form1 : Form
    {
        private string _conStr;

        public Form1()
        {
            InitializeComponent();
            try
            {
                _conStr = ConfigurationManager.ConnectionStrings["local"]?.ConnectionString;
                if (_conStr == null) MessageBox.Show("local connectionstring NULL");
            }
            catch (ConfigurationErrorsException ex)
            {
                MessageBox.Show("연결정보 암호화를 위해 최초 설정 후 재시작이 필요합니다. \r\n프로그램을 다시 실행해주세요.");
                Application.Exit();
            }

            this.FormClosed += FormClosingHandler;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string strTables = RemoveDuplicate(textBox1.Text);

            using (var sqlCon = new SqlConnection(_conStr))
            using (var cmd = new SqlCommand())
            {
                sqlCon.Open();

                cmd.Connection = sqlCon;
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.CommandText = "dbo.getscandensity";

                var outParam = new SqlParameter
                {
                    Direction = ParameterDirection.Output,
                    SqlDbType = SqlDbType.VarChar,
                    Size = 500,
                    ParameterName = "@ReturnMessage"
                };
                var inParam = new SqlParameter
                {
                    Direction = ParameterDirection.Input,
                    ParameterName = "@TableNames",
                    SqlDbType = SqlDbType.VarChar,
                    Size = 100,
                    Value = strTables
                };

                cmd.Parameters.Add(outParam);
                cmd.Parameters.Add(inParam);


                var ds = new DataSet();
                using (var adapter = new SqlDataAdapter(cmd))
                {
                    try
                    {
                        adapter.Fill(ds);
                    }
                    catch (SqlException ex)
                    {
                        MessageBox.Show("데이터베이스 조회 실패. 테이블명 확인");
                        textBox1.Focus();
                        return;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                        textBox1.Focus();
                        return;
                    }
                }
                    
                dataGridView1.DataSource = ds.Tables[0];
                dataGridView1.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                label1.Text = outParam.Value.ToString();
            }
        }

        private string RemoveDuplicate(string str)
        {
            var list =  str.Split(',')
                .Select(s => s.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();
            return string.Join(",", list);
        }

        private void FormClosingHandler(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void BtnDBSettings_Click(object sender, EventArgs e)
        {
            Form2 form2 = new Form2();
            //when constr changed
            form2.conStrChanged += Form2_ConStrChanged;
            form2.ShowDialog(this);
        }

        private void Form2_ConStrChanged(object sender, EventArgs e)
        {
            this._conStr = ConfigurationManager.ConnectionStrings["local"].ConnectionString;
        }
    }
}
