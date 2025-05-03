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
    public class AllowedExtensions : ValidationAttribute
    {
        private readonly string _allowedExtensions;
        public AllowedExtensions(string allowedExtensions)
        {
            _allowedExtensions = allowedExtensions;
        }
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if(value is null)
            {
                return ValidationResult.Success;
            }
            if(value is IFormFile file)
            {
                var extension = Path.GetExtension(file.FileName);
                if (string.IsNullOrEmpty(extension))
                {
                    return new ValidationResult("file has no extension");
                }
                var isValid = _allowedExtensions.Split(",").Contains(extension, StringComparer.OrdinalIgnoreCase);
                if (!isValid)
                {
                    return new ValidationResult($"Only '{FileSettings.AllowedExtinsions}' is allowed");
                }
                return ValidationResult.Success;
            }

            return new ValidationResult("Invalid file type");
            
        }
    }
}
