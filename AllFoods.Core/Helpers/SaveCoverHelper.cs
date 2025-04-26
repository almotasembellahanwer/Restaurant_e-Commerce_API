using AllFoods.Core.Domain.Entities;
using AllFoods.Core.Settinges;
using Microsoft.AspNetCore.Http;


namespace AllFoods.Core.Helpers
{
    public static class SaveCoverHelper
    {
        private static readonly string _imagePath;
         static SaveCoverHelper()
        {

            _imagePath = $"wwwroot{FileSettings.ImagesPath}";
        }
        public static async Task<string> SaveCover(IFormFile cover,string folderName)
        {
            var folderPath = Path.Combine(Directory.GetCurrentDirectory(), _imagePath, folderName);
            //name of image
            var coverName = $"{Guid.NewGuid()}{Path.GetExtension(cover.FileName)}";
            //path of image

            var filePath = Path.Combine(folderPath, coverName);

            using var stream = File.Create(filePath);
            await cover.CopyToAsync(stream);

            return coverName;
        }

        }
}
