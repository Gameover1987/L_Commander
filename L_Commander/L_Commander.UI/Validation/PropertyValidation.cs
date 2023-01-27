namespace L_Commander.UI.Validation
{
    public class PropertyValidation
    {
        private readonly IList<ValidationRule> _rules = new List<ValidationRule>();

        public PropertyValidation(string propertyName)
        {
            PropertyName = propertyName;
        }

        public string PropertyName { get; }

        public PropertyValidation When(Func<bool> validationCriteria, Func<string> errorMessageFunc)
        {
            var rule = new ValidationRule(validationCriteria, errorMessageFunc);
            _rules.Add(rule);

            return this;
        }

        public bool IsValid(out string errorMessage)
        {
            errorMessage = null;

            var invalidRule = _rules.FirstOrDefault(x => !x.IsValid);
            if (invalidRule != null)
            {
                errorMessage = invalidRule.ErrorMessage;
                return false;
            }

            return true;
        }
    }
}