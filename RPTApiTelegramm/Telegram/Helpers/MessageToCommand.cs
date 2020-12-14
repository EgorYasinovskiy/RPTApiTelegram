namespace RPTApi.Telegram.Helpers
{
    public static class MessageToCommand
    {
        public static Commands.ICommand GetCommand(string message)
        {
            // Проводим последовательную проверку наличия команды в тексте
            if (message.StartsWith(Commands.RemoveOrder.Name))
                return new Commands.RemoveOrder();
            if (message.StartsWith(Commands.RenameOrder.Name))
                return new Commands.RenameOrder();
            if (message.StartsWith(Commands.TrackOrder.Name))
                return new Commands.TrackOrder();
            if (message.StartsWith(Commands.Help.Name))
                return new Commands.Help();
            return new Commands.Unknown();
        }
    }
}
