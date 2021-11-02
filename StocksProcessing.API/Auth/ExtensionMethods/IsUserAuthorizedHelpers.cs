using StocksProccesing.Relational.Model;

namespace StocksProcessing.API.Auth.ExtensionMethods
{
    public static class IsUserAuthorizedHelpers
    {
        public static bool UserOwnsTheTransaction(this ApplicationUser user, StocksTransaction transaction)
        {
            return !(user is null || transaction is null || transaction.ApplicationUserId != user.Id);
        }

    }
}
