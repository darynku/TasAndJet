namespace SharedKernel.Common.Api;

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
        public static Error AccessDenied()
        {
            return Error.Validation("user.access.denied", "Неверная роль, доступ отклонен");
        }
        public static Error InvalidCredentials()
        {
            return Error.Validation("credentials.is.invalid", "Неверные учетные данные");
        }

        public static Error InvalidRole()
        {
            return Error.Failure("role.is.invalid", "Неверная роль пользователя");
        }
    }

    public static class Reviews
    {
        public static Error InvalidOperation()
        {
            return Error.Validation("order.not.completed", "Order not completed");
        }
        public static Error ReviewNotNull()
        {
            return Error.Validation("review.not.null", "Нельзя менять отзыв");
        }
    }
    
    public static class Authentication
    {
        public static Error InvalidGoogleToken() =>
            Error.Validation("auth.google.invalid", "Ошибка проверки Google токена");

        public static Error UserNotFound() =>
            Error.NotFound("auth.user.not.found", "Пользователь не найден");

        public static Error InvalidVerificationCode() =>
            Error.Validation("auth.code.invalid", "Неверный код подтверждения");
    }
    public static class Orders
    {
        public static Error InvalidStatus()
        {
            return Error.Validation("status.invalid", "Неверный статус заказа");
        }

        public static Error CantCancel()
        {
            return Error.Validation("order.is.canceled", "Нельзя изменить статус отмененного заказа.");
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