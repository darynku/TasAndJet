using MediatR;
using SharedKernel.Paged;
using TasAndJet.Contracts.Response;

namespace TasAndJet.Application.Applications.Handlers.Accounts.Get;

public record GetUsersQuery(int Page, int PageSize) : IRequest<PagedList<UserResponse>>;