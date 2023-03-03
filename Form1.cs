using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Security.Policy;
using System.Net.NetworkInformation;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {

        public Form1()
        {
            InitializeComponent();
        }
        //витягуємо з БД усі бази даних
        private void Form1_Load(object sender, EventArgs e)
        {
            using (SqlConnection connection = new SqlConnection("Data Source=DESKTOP-GCHC06A\\SQLEXPRESS; Integrated Security= true"))
            {
                connection.Open();

                var sqlCmd = new SqlCommand();

                sqlCmd.Connection = connection;
                sqlCmd.CommandType = CommandType.Text;
                sqlCmd.CommandText = "Select name from master.sys.databases";

                var adapter = new SqlDataAdapter(sqlCmd);

                var dataset = new DataSet();
                adapter.Fill(dataset);

                DataTable dbDataBases = dataset.Tables[0];

                for (int i = 0; i < dataset.Tables[0].Rows.Count; i++)
                {
                    comboBox1.Items.Add(dataset.Tables[0].Rows[i][0].ToString());

                    connection.Close();
                }
            }
        }
        //вибираємо файл для імпорта
        private void LoadFile_Click(object sender, EventArgs e)
        {

            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "All files (*.*) | *.*";
                ofd.Title = "Choose the file";

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    FileName_Load.Text = ofd.FileName;
                }
            }
        }
        private void Import_Click(object sender, EventArgs e)
        {
            ColumnHelper columnHelper = new ColumnHelper();
            columnHelper.InsertToDB(FileName_Load.Text, comboBox1.Text, comboBox2.Text);


            label1.Text = "Імпортовано";
            fillGridview();

        }
        //Імпортуємо дані з файла
        /*private void Import_Click(object sender, EventArgs e)
        {
            var lineNumber = 0;
            SqlConnection con = new SqlConnection(@"Data Source=DESKTOP-GCHC06A\SQLEXPRESS; Integrated Security= true");

            con.Open();

            StreamReader reader = new StreamReader(FileName_Load.Text);


            var tableColumns = ReadColumsTable();

            string fline = File.ReadLines(FileName_Load.Text).First();
            var csvColumns = fline.Split(',');

            var orderArray = SearchIndex(tableColumns, csvColumns);

            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                if (lineNumber != 0 && line != "")
                {

                        var values = line.Split(',', ';', '=', '_');

                        int[] order = new int[orderArray.Length];
                        Array.Copy(orderArray, order, orderArray.Length);
                        string[] csvCols = new string[csvColumns.Length];
                        Array.Copy(csvColumns, csvCols, csvColumns.Length);

                        var orderedValues = SwapCSVFileValues(values, order, tableColumns, csvCols);

                        var columns = new StringBuilder();

                        for (int i = 0; i < orderedValues.Count(); i++)
                        {
                            var value = orderedValues[i];
                            columns.Append(value);

                            if (i != orderedValues.Length - 1)
                            {
                                columns.Append("','");
                            }

                        }
                        var sql = "INSERT INTO " + comboBox1.Text + ".dbo." + comboBox2.Text + " VALUES ('" + columns.ToString() + "')";

                    var cmd = new SqlCommand();
                    cmd.CommandText = sql;
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.Connection = con;
                    cmd.ExecuteNonQuery();
                                   
                    lineNumber++;                   
                }
                                
            }
            con.Close();
            label1.Text = "Імпортовано";
            fillGridview();

        }*/
        //Імпортуємо дані з файла

        /*public string[] SwapCSVFileValues(string[] values, int[] order, List<string> tableColumns, string[] csvcols)
        {

            for (int i = 0; i < values.Length; i++)
            {
                if (tableColumns[i] != csvcols[i])
                {
                    //swap csv values according to order
                    int indexToChange = Array.IndexOf(order, i);
                    string tmp = values[indexToChange];
                    values[indexToChange] = values[i];
                    values[i] = tmp;

                    //swap csv columns
                    string csvColTmp = csvcols[indexToChange];
                    csvcols[indexToChange] = csvcols[i];
                    csvcols[i] = csvColTmp;

                    //swap order
                    int orderTmp = order[indexToChange];
                    order[indexToChange] = order[i];
                    order[i] = orderTmp;
                }
            }
            return values;
        }*/


        //Витягуємо таблиці із БД
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            comboBox2.Items.Clear();
            SqlConnection conne = new SqlConnection(@"Data Source=DESKTOP-GCHC06A\SQLEXPRESS; Initial Catalog = '" + comboBox1.Text + "'; Integrated Security = True");

            conne.Open();

            DataTable tables = conne.GetSchema("Tables");


            foreach (DataRow row in tables.Rows)
            {
                comboBox2.Items.Add(row[2].ToString());

            }
            conne.Close();
        }
        //Відтворення даних із БД

        /*public List<string> ReadColumsTable()
        {
            SqlConnection conne = new SqlConnection(@"Data Source=DESKTOP-GCHC06A\SQLEXPRESS; Initial Catalog = '" + comboBox1.Text + "'; Integrated Security = True");

            conne.Open();

            var sql = "SELECT name FROM sys.columns WHERE object_id = OBJECT_ID('" + comboBox2.Text + "')";
            List<string> tableColumns = new List<string>();

            var cmd = new SqlCommand();
            cmd.CommandText = sql;
            cmd.CommandType = System.Data.CommandType.Text;
            cmd.Connection = conne;
            var reader = cmd.ExecuteReader();


            while (reader.Read())
            {
                tableColumns.Add(reader.GetString(0));
            }
            conne.Close();
            return tableColumns;
        }*/
        /*public int[] SearchIndex(List<string> tableColumns, string[] csvcols)
        {

            //Список порядку перестановки  елементів за індексом
            int[] order = new int[tableColumns.Count];

            for (int i = 0; i < tableColumns.Count; i++)
            {
                if (tableColumns[i] == csvcols[i])
                {
                    order[i] = i;
                }
                else
                {
                    order[i] = tableColumns.IndexOf(csvcols[i]);
                }
            }
            return order;

        }*/
        public void fillGridview()
        {
            SqlConnection con = new SqlConnection(@"Data Source=DESKTOP-GCHC06A\SQLEXPRESS; Initial Catalog = '" + comboBox1.Text + "'; Integrated Security = True");

            con.Open();
            SqlDataAdapter ad = new SqlDataAdapter("select * from " + comboBox2.Text, con);
            DataTable dt = new DataTable();
            ad.Fill(dt);
            dataGridView1.DataSource = dt;
            con.Close();
        }
        //Синхронізація вибору таблиці та даниз із БД
        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {

            fillGridview();
        }
    }
}
