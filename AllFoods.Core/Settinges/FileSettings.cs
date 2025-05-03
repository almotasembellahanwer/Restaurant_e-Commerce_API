namespace AllFoods.Core.Settinges
{
    public static class FileSettings
    {
        public const string ImagesPath = "\\assets\\images";
        public const string AllowedExtinsions = ".jpg,.jpeg,.png";
        public const int MaxFileSizeInMB = 1;
        public const int MaxFileSizeInBytes = MaxFileSizeInMB * 1024 * 1024;
    }
}
