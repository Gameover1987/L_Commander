using System.Collections;
using System.ComponentModel;
using System.Linq.Expressions;
using L_Commander.Common.Extensions;
using L_Commander.UI.ViewModels;

namespace L_Commander.UI.Validation
{
    public abstract class ValidationSupportableViewModel : ViewModelBase, IValidationSupportableViewModel
    {        
        private readonly List<PropertyValidation> _validations = new List<PropertyValidation>(); 

        private Dictionary<string, List<string>> _errorMessages = new Dictionary<string, List<string>>();

        protected ValidationSupportableViewModel()
        {
            
        }

        public bool HasErrors
        {
            get { return _errorMessages.SelectMany(x => x.Value).Any(); }
        }

        public IEnumerable GetErrors(string propertyName)
        {
            if (propertyName == null)
                return Enumerable.Empty<string>();

            if (_errorMessages.ContainsKey(propertyName))
                return _errorMessages[propertyName];

            return Enumerable.Empty<string>();
        }

        public string[] GetErrors<T>(Expression<Func<T>> propertyExpression)
        {
            var propertyName = GetPropertyName(propertyExpression);
            var errors = GetErrors(propertyName).Cast<string>().ToArray();
            return errors;
        }

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        /// <summary>
        /// Запускает валидацию всех свойств определенных через конструкцию AddValidationFor
        /// </summary>
        public void Validate()
        {
            var propertyNamesWithValidationErrors = _errorMessages.Keys;

            _errorMessages = new Dictionary<string, List<string>>();

            _validations.ForEach(PerformValidation);

            var propertyNamesThatMightHaveChangedValidation = _errorMessages.Keys.Union(propertyNamesWithValidationErrors).ToList();

            propertyNamesThatMightHaveChangedValidation.ForEach(OnErrorsChanged);

            OnPropertyChanged(() => HasErrors);
        }

        protected void ResetValidation()
        {
            _errorMessages.Clear();
            OnPropertyChanged(() => HasErrors);
        }

        protected void ValidateProperty<T>(Expression<Func<T>> propertyExpression)
        {
            var propertyName = GetPropertyName(propertyExpression);
            ValidateProperty(propertyName);
        }

        protected void ValidateProperty(string propertyName)
        {
            _errorMessages.Remove(propertyName);

            _validations.Where(v => v.PropertyName == propertyName).ForEach(PerformValidation);

            OnErrorsChanged(propertyName);
            OnPropertyChanged(() => HasErrors);
        }

        protected override void OnPropertyChanged(string propertyName)
        {
            base.OnPropertyChanged(propertyName);

            if (_validations.Any(x => x.PropertyName == propertyName))
            {
                ValidateProperty(propertyName);
            }
        }

        protected PropertyValidation AddValidationFor<T>(Expression<Func<T>> propertyExpression)
        {
            var propertyName = GetPropertyName(propertyExpression);
            var validation = new PropertyValidation(propertyName);
            _validations.Add(validation);

            return validation;
        }

        private void PerformValidation(PropertyValidation validation)
        {
            string errorMessage;

            if (!validation.IsValid(out errorMessage))
            {
                if (errorMessage.IsNullOrWhiteSpace())
                {
                    throw new Exception();
                }

                AddErrorMessageForProperty(validation.PropertyName, errorMessage);
            }
        }

        private void AddErrorMessageForProperty(string propertyName, string errorMessage)
        {
            if (_errorMessages.ContainsKey(propertyName))
            {
                _errorMessages[propertyName].Add(errorMessage);
            }
            else
            {
                _errorMessages.Add(propertyName, new List<string> { errorMessage });
            }
        }

        private void OnErrorsChanged(string propertyName)
        {
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
        }
    }
}
