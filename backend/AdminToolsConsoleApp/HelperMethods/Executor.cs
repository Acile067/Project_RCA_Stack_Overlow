using AdminToolsConsoleApp.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
            Console.WriteLine("\nEmail adrese za upozorenja:");
            foreach (var email in emails)
            {
                Console.WriteLine($"- {email}");
            }
        }

        public async Task AddAsync()
        {
            Console.Write("Unesi novu email adresu: ");
            string email = Console.ReadLine();
            await repository.AddEmailAsync(email);
            Console.WriteLine("Email dodat.");
        }

        public async Task DeleteAsync()
        {
            Console.Write("Unesi email koji zelis obrisati: ");
            string email = Console.ReadLine();

            bool deleted = await repository.DeleteEmailAsync(email);
            Console.WriteLine(deleted ? "Email obrisan." : "Email nije pronadjen.");
        }
    }
}
