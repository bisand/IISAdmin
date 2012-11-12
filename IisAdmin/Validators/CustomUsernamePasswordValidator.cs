using System;
using System.IdentityModel.Selectors;
using System.IdentityModel.Tokens;
using System.ServiceModel;

namespace IisAdmin.Validators
{
    public class CustomUsernamePasswordValidator : UserNamePasswordValidator
    {
        public override void Validate(string userName, string password)
        {
            if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password))
            {
                Console.WriteLine("Username and password required");
                throw new SecurityTokenException("Username and password required");
            }

            if (userName != "admin" || password != "password")
            {
                Console.WriteLine("Wrong username ({0}) or password ", userName);
                throw new FaultException(string.Format("Wrong username ({0}) or password ", userName));
            }
        }
    }
}