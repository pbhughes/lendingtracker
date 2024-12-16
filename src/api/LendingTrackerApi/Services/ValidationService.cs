using System.ComponentModel.DataAnnotations;
using LendingTrackerApi.Models;

namespace LendingTrackerApi.Services
{
    public class ValidatorService : IValidationServices
    {
        public ValidatorService()
        {

        }
        public ValidationMessage ValidateBorrower(Borrower borrower)
        {
            if (borrower.UserId == Guid.Empty)
            {
                return new ValidationMessage
                {
                    Valid = false,
                    ErrorMessage = "User ID must be the assigned subject identidier from the authorization system"
                };
            }
            return ValidateFromAnnotations(borrower);
        }

        public ValidationMessage ValidateItem(Item item)
        {
            return ValidateFromAnnotations(item);
        }

        public ValidationMessage ValidateTransaction(Transaction transaction)
        {
            return ValidateFromAnnotations(transaction);
        }

        public ValidationMessage ValidateUser(User user)
        {
            if (user.UserId == Guid.Empty)
            {
                return new ValidationMessage
                {
                    Valid = false,
                    ErrorMessage = "User ID must be the assigned subject identidier from the authorization system"
                };
            }

            return ValidateFromAnnotations(user);

            
        }

        private static ValidationMessage ValidateFromAnnotations<T>(T model)
        {
            List<ValidationResult> results = new List<ValidationResult>();
            if (!Validator.TryValidateObject(model, new ValidationContext(model), results, true))
            {

                if (results.Count > 0)
                {
                    return new ValidationMessage
                    {
                        Valid = false,
                        ErrorMessage = results[0]?.ErrorMessage ?? "Unknown error"
                    };
                }
            }

            return new ValidationMessage
            {
                Valid = true,
                ErrorMessage = ""
            };
        }
    }
}