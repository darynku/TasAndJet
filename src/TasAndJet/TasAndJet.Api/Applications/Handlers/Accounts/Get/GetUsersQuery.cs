using MediatR;
using TasAndJet.Api.Common;
using TasAndJet.Api.Common.Paged;
using TasAndJet.Api.Contracts.Response;

namespace TasAndJet.Api.Applications.Handlers.Accounts.Get;

public record GetUsersQuery(Int32 Page, int PageSize) : IRequest<PagedList<UserResponse>>;