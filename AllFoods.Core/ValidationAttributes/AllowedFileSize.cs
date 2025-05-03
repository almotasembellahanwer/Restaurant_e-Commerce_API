using AllFoods.Core.Settinges;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllFoods.Core.ValidationAttributes
{
    public class AllowedFileSize : ValidationAttribute
    {
        private readonly int _allowedFileSize;

        public AllowedFileSize(int allowedFileSize)
        {
            _allowedFileSize = allowedFileSize;
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is null)
            {
                return ValidationResult.Success;
            }
            if(value is IFormFile file)
            {
                var fileSize = file.Length;
                var maxFileSizeInBytes = _allowedFileSize;
                var maxFileSizeInMB = FileSettings.MaxFileSizeInMB;
                if (fileSize > maxFileSizeInBytes)
                {
                    return new ValidationResult($"File size should be less than {maxFileSizeInMB}");
                }
                return ValidationResult.Success;
            }
            return new ValidationResult("Invalid file type");
        }
    }
}
