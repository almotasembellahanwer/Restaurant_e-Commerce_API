using AllFoods.Core.Settinges;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllFoods.Core.Helpers
{
    public static class DeleteCoverHelper
    {
        private static readonly string _imagePath;
        static DeleteCoverHelper()
        {

            _imagePath = $"wwwroot{FileSettings.ImagesPath}";
        }
        public static void DeleteFile(string fileName, string folderName)
        {
            string cover = Path.Combine(Directory.GetCurrentDirectory(),_imagePath,folderName, fileName);
            if (File.Exists(cover))
            {
                File.Delete(cover);
            }

        }
    }
}
