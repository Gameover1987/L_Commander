using L_Commander.App.Infrastructure;
using L_Commander.Common.Extensions;
using L_Commander.UI.ViewModels;
using System;
using System.Windows.Media;

namespace L_Commander.App.ViewModels.Settings
{
    public class TagViewModel : ViewModelBase
    {
        private string _name;        
        private Color _color;

        public TagViewModel(Tag tag)
        {
            _name = tag.Text;
            _color = tag.Color.ToColor();
        }

        public string Name
        {
            get { return _name; }
            set
            {
                if (_name == value)
                    return;
                _name = value;
                OnPropertyChanged(() => Name);
            }
        }

        public Color Color
        {
            get { return _color; }
            set
            {
                _color = value;                
                OnPropertyChanged(() => Color);
            }
        }
    }
}
