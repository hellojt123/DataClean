using DataClean.Properties;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataClean.Views
{
    public class BaseView
    {
        public Image getRes(string name)
        {
           return (Bitmap)Resources.ResourceManager.GetObject(name);
        }
    }
}
