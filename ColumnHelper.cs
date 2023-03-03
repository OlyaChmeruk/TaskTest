using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp1
{
    internal class ColumnHelper
    {
        public void InsertToDB(string fileName, string dbName, string tableName)
        {
            var lineNumber = 0;
            SqlConnection con = new SqlConnection(@"Data Source=DESKTOP-GCHC06A\SQLEXPRESS; Integrated Security= true");

            con.Open();

            using (StreamReader reader = new StreamReader(fileName))
            {

                var tableColumns = ReadColumsTable(dbName, tableName);

                string fline = File.ReadLines(fileName).First();
               //string fline = reader.ReadLine();
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
                        var sql = "INSERT INTO " + dbName + ".dbo." + tableName + " VALUES ('" + columns.ToString() + "')";

                        var cmd = new SqlCommand();
                        cmd.CommandText = sql;
                        cmd.CommandType = System.Data.CommandType.Text;
                        cmd.Connection = con;
                        cmd.ExecuteNonQuery();


                    }
                    lineNumber++;

                }

            }
            con.Close();

        }

        public string[] SwapCSVFileValues(string[] values, int[] order, List<string> tableColumns, string[] csvcols)
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
        }
        public int[] SearchIndex(List<string> tableColumns, string[] csvcols)
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

        }
        public List<string> ReadColumsTable(string dbName, string tableName)
        {
            using (SqlConnection conne = new SqlConnection(@"Data Source=DESKTOP-GCHC06A\SQLEXPRESS; 
                   Initial Catalog = '" + dbName + "'; Integrated Security = True"))
            {
                conne.Open();

                var sql = "SELECT name FROM sys.columns WHERE object_id = OBJECT_ID('" + tableName + "')";
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
            }

        }
    }
}
