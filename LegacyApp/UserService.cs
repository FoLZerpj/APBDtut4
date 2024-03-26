using System;

namespace LegacyApp
{
    public class UserService
    {
        private (bool, int) getUserCreditLimit(string lastName, DateTime dateOfBirth, string clientType)
        {
            if (clientType == "VeryImportantClient")
            {
                return (false, -1);
            }
            else if (clientType == "ImportantClient")
            {
                using (var userCreditService = new UserCreditService())
                {
                    int creditLimit = userCreditService.GetCreditLimit(lastName, dateOfBirth);
                    return (true, creditLimit * 2);
                }
            }
            else
            {
                using (var userCreditService = new UserCreditService())
                {
                    int creditLimit = userCreditService.GetCreditLimit(lastName, dateOfBirth);
                    return (true, creditLimit);
                }
            }
        }
        
        public bool AddUser(string firstName, string lastName, string email, DateTime dateOfBirth, int clientId)
        {
            if (string.IsNullOrEmpty(firstName) || string.IsNullOrEmpty(lastName))
            {
                return false;
            }

            if (!UserValidator.CheckUserEmail(email))
            {
                return false;
            }

            if (!UserValidator.CheckUserAge(dateOfBirth))
            {
                return false;
            }

            var clientRepository = new ClientRepository();
            var client = clientRepository.GetById(clientId);

            var user = new User
            {
                Client = client,
                DateOfBirth = dateOfBirth,
                EmailAddress = email,
                FirstName = firstName,
                LastName = lastName
            };

            (user.HasCreditLimit, user.CreditLimit) = getUserCreditLimit(lastName, dateOfBirth, client.Type);

            if (user.HasCreditLimit && user.CreditLimit < 500)
            {
                return false;
            }

            UserDataAccess.AddUser(user);
            return true;
        }
    }
}
