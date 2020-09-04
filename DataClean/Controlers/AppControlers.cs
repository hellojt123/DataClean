
namespace DataClean.Controlers
{
    public class AppControlers
    {
        public Data.DataControler dataControler;
        public Page.MainPageControler mainPageControler;

        public AppControlers() 
        {
          
        }

        public void init()
        {
            dataControler = new Data.DataControler();
            mainPageControler = new Page.MainPageControler();
        }
    }
}
