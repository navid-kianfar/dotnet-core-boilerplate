namespace MyApplication.Abstraction.Fixtures;

public class EndpointConstants
{
    public const string Prefix = "api/v1/my-application";
    public const string Login = "account/login";
    public const string Register = "account/register";
    public const string Profile = "account/profile";
    public const string UploadPublic = "storage/upload/public";
    public const string UploadProtected = "storage/upload/protected";
    public const string DownloadPublic = "storage/download/public/{*path}";
    public const string DownloadProtected = "storage/download/protected/{*path}";
}