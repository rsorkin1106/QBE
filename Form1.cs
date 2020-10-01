using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Entity;
using System.Data.Odbc;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Drawing;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;

namespace QBE
{

    /// <summary>
    /// Query By Example with basic add/edit/execute features
    /// 
    /// Table in use: Test.dbo.QueriesTable
    /// Field          Type         Description
    /// 
    /// ID             Int          Identity
    /// StatusID       Int          1 - Active, 2 - Inactive
    /// TypeID         Int          1 - Stored Procedure, 2 - View, 3 - Table
    /// Name           Varchar(50)  Name of query
    /// Description    Varchar(250) Description of what query does
    /// Query          Varchar(max) The name of Stored Procedure/View/Table that will be used
    /// Parameters     XML          All columns/parameters chosen as well as the value being input
    /// CreateDate     DateTime     Time of query's creation
    /// UpdateDate     DateTime     Time of query's most recent addition/edit/execution
    /// </summary>
    public partial class Form1 : Form
    {
        private string[] edit;
        private string[] add;
        private string selectedQuery;
        private TestEntities1 db;
        private Thread addThread;
        private Thread editThread;
        private Thread execThread;
        private AddQuery a;
        private EditQuery f;
        private XmlDocument xDoc;
        string connectionstring = "Driver={SQL Server}; Server=ROBS-LAPTOP\\SQLEXPRESS; UID=BmSys; PWD=Robert99; Database=Test";
        public Form1()
        {
            InitializeComponent();
            db = new TestEntities1();
            edit = new string[1];
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            resetList();
            dtCreate.Value = new DateTime(1753, 1, 1);
            dtUpdate.Value = new DateTime(1753, 1, 1);
            btnAdd.Enabled = false;
            btnExecute.Enabled = false;
            btnEdit.Enabled = false;
            xDoc = new XmlDocument();
        }


        //Resets listView from different threads
        public void threadResetList()
        {
            lvQuery.Invoke(new updates(resetList));
        }

        public delegate void updates();

        //Resets listView per any changes to database
        public void resetList()
        {
            db = new TestEntities1();
            lvQuery.BeginUpdate();
            lvQuery.Items.Clear();

            var queries = db.QueriesTables.ToList();
            foreach (var q in queries)
            {
                var row = new string[] { q.ID.ToString(), q.StatusID.ToString(), q.TypeID.ToString(), q.Name,
                    q.Description, q.Query, q.Parameters, q.CreateDate.ToString(), q.UpdateDate.ToString()};
                var lvi = new ListViewItem(row);
                lvQuery.Items.Add(lvi);
            }
            //lvQuery.Update();
            //lvQuery.Refresh();
            lvQuery.EndUpdate();
        }

        //Changes listView depending on search fields
        private void updateList(List<QueriesTable> x)
        {
            lvQuery.BeginUpdate();
            lvQuery.Items.Clear();

            foreach (var q in x)
            {
                var row = new string[] { q.ID.ToString(), q.StatusID.ToString(), q.TypeID.ToString(), q.Name,
                    q.Description, q.Query, q.Parameters, q.CreateDate.ToString(), q.UpdateDate.ToString()};
                var lvi = new ListViewItem(row);
                lvQuery.Items.Add(lvi);
            }
            
            lvQuery.EndUpdate();
        }

        //Clears search fields
        private void btnClear_Click(object sender, EventArgs e)
        {
            resetList();
            tbID.Text = "";
            tbStatus.Text = "";
            tbType.Text = "";
            tbName.Text = "";
            tbDesc.Text = "";
            dtCreate.Value = new DateTime(1753, 1, 1);
            dtUpdate.Value = new DateTime(1753, 1, 1);
        }

        //Takes search fields and displays any queries with matching fields
        private void btnSearch_Click(object sender, EventArgs e)
        {
            var queries = db.QueriesTables.ToList();
            if (!string.IsNullOrWhiteSpace(tbID.Text))
            {
                var tempList = from query in queries
                           where query.ID == Int32.Parse(tbID.Text)
                           select query;
                queries = tempList.ToList();
            }
            if (!string.IsNullOrWhiteSpace(tbStatus.Text))
            {
                var tempList = from query in queries
                               where query.StatusID == Int32.Parse(tbStatus.Text)
                               select query;
                queries = tempList.ToList();
            }
            if (!string.IsNullOrWhiteSpace(tbType.Text))
            {
                var tempList = from query in queries
                               where query.TypeID == Int32.Parse(tbType.Text)
                               select query;
                queries = tempList.ToList();
            }
            if (!string.IsNullOrWhiteSpace(tbName.Text))
            {
                var tempList = from query in queries
                               where query.Name.Contains(tbName.Text)
                               select query;
                queries = tempList.ToList();
            }
            if (!string.IsNullOrWhiteSpace(tbDesc.Text))
            {
                var tempList = from query in queries
                               where query.Description.Contains(tbDesc.Text)
                               select query;
                queries = tempList.ToList();
            }
            if (cbQuery.SelectedIndex > -1)
            {
                var tempList = from query in queries
                               where query.Query.Contains((string)cbQuery.SelectedItem)
                               select query;
                queries = tempList.ToList();
            }
            if(dtCreate.Value.Year != 1753)
            {
                var tempList = from query in queries
                               where query.CreateDate == dtCreate.Value
                               select query;
                queries = tempList.ToList();
            }
            if (dtUpdate.Value.Year != 1753)
            {
                var tempList = from query in queries
                               where query.UpdateDate == dtUpdate.Value
                               select query;
                queries = tempList.ToList();
            }
            updateList(queries);
        }

