
namespace VITRACK.Api.Errors;

public enum ErrorCodeEnum
{
    UNXEPECTED_ERROR,
    USER_ALREADY_EXISTS,
    INPUT_ERROR,
    LOGIN_PASSWORD_ERROR,
    LOCKED_OUT_ERROR,
    USER_NOT_FOUND,
    NOT_FOUND,
    INTERNAL_SERVER_ERROR,
    ATTENDANCE_RECORD_ALREADY_EXISTS,
}

public static class ErrorCodes
{
    public const string UNXEPECTED_ERROR = "Gözlənilməz xəta baş verdi.";
    public const string USER_ALREADY_EXISTS = "Belə bir istifadəçi artıq mövcuddur.";
    public const string INPUT_ERROR = "Daxil olunan məlumat sehvdir.";
    public const string LOGIN_PASSWORD_ERROR = "Login ve ya şifre sehvdir.";
    public const string LOCKED_OUT_ERROR = "Hesab müvəqqəti olaraq bloklanıb.";
    public const string USER_NOT_FOUND = "İstifadəçi tapılmadı.";
    public const string INTERNAL_SERVER_ERROR = "Daxili server xətası.";
    public const string ATTENDANCE_RECORD_ALREADY_EXISTS = "Bu tarix üçün qeyd artıq mövcuddur.";

}

