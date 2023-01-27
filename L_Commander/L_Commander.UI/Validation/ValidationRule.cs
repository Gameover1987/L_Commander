namespace L_Commander.UI.Validation
{
    internal class ValidationRule
    {
        private readonly Func<bool> _validationCriteria;
        private readonly Func<string> _getErrorMessageFunc;

        public ValidationRule(Func<bool> validationCriteria, Func<string> getErrorMessageFunc)
        {
            _validationCriteria = validationCriteria;
            _getErrorMessageFunc = getErrorMessageFunc;
        }

        public bool IsValid => !_validationCriteria();

        public string ErrorMessage => _getErrorMessageFunc();
    }
}