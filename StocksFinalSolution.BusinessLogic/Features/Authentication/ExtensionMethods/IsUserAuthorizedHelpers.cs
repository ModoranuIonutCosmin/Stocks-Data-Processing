using StocksProccesing.Relational.Model;

namespace StocksFinalSolution.BusinessLogic.Features.Authentication.ExtensionMethods
{
    public static class IsUserAuthorizedHelpers
    {
        public static bool UserOwnsTheTransaction(this ApplicationUser user, StocksTransaction transaction)
        {
            return !(user is null || transaction is null || transaction.ApplicationUserId != user.Id);
        }

    }
}
