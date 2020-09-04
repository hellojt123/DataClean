using DataClean.Properties;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DataClean.Views.Page
{
    public class MainPageView: BaseView
    {

        public delegate void updateDelegate();
        public MainPageView()
        {
            Program.appViews.window.Shown += onWindowShown;
                
        }

        private void onWindowShown(object sender, EventArgs e)
        {
            Program.appViews.window.Invoke(new updateDelegate(() => {
                updateMainPage();
            }));
        }

        public void updateMainPage()
        {
            Program.appViews.window.metroButton1.Text = "添加";
            Program.appViews.window.metroButton2.Text = "导出";
            Program.appViews.window.metroButton3.Text = "首页";
            Program.appViews.window.metroButton4.Text = "上一页";
            Program.appViews.window.metroButton5.Text = "下一页";
            Program.appViews.window.metroButton6.Text = "尾页";
            Program.appViews.window.metroButton7.Text = "开始清洗";
            Program.appViews.window.metroButton8.Text = "任务";

            Program.appViews.window.metroCheckBox1.Text = "删除重复";
            Program.appViews.window.metroCheckBox2.Text = "删除空数据";
            Program.appViews.window.metroCheckBox3.Text = "智能填充";

            Program.appViews.window.metroButton1.Click += Program.appControlers.mainPageControler.onClickAdd;
            Program.appViews.window.metroButton2.Click += Program.appControlers.mainPageControler.onClickExport;
            Program.appViews.window.metroButton3.Click += Program.appControlers.mainPageControler.OnClickFirst;
            Program.appViews.window.metroButton4.Click += Program.appControlers.mainPageControler.OnClickPrevious;
            Program.appViews.window.metroButton5.Click += Program.appControlers.mainPageControler.OnClickNext;
            Program.appViews.window.metroButton6.Click += Program.appControlers.mainPageControler.OnClickLast;
            Program.appViews.window.metroButton7.Click += Program.appControlers.mainPageControler.onClickDataClean;
            Program.appViews.window.metroButton8.Click += Program.appControlers.mainPageControler.onClickShowProcess;


            Program.appViews.window.treeView1.NodeMouseClick += Program.appControlers.mainPageControler.onClickTreeView;
            Program.appViews.window.treeView1.AfterLabelEdit += Program.appControlers.mainPageControler.AfterTreeNodeEdited;
            Program.appViews.window.treeView1.AfterExpand += Program.appControlers.mainPageControler.AfterTreeNodeExpand;
            Program.appViews.window.treeView1.AfterCollapse += Program.appControlers.mainPageControler.AfterTreeNodeCollapse;
            Program.appViews.window.treeView1.ImageList = new ImageList();
            Program.appViews.window.treeView1.ImageList.Images.Add(getRes("folder"));
            Program.appViews.window.treeView1.ImageList.Images.Add(getRes("folder_open"));
            Program.appViews.window.treeView1.ImageList.Images.Add(getRes("file_text_fill"));
            Program.appViews.window.metroPanel1.Visible = true;
            Program.appViews.window.metroLabel1.Text = "";
            Program.appViews.window.metroLabel3.Text = "";
            Program.appViews.window.metroGrid1.CellEndEdit += Program.appControlers.mainPageControler.onCellEndEdit;
            Program.appViews.window.metroGrid1.AllowUserToOrderColumns = false;
            Program.appViews.window.metroGrid1.ColumnHeaderMouseClick += Program.appControlers.mainPageControler.GridViewHeadClick;
            Program.appViews.window.listBox1.MouseDoubleClick += Program.appControlers.mainPageControler.onListBoxDoubleClick;
            Program.appViews.window.metroLabel3.SizeChanged += Program.appControlers.mainPageControler.onTextWindowChanged;
            Program.appViews.window.metroGrid1.DataSourceChanged += Program.appControlers.mainPageControler.onDataSourceChanged;
            //Program.appViews.window.metroGrid1.AutoGenerateColumns = true;
            //Program.appViews.window.metroGrid1.AutoGenerateColumnsChanged += Program.appControlers.mainPageControler.onDataSourceChanged;
            //Program.appViews.window.metroListView1.


        }

    }
}
