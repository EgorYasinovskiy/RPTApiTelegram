using System;
using System.Collections.Generic;
using System.Text;

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
        public void RegisterNewUser();
        /// <summary>
        /// Gets info about order and sends it to user. If order is new -> adding order and records to database.
        /// </summary>
        /// <param name="barcode"></param>
        /// <param name="userId"></param>
        public void GetOrder(string barcode, int userId);
        /// <summary>
        /// Remove order from user orders list (and database optionaly).
        /// </summary>
        /// <param name="barcode"></param>
        public void DeleteOrder(string barcode);

    }
}
