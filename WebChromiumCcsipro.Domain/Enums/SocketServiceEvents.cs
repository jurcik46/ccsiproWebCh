namespace WebChromiumCcsipro.Domain.Enums
{
    public enum SocketServiceEvents
    {
        Create,
        ServerConnect,
        KioskConnect,
        ServerConnectSuccessfully,
        KioskConnectSuccessfully,
        ServerConnectSocketError,
        KioskConnectSocketError,
        ServerConnectError,
        KioskConnectError,
        Disconnect,
        ServerTryingReconnect,
        KioskTryingReconnect,
        SendData,
        SendMotionDetectSocket,
        SendMotionDetectSocketError,
        KioskSendData,
        KioskSendDataError,
        ServerReadData,
        ServerReadDataError,
        SendOneTimeSocketMessage,
        SendOneTimeSocketMessageSocketError,
        SendOneTimeSocketMessageError,


    }
}