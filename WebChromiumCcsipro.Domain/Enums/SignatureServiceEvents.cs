namespace WebChromiumCcsipro.Domain.Enums
{
    public enum SignatureServiceEvents
    {
        Create,
        StartSign,
        CreateDirectory,
        CreateDirectoryError,
        SignFile,
        SaveFile,
        SaveFileError,
        SignFileWindowFound,
        SignFileWindowClosed,
        SignFileAfterWait,
        WindowNotFound,
        WindowFound,
        WindowFoundAndClosing
    }
}