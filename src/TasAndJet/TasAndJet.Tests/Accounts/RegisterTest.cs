// using CSharpFunctionalExtensions;
// using MediatR;
// using Microsoft.Extensions.DependencyInjection;
// using SharedKernel.Common;
// using TasAndJet.Application.Applications.Handlers.Accounts.Register;
// using TasAndJet.Contracts.Data.Accounts;
//
// namespace TasAndJet.Tests.Accounts;
//
// public class RegisterTest : TestsBase
// {
//     public RegisterTest(IntegrationTestWebFactory factory) : base(factory)
//     {
//     }
//
//     [Fact]
//     public async Task RegisterUser_Should_Fail_When_Role_Not_Exists()
//     {
//         // Очистка базы перед тестом
//         var cancellationToken = new CancellationTokenSource().Token;
//
//         // Засеивание ролей
//         await SeedRoles();
//
//         var command = new RegisterUserCommand(new RegisterData
//         {
//             Address = "123 Main Street",
//             Email = "test@gmail.com",
//             FirstName = "John",
//             LastName = "Doe",
//             Password = "password",
//             PhoneNumber = "+77784928492",
//             Region = "asdada",
//             RoleId = 999 // Роли с таким ID нет
//         });
//
//         var result = await Mediator.Send(command, cancellationToken);
//
//         Assert.False(result.IsSuccess);
//         Assert.Contains(result.Error, e => e.Message.Contains("role")); // Проверяем, что ошибка связана с отсутствием роли
//     }
// }
