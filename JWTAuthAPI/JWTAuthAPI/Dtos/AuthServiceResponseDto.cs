namespace JWTAuthAPI.Dtos;

public class AuthServiceResponseDto
{
    public AuthServiceResponseDto() { }

    public AuthServiceResponseDto(bool isSucceed, string message)
    {
        IsSucceed = isSucceed;
        Message = message;
    }

    public bool IsSucceed { get; set; }
    public string Message { get; set; }
}
