using System.Collections.Generic;
using CalendarIntegrationCore.Models;
using CalendarIntegrationCore.Services.Repositories;

namespace CalendarIntegrationCore.Services.DataRetrieving
{
    public class AvailabilityStatusMessageQueue : IAvailabilityStatusMessageQueue
    {
        private readonly IAvailabilityStatusMessageRepository _availabilityStatusMessageRepository;

        public AvailabilityStatusMessageQueue(IAvailabilityStatusMessageRepository availabilityStatusMessageRepository)
        {
            _availabilityStatusMessageRepository = availabilityStatusMessageRepository;
        }

        /// <summary>
        /// Данный метод добавляет значение в конец очереди 
        /// </summary>
        /// <param name="availabilityStatusMessage">
        /// Объект класса DateChangeStatus, добавляемый в очередь
        /// </param>
        public void Enqueue(AvailabilityStatusMessage availabilityStatusMessage)
        {
            _availabilityStatusMessageRepository.Add(availabilityStatusMessage);
        }

        /// <summary>
        /// Данный метод добавляет в очередь элементы, находящиеся в списке.
        /// Элементы добавляются в том же порядке что и в списке 
        /// </summary>
        /// <param name="availabilityStatusMessages">
        /// Объекты типа AvailabilityStatusMessage, которые будут добавлены в очередь
        /// </param>
        public void EnqueueMultiple(List<AvailabilityStatusMessage> availabilityStatusMessages)
        {
            foreach (AvailabilityStatusMessage currAvailStatusMessage in availabilityStatusMessages)
            {
                _availabilityStatusMessageRepository.Add(currAvailStatusMessage);
            }
        }

        /// <summary>
        /// Метод возвращает объект, находящийся в начале очереди
        /// </summary>
        /// <returns>
        /// Объект типа AvailabilityStatusMessage, находящийся в начале очереди
        /// </returns>
        public AvailabilityStatusMessage Peek()
        {
            return _availabilityStatusMessageRepository.GetFirst();
        }

        
        /// <summary>
        /// Данный метод возвращает указанное количество элементов из очереди
        /// </summary>
        /// <param name="numberOfElements">Количество элементов, которое необходимо вернуть</param>
        /// <returns></returns>
        public List<AvailabilityStatusMessage> PeekMultiple(int numberOfElements)
        {
            List<AvailabilityStatusMessage> result = _availabilityStatusMessageRepository.GetTop(numberOfElements);
            return result;
        }
        
        /// <summary>
        /// Данный метод удаляет первый элемент из очереди и возвращает его
        /// </summary>
        /// <returns>
        /// Объект типа AvailabilityStatusMessage, находящийся в начале очереди
        /// </returns>
        public AvailabilityStatusMessage Dequeue()
        {
            AvailabilityStatusMessage firstElement = _availabilityStatusMessageRepository.GetFirst();
            _availabilityStatusMessageRepository.Delete(firstElement);
            return firstElement;
        }
        
        /// <summary>
        /// Данный метод удаляет из начала очереди указанное количество элементов
        /// </summary>
        /// <param name="numberOfElements">Количество элементов, которое необходимо удалить</param>
        public void DequeueMultiple(int numberOfElements)
        {
            List<AvailabilityStatusMessage> result = _availabilityStatusMessageRepository.GetTop(numberOfElements);
            _availabilityStatusMessageRepository.DeleteMultiple(result);
        }
    }
}