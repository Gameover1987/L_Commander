using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Media;
using L_Commander.Common.Extensions;

namespace L_Commander.App.OperatingSystem
{
    public interface IIconCache
    {
        ImageSource GetByPath(string path);
    }

    public sealed class IconCache : IIconCache
    {
        private string[] _stopExtensions = new string[] {".exe"};
        private readonly Dictionary<string, ImageSource> _cache = new Dictionary<string, ImageSource>();

        public ImageSource GetByPath(string path)
        {
            var extension = Path.GetExtension(path).ToLower();

            // Если экстеншен содаржится в списке исключений то не используем кэш, данный подход необходим для корректного отображения .exe файлов
            if (_stopExtensions.Contains(extension))
                return BitmapExtensions.ToImageSource(path);

            lock (_cache)
            {
                if (!_cache.ContainsKey(extension))
                    _cache[extension] = BitmapExtensions.ToImageSource(path);

                return _cache[extension];
            }
        }
    }
}
