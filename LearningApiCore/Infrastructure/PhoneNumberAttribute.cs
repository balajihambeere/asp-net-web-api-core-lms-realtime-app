namespace LearningApiCore.Infrastructure
{
    using System.ComponentModel.DataAnnotations;
    using System.Text.RegularExpressions;
    public class PhoneNumberAttribute : ValidationAttribute //implement
    {
        public override bool IsValid(object value)
        {
            var phone = value as string;
            if (IsValidPhoneNumber(phone))
            {
                //valid phone
                return true;
            }
            // Not valid phone
            return false;
        }

        private bool IsValidPhoneNumber(string phoneNumberToValidate)
        {
            if (!string.IsNullOrWhiteSpace(phoneNumberToValidate))
            {
                var regex = new Regex(@"^([0]|\+91)?[6789]\d{9}$");
                return regex.IsMatch(phoneNumberToValidate);
            }
            return false;
        }
    }
}
