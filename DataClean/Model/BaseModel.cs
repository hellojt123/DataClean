using DataClean.Controlers;
using DataClean.Views;
using DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataClean.Model
{
    public class BaseModel : IBaseModel
    {

        public AppViews appViews {
            get { return Program.appViews; }
        }
        public AppControlers appControlers {
            get { return Program.appControlers; }
        }
        public DatabaseDB database
        {
            get { return appControlers.dataControler.database; }
        }
        public Database1DB database1
        {
            get { return appControlers.dataControler.database1; }
        }

        public virtual void initComponent()
        {
          
        }
    }
}

public enum SystemImages 
{ 
    FOLDER=0,
    FOLDER_OPEN,
    FILE,
}