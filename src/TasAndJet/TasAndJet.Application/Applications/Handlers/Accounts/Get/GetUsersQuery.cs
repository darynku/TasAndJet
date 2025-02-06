using MediatR;
using SharedKernel.Paged;
using TasAndJet.Contracts.Response;

namespace TasAndJet.Application.Applications.Handlers.Accounts.Get;

public record GetUsersQuery(Int32 Page, int PageSize) : IRequest<PagedList<UserResponse>>;