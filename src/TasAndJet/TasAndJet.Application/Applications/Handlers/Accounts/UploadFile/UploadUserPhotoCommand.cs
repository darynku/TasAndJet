using MediatR;
using Microsoft.AspNetCore.Http;

namespace TasAndJet.Application.Applications.Handlers.Accounts.UploadFile;
public record UploadUserPhotoCommand(Guid UserId, IFormFile File) : IRequest;