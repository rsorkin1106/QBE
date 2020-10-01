using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Odbc;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;

namespace QBE
{

    

    public partial class AddQuery : Form
    {
        Form1 mainForm;
        string connectionstring = "Driver={SQL Server}; Server=ROBS-LAPTOP\\SQLEXPRESS; UID=BmSys; PWD=Robert99; Database=Test";
        private string type;
        private string name;
        private string desc;
        private string query;
        private List<string> paramNames;
        private List<TextBox> inputVariables;
        public AddQuery(string[] x, Form1 f)
        {
            type = x[0];
            name = x[1];
            desc = x[2];
            query = x[3];
            mainForm = f;
            InitializeComponent();
        }

        private void AddQuery_Load(object sender, EventArgs e)
        {
            //Fills in any fields that were already typed in main window
            lblType.Text = type;
            if (type == "1")
                lblParams.Text = "Parameters";
            else
                lblParams.Text = "Columns";
            lblQuery.Text = query;
            tbDesc.Text = desc;
            tbName.Text = name;
            paramNames = new List<string>();
            inputVariables = new List<TextBox>();
            using (OdbcConnection odbcCon = new OdbcConnection(connectionstring))
            {
                odbcCon.Open();

                //Takes parameters from current XML file and fills the cehckboxes/textboxes in
                if (lblType.Text == "1")
                {
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
                    
                }
                else
                {
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

                        inputVariables.Add(new TextBox());
                        inputVariables[i].Visible = false;

                        fLPParam.Controls.Add(inputVariables[i]);
                    }
                }
                odbcCon.Close();
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {

            XDocument xDoc = new XDocument(
                new XDeclaration("1.0", "utf-8", "yes"),
                 new XElement("Parameters", createXML()));

            //Adds newest query to query database
            using (OdbcConnection connection = new OdbcConnection(connectionstring))
            {
                using (OdbcCommand command = new OdbcCommand("INSERT INTO dbo.QueriesTable(TypeID, Name, Description, Query, Parameters) VALUES (?, ?, ?, ?, ?)", connection))
                {
                    XmlDocument xmlDoc = new XmlDocument();
                    using (var xmlReader = xDoc.CreateReader())
                    {
                        xmlDoc.Load(xmlReader);
                    }

                    command.Parameters.AddWithValue("@type", lblType.Text);
                    command.Parameters.AddWithValue("@name", tbName.Text);
                    command.Parameters.AddWithValue("@desc", tbDesc.Text);
                    command.Parameters.AddWithValue("@q", lblQuery.Text);
                    command.Parameters.Add("@params", System.Data.Odbc.OdbcType.NText).Value = xmlDoc.InnerXml;

                    connection.Open();

                    command.ExecuteNonQuery();

                    connection.Close();
                }
            }
            mainForm.threadResetList();
            this.Close();
        }

        //Creates an XML with listed parameters
        private XElement[] createXML()
        {
            var result = new List<XElement>();
            for (int i = 0; i < checkedListBox1.CheckedItems.Count; i++)
            {
                result.Add(new XElement("Parameter",
                    new XElement("Name", paramNames[checkedListBox1.CheckedIndices[i]]),
                    new XElement("Label", inputVariables[i].Text)));
            }
            return result.ToArray();
        }
        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void checkedListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void checkedListBox1_ItemCheck(object sender, ItemCheckEventArgs e)
        {

        }
    }
}
