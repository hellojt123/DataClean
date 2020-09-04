using DataClean.Components;
using DataClean.Model;
using DataClean.Views;
using DataModels;

namespace DataClean.Controlers
{
    public class BaseControler
    {
        public AppViews appViews
        {
            get { return Program.appViews; }
        }
        public AppControlers appControlers
        {

            get { return Program.appControlers; }
        }

        public DatabaseDB database {
            get { return appControlers.dataControler.database; }
        }

        public Database1DB database1
        {
            get { return appControlers.dataControler.database1; }
        }

        public AppComponents appComponents 
        {
            get { return Program.appComponents; }
        }

        public Models appModels
        {
            get { return Program.models; }
        }
    }
}
