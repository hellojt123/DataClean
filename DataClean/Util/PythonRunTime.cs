//using IronPython.Hosting;
//using Microsoft.Scripting.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DataClean.Util
{
    public class PythonRunTime
    {
        private static Dictionary<string,System.Diagnostics.Process> processlist = new Dictionary<string, System.Diagnostics.Process>();
        private static Dictionary<string, StringBuilder> process_str = new Dictionary<string, StringBuilder>();
        //private static ScriptEngine pyengine;
        private static PythonRunTime _Instance;
        //private static ScriptScope pyScope;
        public static  PythonRunTime Instance {
            get {
                if (_Instance == null)
                {
                    //pyengine = Python.CreateEngine();
                    //ICollection<string> Paths = pyengine.GetSearchPaths();
                    //Paths.Add(@"D:\BigData\DataClean\DataClean\Python\Packages");
                    //Paths.Add(@"D:\BigData\DataClean\DataClean\Python\Lib");
                    ////Paths.Add(AppDomain.CurrentDomain.BaseDirectory + "Packages");
                    ////Paths.Add(AppDomain.CurrentDomain.BaseDirectory + "Lib");
                    //pyengine.SetSearchPaths(Paths);
                    ////pyengine.ImportModule("sys");
                    //pyengine.ImportModule("numpy");
                    //pyScope = pyengine.CreateScope();
                    _Instance = new PythonRunTime();

                }
                return _Instance;
            }
        }

        public void Execute(string name,string avgs) 
        {
            Thread thread = new Thread(new ThreadStart(()=>_Execute(name,avgs)));//创建线程
            thread.Start();
        }

        private void _Execute(string name,string avgs)
        {
            if (!processlist.ContainsKey(name) && processlist.Count<5)
            {
#if (!DEBUG)
                string filePath = AppDomain.CurrentDomain.BaseDirectory + "Python/run.py";
                string pyPath = AppDomain.CurrentDomain.BaseDirectory + "Python/python.exe";
#else
                string filePath = "D:/BigData/DataClean/DataClean/Python/run.py ";
                string pyPath = "D:/BigData/DataClean/DataClean/Python/python.exe ";
#endif            
                System.Diagnostics.ProcessStartInfo procStartInfo = new System.Diagnostics.ProcessStartInfo(pyPath, filePath + avgs);
                procStartInfo.RedirectStandardOutput = true;
                procStartInfo.UseShellExecute = false;
                procStartInfo.CreateNoWindow = true;
                //using (System.Diagnostics.Process process = System.Diagnostics.Process.Start(procStartInfo))
                System.Diagnostics.Process process = System.Diagnostics.Process.Start(procStartInfo);
                {          
                    //using (StreamReader reader = process.StandardOutput)
                    {
                        StringBuilder sb = new StringBuilder();
                        processlist.Add(name, process);
                        process_str.Add(name, sb);
                        Program.models.mainPageModel.updateWindowText(processlist.Count.ToString());
                        process.EnableRaisingEvents = true;
                        process.OutputDataReceived += (object sender, System.Diagnostics.DataReceivedEventArgs e) =>
                        {                       
                            onRec(name, e);
                        };
                        process.Exited += (object sender, EventArgs e) => onProcessExit(name);
                        process.BeginOutputReadLine();
                        process.WaitForExit();
                        process.Close();                   
                      
                    }
                }
            }
                
        }

        private void onRec(string name,System.Diagnostics.DataReceivedEventArgs e)
        {
            StringBuilder sb;
            process_str.TryGetValue(name,out sb);
            sb.AppendLine(e.Data);
            if (Program.appComponents.processDialog.Created)
            {
                Program.appComponents.processDialog.UpdateText();
            }            
        }

        private void onProcessExit(string processname)
        {
            processlist.Remove(processname);
            //process_str.Remove(processname);
            Program.models.mainPageModel.updateWindowText(processlist.Count.ToString());
        }

        public Dictionary<string, System.Diagnostics.Process> getProcess() {
            return processlist;
        }

        public Dictionary<string, StringBuilder> getProcessStr()
        {
            return process_str;
        }

    }
}
