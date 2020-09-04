using DataClean.Util;
using DataModels;
using LinqToDB;
using LinqToDB.Data;
using LinqToDB.Extensions;
using MetroFramework.Controls;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace DataClean.Model.Page
{
    public class MainPageModel: BaseModel,IMainPageModel
    {
        public List<string> columnNames = new List<string>();
        public MainPageModel()
        {
            initComponent();
        }

        public override void initComponent()
        {
            initTreeView();         
        }
      

        private void initTreeView()
        {
            List<Category>  list = appControlers.mainPageControler.getTVDByParentId(0);
            appViews.window.treeView1.Nodes.Clear();
            for (int i = 0; i < list.Count; i++) 
            {
                Category category = list[i];
                TreeNode node = appViews.window.treeView1.Nodes.Add(category.Id.ToString(), category.Name, (int)SystemImages.FOLDER, (int)SystemImages.FOLDER);
                addSonTreeNode(node, (int)category.Id);
            }
        }

        private void addSonTreeNode(TreeNode parent,int parentId) 
        {
            List<Category> sons = appControlers.mainPageControler.getTVDByParentId(parentId);
            if (sons != null && sons.Count > 0)  {
                foreach (Category son in sons)  {
                    TreeNode sonNode;
                    if (son.Isdata == 0) {
                        sonNode = parent.Nodes.Add(son.Id.ToString(), son.Name, (int)SystemImages.FOLDER, (int)SystemImages.FOLDER);
                        addSonTreeNode(sonNode,(int)son.Id);
                    } else {
                        sonNode = parent.Nodes.Add(son.Id.ToString(), son.Name, (int)SystemImages.FILE, (int)SystemImages.FILE);
                    }
                }
            }
        }

        public void addTreeView(string name)
        {
            name = name.ToLower().Replace(".xlsx","").Replace(".xls","");
            TreeNode selectnode = appViews.window.treeView1.SelectedNode;
            if (selectnode != null)
            {
                int id = 0;
                int.TryParse(selectnode.Name, out id);
                long max = database.Categories.Max(table => table.Id) +1;
                Category category = new Category();
                category.Id = max;
                category.Name = name;
                category.Isdata = 1;
                category.ParentId = id;
                TreeNode node = selectnode.Nodes.Add(max.ToString(), name, (int)SystemImages.FILE, (int)SystemImages.FILE);
                node.Checked = true;
                database.InsertAsync<Category>(category);
            }
        }
        private delegate void updategridview();
        public void UpdateGridView(string tablename) 
        {         
            //DataTable clear 只能清除数据，不会改变结构
            cacheTable = new DataTable();           
            string sql = "SELECT * FROM '" + tablename + "' limit " + (currentPage - 1) * pageSize + "," + pageSize;
            var reader = database1.ExecuteReader(sql);         
            cacheTable.Load(reader.Reader);
            int count = database1.Query<int>("SELECT COUNT(*) FROM '"+ tablename + "'").First();
            totalPage = (count + pageSize - 1) / pageSize;
         
            appViews.window.Invoke(new updategridview(() => _UpdateGridView(tablename)));
        }

        public DataTable cacheTable = new DataTable();
        public string cacheName = "";
        private int currentPage = 1;
        private int pageSize = 20;
        private int totalPage = 1;

        private void _UpdateGridView(string tablename) {
            if (tablename != "") {
                appViews.window.metroGrid1.Columns.Clear();             
                cacheName = tablename;              
                appViews.window.metroGrid1.DataSource = null;
                appViews.window.metroGrid1.DataSource = cacheTable;           
                appViews.window.metroGrid1.Refresh();
                appViews.window.metroPanel1.Visible = true;
                appViews.window.metroLabel1.Text = tablename+" "+ currentPage + "/" + totalPage;
               
            }               
        }
        private string dataName;
        public void onDataSourceChanged()
        {
            if (appViews.window.metroGrid1.Columns.Count > 0 && cacheName!= dataName)
            {
                dataName = cacheName;
                columnNames.Clear();
                appViews.window.listBox1.Items.Clear();
                appViews.window.metroGrid1.Columns[0].Visible = false;
                int count = cacheTable.Columns.Count;      
                for (int i = 0; i < count; i++)
                {
                    if (cacheTable.Columns[i].ColumnName == "d_tid")
                    {
                        appViews.window.metroGrid1.Columns[i].Visible = false;
                    }
                    else {
                        appViews.window.metroGrid1.Columns[i].Visible = true;
                    }
                    string type = "";
                    string name = cacheTable.Columns[i].ColumnName;

                    if (cacheTable.Columns[i].DataType.IsIntegerType() || cacheTable.Columns[i].DataType.IsFloatType())
                    {
                        type = " integer";
                    }
                    else
                    {
                        type = " text";
                    }

                    if (i > 0)
                    {
                        addColumnName(cacheTable.Columns[i].ColumnName);
                    }
                    appViews.window.metroGrid1.Columns[i].HeaderText = "'" + cacheTable.Columns[i].ColumnName + "'" + type;
                    appViews.window.metroGrid1.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;
                }
            }                   
        }

        public void NextPage() 
        {
            if (currentPage < totalPage) 
            {
                currentPage++;
                UpdateGridView(cacheName);
            }
        }      

        public void save()
        {
            if (cacheTable.Rows.Count > 0 && cacheName != "") {
                int rowCount = cacheTable.Rows.Count;
                int columnCount = cacheTable.Columns.Count;
                for (int i= 0; i < rowCount; i++) {
                    string column = "";
                    for (int j = 0; j < columnCount; j++) {
                        if (cacheTable.Columns[j].ColumnName != "d_tid") {
                            column += cacheTable.Columns[j].ColumnName + "='" + cacheTable.Rows[i][cacheTable.Columns[j].ColumnName] + "',";
                        }                   
                    }
                    column = column.Substring(0, column.Length - 1);
                    column += " WHERE d_tid='" + cacheTable.Rows[i]["d_tid"] + "'";
                    string sql = "UPDATE '" + cacheName + "' SET " + column;
                    database1.Execute(sql);
                }
            }           
        }

        public void PreviousPage()
        {
            if (currentPage > 1) {
                currentPage--;
                UpdateGridView(cacheName);
            }
        }

        public void FirstPage()
        {
            currentPage = 1;
            UpdateGridView(cacheName);
        }

        public void LastPage()
        {
            currentPage = totalPage;
            UpdateGridView(cacheName);
        }

        public void addColumnName(string name)
        {
            if (!columnNames.Contains(name)) 
            {
                columnNames.Add(name);
                appViews.window.listBox1.Items.Add(name);
            }           
        }

        public void removeColumnName(string name)
        {
            columnNames.Remove(name);
            appViews.window.listBox1.Items.Remove(name);
        }

        public void DataClean()
        {
            if (appViews.window.metroCheckBox1.Checked == appViews.window.metroCheckBox2.Checked == appViews.window.metroCheckBox3.Checked == false)
            {
                return;
            }
            appViews.window.metroLabel3.Text = "";
            if (appViews.window.listBox1.Items.Count > 0 && cacheName!="") {
                string avgs = "";
                string column = "";
                avgs += " -t " + cacheName;
                for (int i = 0; i < appViews.window.listBox1.Items.Count; i++) {
                    column += appViews.window.listBox1.Items[i] + ",";
                }
                column = column.Substring(0, column.Length - 1);
                avgs += " -c " + column.Replace(" ","#");
               avgs += " -d " + appViews.window.metroCheckBox1.Checked.ToString();
                avgs += " -e " + appViews.window.metroCheckBox1.Checked.ToString();
                avgs += " -f " + appViews.window.metroCheckBox1.Checked.ToString();
                PythonRunTime.Instance.Execute(cacheName,avgs);
            }
        }

        public void updateWindowText(string text)
        {
          
            appViews.window.Invoke(new updategridview(() =>
            {
                appViews.window.metroLabel3.Text = "当前任务:"+text+"/5";
                initTreeView();
            }));
        }
    }
}
