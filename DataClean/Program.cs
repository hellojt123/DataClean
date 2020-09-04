using DataClean.Controlers;
using DataClean.Model;
using DataClean.Views;
using DataClean.Components;
using System;
using System.Windows.Forms;
using System.Reflection;
using System.IO;
using System.Globalization;
using System.Collections.Generic;

namespace DataClean
{
    static class Program
    {
        public static AppControlers appControlers;
        public static Models models;
        public static AppViews appViews;
        public static AppComponents appComponents;

        

        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Form1 form1 = new Form1();
            appViews = new AppViews(form1);
            appControlers = new AppControlers();
            models = new Models();
            appComponents = new AppComponents();

            appControlers.init();
            appViews.init();         
            models.init();
            appComponents.init();

            //AppDomain.CurrentDomain.AssemblyResolve += OnResolveAssembly;
            //Application.EnableVisualStyles();
            //Application.SetCompatibleTextRenderingDefault(false);

            Application.Run(form1);         
        }

        private static Dictionary<string, Assembly> LoadedDlls = new Dictionary<string, Assembly>();
        //解析程序集失败，会加载对应的程序集
        private static Assembly OnResolveAssembly(object sender, ResolveEventArgs args)
        {
            var assName = new AssemblyName(args.Name).FullName;
            try
            {
                //判断是否已经加载
                if (LoadedDlls.ContainsKey(assName))
                {
                    return LoadedDlls[assName];
                }
                string file = Application.StartupPath + "\\libs\\" + assName.Split(',')[0] + ".dll";
                byte[] buff = System.IO.File.ReadAllBytes(file);
                var da = Assembly.Load(buff);
                return da;
            }
            catch (Exception ex)
            {
                throw new DllNotFoundException(assName);//否则抛出加载失败的异常
            }
        }
    }
}
