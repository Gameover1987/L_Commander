using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using L_Commander.App.Infrastructure;
using L_Commander.Common.Extensions;
using L_Commander.UI.Commands;
using L_Commander.UI.ViewModels;

namespace L_Commander.App.ViewModels.Settings
{
    public interface IAddTagViewModel
    {
        void Initialize(Tag tag);

        Tag GetTag();
    }

    public class AddTagViewModel : ViewModelBase, IAddTagViewModel
    {
        private string _text;
        private Color _color;

        public AddTagViewModel()
        {
            OkCommand = new DelegateCommand(x => { },x => CanOkCommandHandler());
        }

        public string Title { get; private set; }

        public string Text
        {
            get { return _text; }
            set
            {
                if (_text == value)
                    return;
                _text = value;
                OnPropertyChanged(() => Text);
            }
        }

        public Color Color
        {
            get { return _color; }
            set
            {
                if (_color == value)
                    return;

                _color = value;
                OnPropertyChanged(() => Color);
            }
        }

        public IDelegateCommand OkCommand { get; }

        public void Initialize(Tag tag)
        {
            if (tag == null)
            {
                Title = "Add tag";
                Text = string.Empty;
                return;
            }

            Title = "Edit existed tag";
            Text = tag.Text;
            Color = tag.Color.ToColor();

        }

        public Tag GetTag()
        {
            return new Tag
            {
                Text = Text,
                Color = Color.ToInt()
            };
        }

        private bool CanOkCommandHandler()
        {
            return !Text.IsNullOrWhiteSpace();
        }
    }
}
