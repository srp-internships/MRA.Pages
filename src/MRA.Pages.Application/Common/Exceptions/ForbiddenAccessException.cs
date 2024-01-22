namespace MRA.Pages.Application.Common.Exceptions;

public class ForbiddenAccessException(string message) : Exception(message);