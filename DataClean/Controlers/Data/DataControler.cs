using DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataClean.Controlers.Data
{

    public class DataControler
    {
        private DatabaseDB db;
        private Database1DB db1;
        public DataControler()
        {
#if (!DEBUG)
            db = new DatabaseDB("R_Database");
            db1 = new Database1DB("R_Database1");            
#else
            db = new DatabaseDB("Database");
            db1 = new Database1DB("Database1");
#endif
        }

        public DatabaseDB database {
            get {
                return db;
            }
        }
        public Database1DB database1
        {
            get
            {
                return db1;
            }
        }
    }
}
