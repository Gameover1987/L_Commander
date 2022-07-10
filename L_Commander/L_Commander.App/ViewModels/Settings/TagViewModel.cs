using L_Commander.App.Infrastructure;
using L_Commander.Common.Extensions;
using L_Commander.UI.ViewModels;
using System;
using System.Windows.Media;

namespace L_Commander.App.ViewModels.Settings
{
    public class TagViewModel : ViewModelBase
    {
        private Tag _tag;

        public TagViewModel(Tag tag)
        {
            _tag = tag;
        }

        public string Name
        {
            get { return _tag.Text; }
            set
            {
                if (_tag.Text == value)
                    return;
                _tag.Text = value;
                OnPropertyChanged(() => Name);
            }
        }

        public Color Color
        {
            get { return _tag.Color.ToColor(); }
            set
            {
                _tag.Color = value.ToInt();                
                OnPropertyChanged(() => Color);
            }
        }

        public Tag GetTag()
        {
            return _tag;
        }

        public void Update(Tag tag)
        {
            _tag = tag;

            OnPropertyChanged();
        }
    }
}
