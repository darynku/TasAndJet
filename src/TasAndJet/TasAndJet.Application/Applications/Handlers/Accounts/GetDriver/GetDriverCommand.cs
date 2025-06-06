﻿using CSharpFunctionalExtensions;
using MediatR;
using SharedKernel.Common.Api;
using TasAndJet.Contracts.Response;

namespace TasAndJet.Application.Applications.Handlers.Accounts.GetDriver;

public record GetDriverCommand(Guid Id) : IRequest<Result<DriverProfileResponse, Error>>; 