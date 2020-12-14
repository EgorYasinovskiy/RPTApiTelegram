using System.Threading.Tasks;

namespace RPTApi.Interfaces
{
    /// <summary>
    /// Interface for bots to add Discord, Vk etc. bots without new architecture.
    /// </summary>
    public interface IBotWorker
    {
        /// <summary>
        /// Add a new user to database. Should be runned at the first message from new user.
        /// </summary>
        public Task RegisterNewUser(int userID);
        /// <summary>
        /// Gets info about order and sends it to user. If order is new -> adding order and records to database.
        /// </summary>
        /// <param name="barcode"></param>
        /// <param name="userId"></param>
        public Task<DataBase.Models.Order> GetOrderAsync(string barcode, int userId);
        /// <summary>
        /// Remove order from user orders list (and database optionaly).
        /// </summary>
        /// <param name="barcode"></param>
        public Task<bool> DeleteOrder(string barcode,int userId);
        public Task<bool> RenameOrder(string barcode, string newName, int userId);



    }
}
