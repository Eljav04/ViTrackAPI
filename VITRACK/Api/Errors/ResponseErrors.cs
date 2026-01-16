
namespace VITRACK.Api.Errors;

public class ResponseErrors
{
    public ErrorCodeEnum? ErrorCodeSetter { private get => ErrorCodeSetter; set => ErrorCode = value.ToString(); }
    public string? ErrorCode { get; private set; }
    public string? Message { get; set; }
}


