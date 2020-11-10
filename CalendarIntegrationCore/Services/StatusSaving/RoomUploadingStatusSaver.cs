using System;
using System.Collections.Generic;
using CalendarIntegrationCore.Models;
using CalendarIntegrationCore.Services.Repositories;

namespace CalendarIntegrationCore.Services.StatusSaving
{
    public class RoomUploadingStatusSaver: IRoomUploadingStatusSaver
    {
        private readonly IRoomUploadStatusRepository _roomUploadStatusRepository;
        private readonly List<IObserver<RoomUploadStatus>> _observers;

        public RoomUploadingStatusSaver(List<IObserver<RoomUploadStatus>> observers, IRoomUploadStatusRepository roomUploadStatusRepository)
        {
            _observers = observers;
            _roomUploadStatusRepository = roomUploadStatusRepository;
        }

        public void SetRoomStatus(int roomId, string status, string message)
        {
            RoomUploadStatus newRoomUploadStatus = new RoomUploadStatus
            {
                RoomId = roomId,
                Status = status,
                Message = message
            };
            _roomUploadStatusRepository.SetStatus(newRoomUploadStatus);
            foreach (var observer in _observers)
            {
                observer.OnNext(newRoomUploadStatus);
            }
        }

        public IDisposable Subscribe(IObserver<RoomUploadStatus> observer)
        {
            _observers.Add(observer);
            return new Unsubscriber(_observers, observer);
        }
        
        private class Unsubscriber : IDisposable
        {
            private readonly List<IObserver<RoomUploadStatus>> _observers;
            private readonly IObserver<RoomUploadStatus> _observer;

            public Unsubscriber(List<IObserver<RoomUploadStatus>> observers, IObserver<RoomUploadStatus> observer)
            {
                this._observers = observers;
                this._observer = observer;
            }

            public void Dispose()
            {
                if (_observer != null)
                {
                    _observers.Remove(_observer);
                }
            }
        }
    }
}