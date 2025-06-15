namespace Api.Infrastructure.Configuration;

public class StorageConfiguration
{
    public const string SectionName = "Storage";
    
    public StorageType Type { get; set; } = StorageType.Local;
    public LocalStorageSettings Local { get; set; } = new();
    public S3StorageSettings S3 { get; set; } = new();
}

public enum StorageType
{
    Local = 0,
    S3 = 1
}

public class LocalStorageSettings
{
    public string BasePath { get; set; } = "UploadedInvoices";
}

public class S3StorageSettings
{
    public string BucketName { get; set; } = string.Empty;
    public string Region { get; set; } = "us-east-1";
    public string AccessKey { get; set; } = string.Empty;
    public string SecretKey { get; set; } = string.Empty;
}

public class EmailConfiguration
{
    public const string SectionName = "Email";
    
    public string FromEmail { get; set; } = string.Empty;
    public string FromName { get; set; } = "Your Service Team";
    public AwsSesSettings Ses { get; set; } = new();
}

public class AwsSesSettings
{
    public string Region { get; set; } = "us-east-1";
    public string AccessKey { get; set; } = string.Empty;
    public string SecretKey { get; set; } = string.Empty;
}
