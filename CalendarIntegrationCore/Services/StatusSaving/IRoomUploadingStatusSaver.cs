namespace CalendarIntegrationCore.Services.StatusSaving
{
    public interface IRoomUploadingStatusSaver
    {
        void SetRoomStatus(int roomId, string status, string message);
    }
}