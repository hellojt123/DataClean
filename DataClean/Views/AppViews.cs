using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DataClean.Views
{
    public class AppViews
    {
        public Form1 window;
        public Page.MainPageView mainPageView;
        public AppViews(Form1 form) 
        {
            window = form;
            
        }


        public void init()
        {
            Thread thread = new Thread(new ThreadStart(initViews));//创建线程
            thread.Start();
            //initViews();
        }
        public void initViews() 
        {         
            mainPageView = new Page.MainPageView();          
        }
    }
}
