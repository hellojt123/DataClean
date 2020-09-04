using DataClean.Model.Page;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataClean.Model
{
    public class Models
    {
        public  MainPageModel mainPageModel;

        public Models() 
        {
          
        }

        public void init()
        {
            mainPageModel = new MainPageModel();
        }      
    }
}
