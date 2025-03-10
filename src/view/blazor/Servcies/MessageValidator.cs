using FluentValidation;

namespace LendingView.Servcies
{
    public class MessageValidator<T> : AbstractValidator<T>
    {
        public MessageValidator(Action<IRuleBuilderInitial<T,T>> rule)
        {
            rule(RuleFor(x => x));
        }

        private IEnumerable<string> ValidateValue(T arg)
        {
            var result = Validate(arg);
            if (result.IsValid)
                return new string[0];
            return result.Errors.Select(e => e.ErrorMessage);
        }

        public Func<T, IEnumerable<string>> Validation => ValidateValue;
    }
}
