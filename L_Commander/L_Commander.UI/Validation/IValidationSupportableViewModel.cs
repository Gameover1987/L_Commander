using System.ComponentModel;

namespace L_Commander.UI.Validation
{
    public interface IValidationSupportableViewModel : INotifyDataErrorInfo
    {
        void Validate();
    }
}