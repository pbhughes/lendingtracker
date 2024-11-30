using LendingTrackerApi.Models;

namespace LendingTrackerApi.Services
{
    public interface IValidationServices
    {
        ValidationMessage ValidateUser(User user);
        ValidationMessage ValidateBorrower(Borrower borrower);
        ValidationMessage ValidateTransaction(Transaction schedule);
        ValidationMessage ValidateItem(Item item);
    }
}