namespace MinimalAPI.Model;

public record Response<T>(T? Data, bool Success, string Code, string Message);
