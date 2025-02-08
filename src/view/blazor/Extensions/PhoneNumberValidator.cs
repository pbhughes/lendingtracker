using PhoneNumbers;
using System.ComponentModel.DataAnnotations;
using System;

namespace LendingView.Extensions
{




    public class ValidInternationalPhoneAttribute : ValidationAttribute

    {

        public override bool IsValid(object value)

        {

            if (value == null) return false;



            var phoneNumberString = value.ToString();

            var phoneUtil = PhoneNumberUtil.GetInstance();



            try

            {

                var phoneNumber = phoneUtil.Parse(phoneNumberString, ""); // Assuming no region is required

                return phoneUtil.IsValidNumber(phoneNumber);

            }

            catch 

            {

                return false;

            }

        }

    }

}
