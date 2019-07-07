namespace WebChromiumCcsipro.Domain.Enums
{
    public enum SocketServiceEvents
    {
        Create,
        Connect,
        KioskConnect,
        ConnectSuccessfully,
        ConnectSocketError,
        ConnectError,
        Disconnect,
        TryingReconnect,
        SendData,
        SendMotionDetectSocket,
        SendMotionDetectSocketError,
        ReadData
    }
}