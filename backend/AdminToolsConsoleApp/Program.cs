using AdminToolsConsoleApp.HelperMethods;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminToolsConsoleApp
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var executor = new Executor();
            bool exit = false;

            while (!exit)
            {
                Console.WriteLine("\n=== Admin Tools Console ===");
                Console.WriteLine("1. Show all email addresses");
                Console.WriteLine("2. Add new email address");
                Console.WriteLine("3. Delete email address");
                Console.WriteLine("4. Exit");

                Console.Write("\nChoose an option: ");
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        await executor.ShowAllAsync();
                        break;
                    case "2":
                        await executor.AddAsync();
                        break;
                    case "3":
                        await executor.DeleteAsync();
                        break;
                    case "4":
                        exit = true;
                        break;
                    default:
                        Console.WriteLine("Invalid input.");
                        break;
                }
            }
        }
    }
}
