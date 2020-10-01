using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Odbc;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace QBE
{
    public partial class ExecuteQuery : Form
    {
        string connectionstring = "Driver={SQL Server}; Server=ROBS-LAPTOP\\SQLEXPRESS; UID=BmSys; PWD=Robert99; Database=Test";
        private string type;
        private string name;
        private string desc;
        private string query;
        private string view;
        private List<string> paramNames;
        private List<TextBox> inputVariables;
        private XmlDocument xmlDoc;
        Form1 mainForm;
        public ExecuteQuery(string[] x, XmlDocument y, Form1 f)
        {
            type = x[2];
            name = x[3];
            desc = x[4];
            query = x[5];
            xmlDoc = y;
            mainForm = f;
            InitializeComponent();
            
        }

        private void ExecuteQuery_Load(object sender, EventArgs e)
        {
            paramNames = new List<string>();
            inputVariables = new List<TextBox>();

            //Gets required parameters/column names depending on typeID
            using (OdbcConnection odbcCon = new OdbcConnection(connectionstring))
            {
                odbcCon.Open();
                if (type == "1")
                {
                    lblMsg.Text = $"Executing {query}. Selected Variables:";
                    using (OdbcCommand command = new OdbcCommand($"Select [NAME] from sys.parameters where object_id = object_id('dbo.{query}')", odbcCon))
                    {
                        using (OdbcDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                paramNames.Add(reader.GetString(0));
                            }
                        }
                    }
                    for (int i = 0; i < paramNames.Count; ++i)
                    {
                        checkedListBox1.Items.Add(paramNames[i]);

                        inputVariables.Add(new TextBox());
                        checkedListBox1.SetItemChecked(i, true);

                        fLPParam.Controls.Add(inputVariables[i]);
                    }
                    checkBoxes();
                }
                else
                {
                    if(type == "2")
                        lblMsg.Text = $"Executing {query}. Selected Columns:";
                    else
                        lblMsg.Text = $"Viewing Table {query}. Selected Columns:";
                    using (OdbcCommand command = new OdbcCommand($"USE TEST; SELECT [COLUMN_NAME] FROM INFORMATION_SCHEMA.COLUMNS WHERE[TABLE_NAME] = '{query}'", odbcCon))
                    {
                        using (OdbcDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                paramNames.Add(reader.GetString(0));
                            }
                        }
                    }
                    for (int i = 0; i < paramNames.Count; ++i)
                    {
                        checkedListBox1.Items.Add(paramNames[i]);
                    }
                    checkBoxes();
                    if (type != "1")
                    {
                        view = "SELECT ";
                        for (int i = 0; i < checkedListBox1.CheckedItems.Count; ++i)
                        {
                            view += "[" + checkedListBox1.CheckedItems[i] + "]";
                            if (i < checkedListBox1.CheckedItems.Count - 1)
                                view += ", ";
                            else
                                view += " ";
                        }
                        view += "FROM ";
                    }
                    
                }
                odbcCon.Close();
            }
        }

        //Takes parameters from current XML file and fills the cehckboxes/textboxes in
        private void checkBoxes()
        {
            List<string> labels = new List<string>();
            XmlNodeList nodes = xmlDoc.DocumentElement.SelectNodes("/Parameters/Parameter");
            foreach (XmlNode node in nodes)
            {
                for (int i = 0; i < checkedListBox1.Items.Count; ++i)
                {
                    if (type == "1")
                    {
                        labels.Add(node.SelectSingleNode("Label").InnerText);
                        break;
                    }
                    else
                    { 
                        if (paramNames[i] == node.SelectSingleNode("Name").InnerText)
                        {
                            checkedListBox1.SetItemChecked(i, true);
                            break;
                        }
                    }
                }
            }
            for(int i = 0; i < inputVariables.Count; ++i)
            {
                checkedListBox1.SetItemChecked(i, true);
                inputVariables[i].Text = labels[i];
            }
        }

        private void btnExec_Click(object sender, EventArgs e)
        {
            try
            {
                string schema;
                using (OdbcConnection connection = new OdbcConnection(connectionstring))
                {
                    //gets schema name
                    connection.Open();
                    if (type == "1")
                    {
                        using (OdbcCommand command = new OdbcCommand($"USE TEST; SELECT [schema] = OBJECT_SCHEMA_NAME([object_id]) FROM sys.procedures WHERE [name] = '{query}'", connection))
                        {
                            using (OdbcDataReader reader = command.ExecuteReader())
                            {
                                reader.Read();
                                schema = reader.GetString(0);
                            }
                        }
                    }
                    else
                    {
                        using (OdbcCommand command = new OdbcCommand($"USE TEST; SELECT [TABLE_SCHEMA] FROM INFORMATION_SCHEMA.COLUMNS WHERE[TABLE_NAME] = '{query}'", connection))
                        {
                            using (OdbcDataReader reader = command.ExecuteReader())
                            {
                                reader.Read();
                                schema = reader.GetString(0);
                            }
                        }
                    }

                    //Updates query's updateTime
                    using (OdbcCommand command = new OdbcCommand("UPDATE dbo.QueriesTable SET UpdateDate = ? Where Name = ?", connection))
                    {
                        //Update Query updateTime
                        command.CommandType = CommandType.Text;
                        command.Parameters.AddWithValue("@up1", DateTime.Now);
                        command.Parameters.AddWithValue("@name", name);

                        command.ExecuteNonQuery();
                    }


                    //Fills Data Grid with every tables it receives
                    if (type == "1")
                    {
                        string sp = "{call " + schema + "." + query;
                        if (inputVariables.Count != 0)
                        {
                            sp += "( ";
                            //Selects parameters
                            for (int i = 0; i < inputVariables.Count; ++i)
                            {
                                sp += "?";
                                if (i == inputVariables.Count - 1)
                                {
                                    break;
                                }
                                sp += ", ";
                            }
                            sp += ")";
                        }
                        sp += "}";
                        using (OdbcCommand command = new OdbcCommand(sp, connection))
                        {
                            command.CommandType = CommandType.StoredProcedure;

                            //Input all parameter values
                            for (int i = 0; i < inputVariables.Count; ++i)
                            {
                                command.Parameters.AddWithValue(paramNames[i], inputVariables[i].Text.Trim());
                            }
                            //command.ExecuteNonQuery();
                            OdbcDataAdapter odbcDa = new OdbcDataAdapter();
                            odbcDa.SelectCommand = command;
                            DataSet ds = new DataSet();
                            odbcDa.Fill(ds);
                            for (int i = 0; i < ds.Tables.Count; ++i)
                            {
                                DataGridView d = new DataGridView();
                                d.DataSource = ds.Tables[i];
                                flpGrid.Controls.Add(d);
                            }
                        }
                    }
                    else
                    {
                        view += schema + "." + query;
                        OdbcDataAdapter odbcDa = new OdbcDataAdapter(view, connection);
                        DataTable dtbl = new DataTable();
                        odbcDa.Fill(dtbl);
                        DataGridView dgv = new DataGridView();
                        dgv.DataSource = dtbl;
                        flpGrid.Controls.Add(dgv);
                    }
                    connection.Close();
                }
            }
            //Will usually be a parameter type mismatch
            catch(Exception error)
            {
                MessageBox.Show(error.Message);
            }
            finally
            {
                mainForm.threadResetList();
            }
        }
    }
}
