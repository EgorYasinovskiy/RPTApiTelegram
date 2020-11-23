namespace RPTApi.Telegram
{
    public class Worker : Interfaces.IBotWorker
    {
        public void DeleteOrder(string barcode)
        {
            //TODO: Impliment a method to delete Order from user's orders list.
        }

        public void GetOrder(string barcode, int userId)
        {
            //TODO: Impliment a method to send info about 
        }

        public void RegisterNewUser()
        {
            //TODO: Impliment a method to add new user to DataBase;
        }
    }
}