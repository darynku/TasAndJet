namespace SharedKernel.Common.Exceptions;

public class RoleDeniedAccessException(string message) : Exception(message);