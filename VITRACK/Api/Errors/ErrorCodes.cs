
namespace VITRACK.Api.Errors;

public enum ErrorCodeEnum
{
    INPUT_ERROR,
    EMAIL_PASSWORD_ERROR,
    LOCKED_OUT_ERROR,
    USER_NOT_FOUND,
    FACT_NOT_FOUND,
    INTERNAL_SERVER_ERROR,
}

public static class ErrorCodes
{
    public const string INPUT_ERROR = "Daxil olunan məlumat sehvdir.";
    public const string EMAIL_PASSWORD_ERROR = "Email ve ya şifre sehvdir.";
    public const string LOCKED_OUT_ERROR = "Hesab müvəqqəti olaraq bloklanıb.";
    public const string USER_NOT_FOUND = "İstifadəçi tapılmadı.";
    public const string INTERNAL_SERVER_ERROR = "Daxili server xətası.";

}

