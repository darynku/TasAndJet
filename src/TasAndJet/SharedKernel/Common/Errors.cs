﻿namespace SharedKernel.Common;

public static class Errors
{
    public static class General
    {
        public static Error ValueIsInvalid(string? name = null)
        {
            var label = name ?? "value";
            return Error.Validation("value.is.invalid", $"{label} is invalid");
        }

        public static Error NotFound(Guid? id = null, string? name = null)
        {
            var forId = id == null ? "" : $" for Id '{id}'";
            return Error.NotFound("record.not.found", $"{name ?? "record"} not found{forId}");
        }

        public static Error ValueIsRequired(string? name = null)
        {
            var label = name == null ? "" : " " + name + " ";
            return Error.Validation("length.is.invalid", $"invalid{label}length");
        }

        public static Error AlreadyExist()
        {
            return Error.Validation("record.already.exist", "Record already exist");
        }

        public static Error Failure()
        {
            return Error.Failure("server.failure", "Server failure");
        }

        public static Error NotAllowed()
        {
            return Error.Failure("not.allowed", "Not allowed");
        }
    }

    public static class Tokens
    {
        public static Error ExpiredToken()
        {
            return Error.Validation("token.is.expired", "Your token is expired");
        }

        public static Error InvalidToken()
        {
            return Error.Validation("token.is.invalid", "Your token is invalid");
        }
    }

    public static class User
    {
        public static Error InvalidCredentials()
        {
            return Error.Validation("credentials.is.invalid", "Неверные учетные данные");
        }

        public static Error InvalidRole()
        {
            return Error.Failure("role.is.invalid", "Неверная роль пользователя");
        }
    }

    public static class Files
    {
        public static Error InvalidExtension()
        {
            return Error.Validation("extension.is.invalid", "File extension is invalid");
        }

        public static Error InvalidSize()
        {
            return Error.Validation("size.is.invalid", "File size is invalid");
        }
    }
}