using System;
using System.ComponentModel.DataAnnotations;

namespace Domain.Validators;
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public class MustBeValidUrlAttribute : ValidationAttribute
    {
        private readonly bool _requireHttps;
        private readonly UriKind _uriKind;

        /// <summary>
        /// Validates if the input string is a well-formed URI.
        /// </summary>
        /// <param name="requireHttps">If true, only accepts HTTPS scheme.</param>
        /// <param name="uriKind">Specifies whether the URI must be absolute, relative, or either.</param>
        public MustBeValidUrlAttribute(bool requireHttps = false, UriKind uriKind = UriKind.Absolute)
        {
            _requireHttps = requireHttps;
            _uriKind = uriKind; // Usually Absolute for external URLs

            // Set a default error message, can be overridden
            ErrorMessage = "The {0} field must be a valid and well-formed URL.";
            if (_requireHttps)
            {
                ErrorMessage += " HTTPS scheme is required.";
            }
        }

        public override bool IsValid(object? value)
        {
            // Null or empty is handled by [Required] if present
            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
            {
                return true; // Don't validate if not required and empty
            }

            if (value is not string urlString)
            {
                return false; // Not a string
            }

            // The core validation using Uri.TryCreate
            bool isValidUri = Uri.TryCreate(urlString, _uriKind, out Uri? uriResult);

            if (!isValidUri || uriResult == null)
            {
                return false;
            }

            // Additional check for scheme if required
            if (_requireHttps && uriResult.Scheme != Uri.UriSchemeHttps)
            {
                 // Update ErrorMessage with specific reason (optional, better feedback)
                 ErrorMessage = $"The {{0}} field must use the HTTPS scheme. Found '{uriResult.Scheme}'.";
                 return false;
            }

            // You could add more checks here if needed (e.g., check Host, Port, etc.)

            return true;
        }

         // Optional: Override FormatErrorMessage for better message composition
         public override string FormatErrorMessage(string name)
         {
             return string.Format(ErrorMessageString, name); // Uses ErrorMessage or ErrorMessageString
         }
    }