        //Makes sure that there is a stored procedure (1), view (2), or table (3) by that name
        private void btnVerify_Click(object sender, EventArgs e)
        {
            
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            editThread = new Thread(editApp);
            editThread.Start();
        }
        private void editApp()
        {
            try
            {
                if (edit.Length == 6)
                {
                    f = new EditQuery(edit, xDoc, this);
                    Application.Run(f);
                }
                else
                    throw new Exception("Select a query to edit");
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        private void lvQuery_MouseClick(object sender, MouseEventArgs e)
        {
            edit = new string[] { lvQuery.SelectedItems[0].SubItems[0].Text, lvQuery.SelectedItems[0].SubItems[1].Text, lvQuery.SelectedItems[0].SubItems[2].Text,
                lvQuery.SelectedItems[0].SubItems[3].Text, lvQuery.SelectedItems[0].SubItems[4].Text, lvQuery.SelectedItems[0].SubItems[5].Text };
            var temp = lvQuery.SelectedItems[0].SubItems[6].Text;
            xDoc.LoadXml(lvQuery.SelectedItems[0].SubItems[6].Text);
            btnExecute.Enabled = true;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            btnAdd.Enabled = false;
            addThread = new Thread(addApp);
            addThread.Start();
        }

        private void addApp()
        {
            add = new string[] { tbType.Text, tbName.Text, tbDesc.Text, selectedQuery };
            a = new AddQuery(add, this);
            Application.Run(a);
        }

        private void btnExecute_Click(object sender, EventArgs e)
        { 
            execThread = new Thread(execApp);
            execThread.Start();
        }

        private void execApp()
        {
            ExecuteQuery t;
            try
            {
                if (edit.Length == 6)
                {
                    lvQuery.Invoke(new MethodInvoker(delegate { lvQuery.SelectedItems[0].SubItems[1].Text = "1"; }));
                    t = new ExecuteQuery(edit, xDoc, this);
                    Application.Run(t);
                }
                else
                    throw new Exception("Select a query to execute");
            }
            catch(Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        private void tbType_TextChanged(object sender, EventArgs e)
        {
            cbQuery.SelectedIndex = -1;
            string query;
            cbQuery.Items.Clear();
            switch (tbType.Text)
            {
                case "1":
                    query = "Select [NAME] from sysobjects where type = 'P' and category = 0";
                    break;

                case "2":
                    query = "Select [NAME] from sysobjects where type = 'V' and category = 0";
                    break;

                case "3":
                    query = "SELECT sobjects.name FROM sysobjects sobjects WHERE sobjects.xtype = 'U'";
                    break;

                case "":
                    return;

                default:
                    MessageBox.Show("Please enter a valid Type ID: 1 (Stored Procedure), 2 (View), 3 (Table)");
                    cbQuery.Enabled = false;
                    return;

            }
            List<String> columnData = new List<String>();

            cbQuery.Enabled = true;
            using (OdbcConnection connection = new OdbcConnection(connectionstring))
            {
                connection.Open();
                using (OdbcCommand command = new OdbcCommand(query, connection))
                {
                    using (OdbcDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            columnData.Add(reader.GetString(0));
                        }
                    }
                }
            }
            foreach (var name in columnData)
            {
                cbQuery.Items.Add(name);
            }
        }

        private void cbQuery_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbQuery.SelectedIndex > -1)
            {
                selectedQuery = (string)cbQuery.SelectedItem;
                btnAdd.Enabled = true;
            }
            else
                btnAdd.Enabled = false;
        }

        private void lvQuery_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(lvQuery.SelectedIndices.Count > 0)
            {
                btnExecute.Enabled = true;
                btnEdit.Enabled = true;
            }
            else
            {
                btnExecute.Enabled = false;
                btnEdit.Enabled = false;
            }
        }
    }
}
