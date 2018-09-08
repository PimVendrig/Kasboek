using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.ComponentModel.DataAnnotations;

namespace Kasboek.WebApp.Utils
{

    /// <summary>
    /// Based on http://www.macaalay.com/2014/02/25/unobtrusive-client-and-server-side-not-equal-to-validation-in-mvc-using-custom-data-annotations/
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class UnlikeAttribute : ValidationAttribute, IClientModelValidator
    {
        private const string DefaultErrorMessage = "The {0} field may not be equal to the {1} field.";

        public string OtherProperty { get; private set; }
        public string OtherPropertyName { get; private set; }

        public UnlikeAttribute(string otherProperty, string otherPropertyName) : base(DefaultErrorMessage)
        {
            OtherProperty = otherProperty;
            OtherPropertyName = otherPropertyName;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value != null)
            {
                var otherProperty = validationContext.ObjectInstance.GetType().GetProperty(OtherProperty);

                var otherPropertyValue = otherProperty.GetValue(validationContext.ObjectInstance, null);

                if (value.Equals(otherPropertyValue))
                {
                    return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
                }
            }

            return ValidationResult.Success;
        }

        public override string FormatErrorMessage(string name)
        {
            return string.Format(ErrorMessageString, name, OtherPropertyName);
        }

        public void AddValidation(ClientModelValidationContext context)
        {
            context.Attributes["data-val"] = "true";
            context.Attributes["data-val-unlike"] = FormatErrorMessage(context.ModelMetadata.GetDisplayName());
            context.Attributes["data-val-unlike-otherproperty"] = OtherProperty;
            context.Attributes["data-val-unlike-otherpropertyname"] = OtherPropertyName;
        }

    }
}
