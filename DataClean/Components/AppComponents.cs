using DataClean.Components.Dialog;

namespace DataClean.Components
{ 
    public class AppComponents
    {
        public InputDialog inputDialog;
        public ProcessDialog processDialog;
        public AppComponents() 
        {        
         
        }

        public void init()
        {
            inputDialog = new InputDialog();
            processDialog = new ProcessDialog();
        }

    }
}
