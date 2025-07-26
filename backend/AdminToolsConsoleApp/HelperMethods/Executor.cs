using AdminToolsConsoleApp.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AdminToolsConsoleApp.HelperMethods
{
    public class Executor
    {
        private readonly AlertEmailRepository repository;

        public Executor()
        {
            repository = new AlertEmailRepository();
        }

        public async Task ShowAllAsync()
        {
            var emails = await repository.GetAllEmailsAsync();
            Console.WriteLine("\nAll email addresses:");
            foreach (var email in emails)
            {
                Console.WriteLine($"- {email}");
            }
        }

        public async Task AddAsync()
        {
            Console.Write("Input email: ");
            string email = Console.ReadLine();
            if (!Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.com$"))
            {
                Console.WriteLine("Invalid email format. Email must be in the form something@something.com");
                return;
            }

            bool exists = await repository.EmailExistsAsync(email);
            if (exists)
            {
                Console.WriteLine("Email already exists.");
                return;
            }

            await repository.AddEmailAsync(email);
            Console.WriteLine("Email added.");
        }

        public async Task DeleteAsync()
        {
            Console.Write("Input email for deletion: ");
            string email = Console.ReadLine();

            if (!Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.com$"))
            {
                Console.WriteLine("Invalid email format. Email must be in the form something@something.com");
                return;
            }


            bool deleted = await repository.DeleteEmailAsync(email);
            Console.WriteLine(deleted ? "Email deleted." : "Email not found.");
        }
    }
}
