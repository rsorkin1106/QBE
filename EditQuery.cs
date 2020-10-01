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
    public partial class EditQuery : Form
    {
        string connectionstring = @"Driver={SQL Server}; Server=ROBS-LAPTOP\SQLEXPRESS; UID=BmSys; PWD=Robert99; Database=Test";
        private string[] x;
        Form1 mainForm;
        private List<string> paramNames;
        private List<TextBox> inputVariables;
        private XmlDocument xmlDoc;
        public EditQuery(string[] items, XmlDocument y, Form1 form)
        {
            x = items;
            mainForm = form;
            xmlDoc = y;
            InitializeComponent();
            
        }

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void EditQuery_Load(object sender, EventArgs e)
        {
            lblID.Text = x[0];
            lblStatus.Text = x[1];
            lblType.Text = x[2];
            tbName.Text = x[3];
            tbDesc.Text = x[4];
            lblQuery.Text = x[5];
            paramNames = new List<string>();
            inputVariables = new List<TextBox>();
            using (OdbcConnection odbcCon = new OdbcConnection(connectionstring))
            {
                odbcCon.Open();

                //Gets all required parameter names for stored procedure or column names fo view/table
                if (lblType.Text == "1")
                {
                    using (OdbcCommand command = new OdbcCommand($"Select [NAME] from sys.parameters where object_id = object_id('dbo.{x[5]}')", odbcCon))
                    {
                        using (OdbcDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                paramNames.Add(reader.GetString(0));
                            }
                        }
                    }
                    //Adds checkboxes and textboxes to enter parameters and their values
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
                    using (OdbcCommand command = new OdbcCommand($"SELECT [COLUMN_NAME] FROM INFORMATION_SCHEMA.COLUMNS WHERE[TABLE_NAME] = '{x[5]}'", odbcCon))
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
            checkBoxes();
        }


        //Takes parameters from current XML file and fills this in
        private void checkBoxes()
        {
            List<string> checkedName = new List<string>();
            List<string> checkedLabel = new List<string>();
            XmlNodeList nodes = xmlDoc.DocumentElement.SelectNodes("/Parameters/Parameter");
            int count = 0;
            foreach (XmlNode node in nodes)
            {
                checkedLabel.Add(node.SelectSingleNode("Label").InnerText);
                checkedName.Add(node.SelectSingleNode("Name").InnerText);
                for (int i = 0; i < checkedListBox1.Items.Count; ++i)
                {
                    if (lblType.Text == "1")
                    {
                        checkedListBox1.SetItemChecked(i, true);
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
                inputVariables[count++].Text = node.SelectSingleNode("Label").InnerText;
            }
        }

        //Updates the query to change its name/description/parameters to whatever was picked
        private void btnEdit_Click_1(object sender, EventArgs e)
        {
            XDocument xDoc = new XDocument(
               new XDeclaration("1.0", "utf-8", "yes"),
                new XElement("Parameters", createXML()));
            using (OdbcConnection connection = new OdbcConnection(connectionstring))
            {
                using (OdbcCommand command = new OdbcCommand("UPDATE dbo.QueriesTable SET TypeID = ?, Name = ?," +
                    "Description = ?, Query = ?, Parameters = ?, UpdateDate = ? Where ID = ?", connection))
                {
                    XmlDocument newDoc = new XmlDocument();
                    using (var xmlReader = xDoc.CreateReader())
                    {
                        newDoc.Load(xmlReader);
                    }
                    command.Parameters.AddWithValue("@type", lblType.Text);
                    command.Parameters.AddWithValue("@name", tbName.Text);
                    command.Parameters.AddWithValue("@desc", tbDesc.Text);
                    command.Parameters.AddWithValue("@q", lblQuery.Text);
                    command.Parameters.Add("@params", System.Data.Odbc.OdbcType.NText).Value = newDoc.InnerXml;
                    command.Parameters.AddWithValue("@up", DateTime.Now);
                    command.Parameters.AddWithValue("@id", Int32.Parse(lblID.Text));

                    connection.Open();

                    command.ExecuteNonQuery();

                    connection.Close();
                }
            }
            mainForm.threadResetList();
            this.Close();
        }

        //Creates XML with every parameter as an element
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
    }
}
