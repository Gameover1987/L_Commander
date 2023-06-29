using System.Windows.Media;
using L_Commander.Common.Extensions;

namespace L_Commander.App.OperatingSystem;

public class ApplicationModel
{
    private ImageSource _icon;

    public string DisplayName { get; set; }

    public string Arguments { get; set; }

    public string ExecutePath { get; set; }

    public string Group { get; set; }

    public int Order { get; set; }

    public ImageSource Icon
    {
        get
        {
            if (ExecutePath.IsNullOrWhiteSpace())
                return null;

            if (_icon == null)
                return _icon = BitmapExtensions.ToImageSource(ExecutePath);

            return _icon;
        }
    }
}