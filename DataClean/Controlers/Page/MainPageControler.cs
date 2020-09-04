using DataClean.Util;
using DataModels;
using LinqToDB;
using LinqToDB.Data;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DataClean.Controlers.Page
{
    public class MainPageControler : BaseControler
    {
        public List<Category> getTreeViewData()
        {
            List<Category> result = appControlers.dataControler.database.Categories.Where(table => table.Id > 0).ToList<Category>();
            return result;
        }

        public List<Category> getTVDByParentId(int parentid)
        {
            List<Category> result = appControlers.dataControler.database.Categories.Where(table => table.ParentId == parentid).ToList<Category>();
            return result;
        }

        public void onClickAdd(object sender, EventArgs e)
        {
            TreeNode selectnode = appViews.window.treeView1.SelectedNode;
            if (selectnode != null)
            {
                int id = 0;
                int.TryParse(selectnode.Name, out id);
                if (id == 1)
                {
                    OpenFileDialog dialog = new OpenFileDialog();
                    dialog.Multiselect = false;
                    dialog.Title = "请选择文件夹";
                    dialog.Filter = "(excel文件)|*.xls;*.xlsx";
                    if (dialog.ShowDialog() == DialogResult.OK)
                    {
                        appModels.mainPageModel.addTreeView(dialog.SafeFileName);
                        ExcelOperator.ImportToSqLite(dialog.FileName, AfterTableImport);
                    }
                }                
            }
        }

        public void onClickExport(object sender, EventArgs e)
        {
            if (appModels.mainPageModel.cacheName != "")
            {
                string saveFileName = "";
                SaveFileDialog saveDialog = new SaveFileDialog();
                saveDialog.DefaultExt = "xls";
                saveDialog.Filter = "Excel文件|*.xls";
                saveDialog.FileName = appModels.mainPageModel.cacheName;
                saveDialog.ShowDialog();
                saveFileName = saveDialog.FileName;
                //using (FileStream fs = new FileStream(saveFileName, FileMode.Create))
                {
                    DataTable cacheTable = new DataTable();
                    string sql = "SELECT * FROM " + appModels.mainPageModel.cacheName;
                    var reader = database1.ExecuteReader(sql);
                    cacheTable.Load(reader.Reader);
                    DataTableToExcel(cacheTable, saveFileName);
                }
            }
        }

        public bool DataTableToExcel(DataTable dataTable,string selectPath)
        {        

            int SheetCount = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(dataTable.Rows.Count) / 65535));
            bool result = false;
            IWorkbook workbook = null;
            FileStream fs = null;

            int count = 0;
            try
            {
                if (dataTable != null && dataTable.Rows.Count > 0)
                {
                    workbook = new HSSFWorkbook();
                    for (int k = 0; k < SheetCount; k++)
                    {
                        ISheet sheet = workbook.CreateSheet("Sheet" + k);
                        int rowCount = dataTable.Rows.Count;
                        int columnCount = dataTable.Columns.Count;
                        //设置列头  
                        IRow row = sheet.CreateRow(0);
                        for (int i = 0; i < columnCount; i++)
                        {
                            ICell cell = row.CreateCell(i);
                            cell.SetCellValue(dataTable.Columns[i].ColumnName);
                        }
                        int range = 0;
                        if (k == SheetCount - 1)
                        {
                            range = rowCount;
                        }
                        else
                        {
                            range = (k + 1) * 65535;
                        }
                        for (int i = k * 65535, l = 1; i < range; i++, l++)
                        {
                            row = sheet.CreateRow(l);
                            if (i <= rowCount)
                            {
                                for (int j = 0; j < columnCount; j++)
                                {
                                    ICell cell = row.CreateCell(j);
                                    cell.SetCellValue(dataTable.Rows[i][j].ToString());
                                    if (count < rowCount)
                                        count++;
                                }
                            }
                        }
                    }               
                    using (fs = File.OpenWrite(selectPath))
                    {
                        workbook.Write(fs);
                        result = true;
                    }
                }
                return result;
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex + "");
                if (fs != null)
                {
                    fs.Close();
                }
                return false;
            }
        }



        public void OnClickSave(object sender, EventArgs e)
        {
            appModels.mainPageModel.save();
        }

        public void OnClickFirst(object sender, EventArgs e)
        {
            appModels.mainPageModel.FirstPage();
        }

        public void OnClickPrevious(object sender, EventArgs e)
        {
            appModels.mainPageModel.PreviousPage();
        }

        public void OnClickNext(object sender, EventArgs e)
        {
            appModels.mainPageModel.NextPage();
        }

        public void OnClickLast(object sender, EventArgs e)
        {
            appModels.mainPageModel.LastPage();
        }

        public void onClickDataClean(object sender, EventArgs e)
        {
            appModels.mainPageModel.DataClean();
        }

        public void onClickShowProcess(object sender, EventArgs e)
        {
            appComponents.processDialog.ShowDialog(true);

        }

        private bool checkTableExist(string fullPath) {
            string fileName = Path.GetFileName(fullPath);
            string name = fileName.Replace(".xlsx", "").Replace(".xls", "");
            var num = Program.appControlers.dataControler.database.Execute<int>("SELECT count(*) FROM sqlite_master WHERE type='table' AND name='" + name + "'");
            return num == 0;
        }

        private void AfterTableImport(string obj) {
            appModels.mainPageModel.UpdateGridView(obj);

        }

        public void onClickTreeView(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Button == MouseButtons.Right) {
                if (e.Node.Parent != null)
                {
                    appViews.window.metroContextMenu1.Items.Clear();
                    //appViews.window.metroContextMenu1.Items.Add("新建目录", null, delegate (object o, EventArgs a) { onClickAddNode(e); });
                    appViews.window.metroContextMenu1.Items.Add("重命名", null, delegate (object o, EventArgs a) { onClickReanmeNode(e); });
                    appViews.window.metroContextMenu1.Items.Add("删除", null, delegate (object o, EventArgs a) { onClickDelNode(e); });
                    appViews.window.metroContextMenu1.Show(Cursor.Position);
                }
            }
            else {
                Category category = getCategoryByNode(e.Node);
                if (category != null) {
                    if (category.Isdata == 1) {
                        appModels.mainPageModel.UpdateGridView(category.Name);
                    }
                }
            }
        }

        private Category getCategoryByNode(TreeNode node) {
            int id = 0;
            int.TryParse(node.Name, out id);
            Category result = database.Categories.Where(table => table.Id == id).First<Category>();
            return result;
        }

        public void AfterTreeNodeEdited(object sender, NodeLabelEditEventArgs e)
        {
            if (e.Label!=null && e.Label.Length > 0) {
                int id = 0;             
                int.TryParse(e.Node.Name, out id);
                Category category = database.Categories.Where(table => table.Id == id).First();
                database.Categories.Where(table => table.Id == id).Set(table => table.Name, e.Label).UpdateAsync();
                database1.Execute("alter table  " + category.Name + " rename to " + e.Label);
            }
            else {
                e.CancelEdit = true;
            }           
        }

        public void AfterTreeNodeExpand(object sender, TreeViewEventArgs e)
        {
            if (e.Node.ImageIndex != (int)SystemImages.FILE)
            {
                e.Node.ImageIndex = (int)SystemImages.FOLDER_OPEN;
            }
        }

        public void AfterTreeNodeCollapse(object sender, TreeViewEventArgs e)
        {
            if (e.Node.ImageIndex != (int)SystemImages.FILE)
            {
                e.Node.ImageIndex = (int)SystemImages.FOLDER;
            }
        }

        public void GridViewHeadClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                appViews.window.metroContextMenu1.Items.Clear();
                appViews.window.metroContextMenu1.Items.Add("设置Int类型", null, delegate (object o, EventArgs a) { onSetInt(e); });
                appViews.window.metroContextMenu1.Items.Add("设置String类型", null, delegate (object o, EventArgs a) { onSetString(e); });
                appViews.window.metroContextMenu1.Show(Cursor.Position);
            }
            else {
                string name = appViews.window.metroGrid1.Columns[e.ColumnIndex].HeaderText;
                name = name.Replace(" integer", "").Replace(" text", "").Replace("'", "") ;
                appModels.mainPageModel.addColumnName(name);
            }
        }

        private void onSetInt(DataGridViewCellMouseEventArgs e)
        {

            string sql = "ALTER TABLE '" + appModels.mainPageModel.cacheName + "' RENAME TO '" + appModels.mainPageModel.cacheName + "_old'";
            Program.appControlers.dataControler.database1.Execute(sql);
            try { 
                string col = "";
                foreach (DataGridViewColumn column in appViews.window.metroGrid1.Columns)
                {
                    string colname = column.HeaderText;
                    if (appViews.window.metroGrid1.Columns[e.ColumnIndex].HeaderText == colname)
                    {
                        colname = appViews.window.metroGrid1.Columns[e.ColumnIndex].HeaderText.Replace("text", "integer");
                        appViews.window.metroGrid1.Columns[e.ColumnIndex].HeaderText = colname;
                    }
                    col += colname + ",";
                }
                col = col.Substring(0, col.Length - 1);
                sql = "CREATE TABLE '" + appModels.mainPageModel.cacheName + "' (" + col + ")";
                Program.appControlers.dataControler.database1.Execute(sql);

                col = "";
                foreach (DataGridViewColumn column in appViews.window.metroGrid1.Columns)
                {
                    col += "`" + column.HeaderText.Replace(" integer", "").Replace(" text", "").Replace("'", "") + "`,";
                }
                col = col.Substring(0, col.Length - 1);
                sql = "INSERT INTO " + appModels.mainPageModel.cacheName + " (" + col + ") SELECT " + col + " FROM " + appModels.mainPageModel.cacheName + "_old";
                Program.appControlers.dataControler.database1.Execute(sql);

                sql = "DROP table " + appModels.mainPageModel.cacheName + "_old";
                Program.appControlers.dataControler.database1.Execute(sql);
            }catch(Exception ex)
            {
                sql = "DROP table " + appModels.mainPageModel.cacheName;
                Program.appControlers.dataControler.database1.Execute(sql);

                sql = "ALTER TABLE '" + appModels.mainPageModel.cacheName + "_old' RENAME TO '" + appModels.mainPageModel.cacheName+"'" ;
                Program.appControlers.dataControler.database1.Execute(sql);
            }
        }

        private void onSetString(DataGridViewCellMouseEventArgs e)
        {
            string sql = "ALTER TABLE '" + appModels.mainPageModel.cacheName + "' RENAME TO '" + appModels.mainPageModel.cacheName + "_old'";
            Program.appControlers.dataControler.database1.Execute(sql);
            try
            {
                string col = "";
                foreach (DataGridViewColumn column in appViews.window.metroGrid1.Columns)
                {
                    string colname = column.HeaderText;
                    if (appViews.window.metroGrid1.Columns[e.ColumnIndex].HeaderText == colname)
                    {
                        colname = appViews.window.metroGrid1.Columns[e.ColumnIndex].HeaderText.Replace("integer", "text");
                        appViews.window.metroGrid1.Columns[e.ColumnIndex].HeaderText = colname;
                    }
                    col += colname + ",";
                }
                col = col.Substring(0, col.Length - 1);
                sql = "CREATE TABLE '" + appModels.mainPageModel.cacheName + "' (" + col + ")";
                Program.appControlers.dataControler.database1.Execute(sql);

                col = "";
                foreach (DataGridViewColumn column in appViews.window.metroGrid1.Columns)
                {
                    string colname = column.HeaderText;
                    colname ="`" +colname.Replace(" integer", "").Replace(" text", "").Replace("'", "") + "`,";                  
                    col += colname;
                }
                col = col.Substring(0, col.Length - 1);
                sql = "INSERT INTO " + appModels.mainPageModel.cacheName + " (" + col + ") SELECT " + col + " FROM " + appModels.mainPageModel.cacheName + "_old";
                Program.appControlers.dataControler.database1.Execute(sql);

                sql = "DROP table " + appModels.mainPageModel.cacheName + "_old";
                Program.appControlers.dataControler.database1.Execute(sql);
            }
            catch (Exception ex)
            {
                sql = "DROP table " + appModels.mainPageModel.cacheName;
                Program.appControlers.dataControler.database1.Execute(sql);

                sql = "ALTER TABLE '" + appModels.mainPageModel.cacheName + "_old' RENAME TO '" + appModels.mainPageModel.cacheName + "'";
                Program.appControlers.dataControler.database1.Execute(sql);
            }
        }

        public void onTextWindowChanged(object sender, EventArgs e)
        {
            appViews.window.metroPanel2.VerticalScroll.Value = appViews.window.metroPanel2.VerticalScroll.Maximum;
            appViews.window.metroPanel2.Update();
        }

        public void onDataSourceChanged(object sender, EventArgs e)
        {
            appModels.mainPageModel.onDataSourceChanged();
        }

        public void onCellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (appViews.window.metroGrid1.DataSource != null)
            {
                string result = appViews.window.metroGrid1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();
                DataTable dt = (DataTable)appViews.window.metroGrid1.DataSource;
                if (result != "")
                {                 
                    string d_tid = dt.Rows[e.RowIndex][0].ToString();
                    string colName = dt.Columns[e.ColumnIndex].ColumnName;
                    Console.WriteLine(result);
                    database1.Execute("UPDATE " + appModels.mainPageModel.cacheName + " SET `" + colName + "`='" + result + "' WHERE d_tid='" + d_tid + "'");
                }
                else
                {
                    appViews.window.metroGrid1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = dt.Rows[e.RowIndex][e.ColumnIndex].ToString();
                }
            }          
        }

        public void onListBoxDoubleClick(object sender, MouseEventArgs e)
        {
            ListBox box = (ListBox)sender;
            if (box.SelectedItem != null) 
            {
                appModels.mainPageModel.removeColumnName((string)box.SelectedItem);
            }
         
        }

        public void DoDataClean()
        { 
            //PythonRunTime.Instance.Execute()
        }



        public void onClickReanmeNode(TreeNodeMouseClickEventArgs e) 
        {
            if (e.Node.Name != "1") 
            {
                e.Node.BeginEdit();
            }
          
        }

        public void onClickDelNode(TreeNodeMouseClickEventArgs e)
        {
            int id = 0;
            int.TryParse(e.Node.Name, out id);
            database.Categories.Where(table => table.Id == id).DeleteAsync();
            appViews.window.treeView1.Nodes.Remove(e.Node);
        }


        delegate void showDialog();
        public void onClickAddNode(TreeNodeMouseClickEventArgs e)
        {
            appComponents.inputDialog.showDialog(delegate (string input) { onReturnInputDialog(e,input); });
        }

        public void onReturnInputDialog(TreeNodeMouseClickEventArgs e,string name) 
        {           
            int id = 0;
            int.TryParse(e.Node.Name, out id);
            long max = database.Categories.Max(table => table.Id);
            max = max +1;
            Category result = database.Categories.Where(table => table.Id == id).First<Category>();
            Category category = new Category();
            category.Id = max;
            category.Name = name;
            category.Isdata = 0;           
            if (result.ParentId == 0)
            {
                appViews.window.treeView1.Nodes[0].Nodes.Add(max.ToString(), name,(int)SystemImages.FOLDER);               
                category.ParentId = 1;
            }
            else 
            {
                category.ParentId = id;
                e.Node.Nodes.Add(max.ToString(), name, (int)SystemImages.FOLDER);
            }
            database.InsertAsync<Category>(category);
        }

        public void showInput() { 
        }
    }
}
