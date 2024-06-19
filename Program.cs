using Leave_Manager.Application;
using Leave_Manager.Leave_Manager.Core.Interfaces;
using Leave_Manager.Leave_Manager.Core.Services;
using Leave_Manager.Leave_Manager.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

var serviceCollection = new ServiceCollection();

// Register Services (DbContext, Repositories, Services)
serviceCollection.AddDbContext<LMDbContext>(options =>
    options.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=Leave_Manager_ConsoleDb;Trusted_Connection=True;"));

// 1.2. Register Dependencies (Make sure this is here, BEFORE building the service provider)
serviceCollection.AddScoped<ILeaveManagementService, LeaveManagementService>(); 
serviceCollection.AddScoped<IListOfEmployeesService, ListOfEmployeesService>();
serviceCollection.AddScoped<IEmployeeRepository, EmployeeRepository>();
serviceCollection.AddScoped<ILeaveRepository, LeaveRepository>();
serviceCollection.AddScoped<Application>();

//  Build ServiceProvider
var serviceProvider = serviceCollection.BuildServiceProvider();

// Resolve and Use the LeaveManagementService
var application = serviceProvider.GetRequiredService<Application>();
//var leaveManagementService = serviceProvider.GetRequiredService<ILeaveManagementService>();
//var listOfEmployeesService = serviceProvider.GetRequiredService<IListOfEmployeesService>();

Console.WriteLine("--- Leave management app ---");

Menus.ShowMainMenu();

var userInput = Console.ReadLine();

//    Application application = new();

while (true)
{
    switch (userInput)
    {
        case "M":
        case "m":
            Menus.ShowMainMenu();
            break;
        case "1":
            await application.AddEmployeeAsync();
            break;
        case "2":
            await application.DisplayAllEmployeesAsync();
            break;
        case "3":
            Console.WriteLine("Insert search phrase");
            var searchPhrase = (Console.ReadLine() ?? ""); //null checker
            await application.DisplayMatchingEmployeesAsync(searchPhrase);
            break;
        case "4":
            Console.Write("Employee - ");
            int intToRemove = AuxiliaryMethods.GetId();
            if (intToRemove != 0)
            {
                await application.RemoveEmployeeAsync(intToRemove);
            }
            break;
        case "5":
            Console.Write("Employee - ");
            int employeeId = AuxiliaryMethods.GetId();
            if (employeeId != 0)
            {
                await application.AddLeaveAsync(employeeId);
            }
            break;
        case "6":
            await application.DisplayAllLeavesAsync();
            break;
        case "6D":
            await application.DisplayAllLeavesOnDemandAsync();
            break;
        case "6E":
            Console.Write("Employee - ");
            employeeId = AuxiliaryMethods.GetId();
            if (employeeId != 0)
            {
                await application.DisplayAllLeavesForEmployeeAsync(employeeId);
            }
            break;
        case "6ED":
        case "6DE":
            Console.Write("Employee - ");
            employeeId = AuxiliaryMethods.GetId();
            if (employeeId != 0)
            {
                await application.DisplayAllLeavesForEmployeeOnDemandAsync(employeeId);
            }
            break;
        case "7":
            Console.Write("Leave - ");
            int intOfLeaveToRemove = AuxiliaryMethods.GetId();
            if (intOfLeaveToRemove != 0)
            {
                await application.RemoveLeaveAsync(intOfLeaveToRemove);
            }
            break;
        case "8":
            Console.Write("Leave - ");
            int intOfLeaveToEdit = AuxiliaryMethods.GetId();
            if (intOfLeaveToEdit != 0)
            {
                await application.EditLeaveAsync(intOfLeaveToEdit);
            }
            break;
        case "9":
            Console.Write("Employee - ");
            int intOfEmployeeToEdit = AuxiliaryMethods.GetId();
            if (intOfEmployeeToEdit != 0)
            {
                await application.EditSettingsAsync(intOfEmployeeToEdit);
            }
            break;
        case "x":
            return;
        default:
            Console.WriteLine("Invalid operation");
            break;
    }

    Console.WriteLine("Select operation");
    userInput = Console.ReadLine();
}