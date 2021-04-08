using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMqUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ReservationProcessor
{
    public class ReservationListener : RabbitListener
    {
        private readonly ILogger<ReservationListener> _logger;
        private readonly ReservationHttpService _service;

        public ReservationListener(ILogger<ReservationListener> logger, IOptionsMonitor<RabbitOptions> options, ReservationHttpService service) : base(options)
        {
            _logger = logger;
            base.QueueName = "reservations";
            base.ExchangeName = "";
            _service = service;
        }

        public override async Task<bool> Process(string message)
        {
           
            // deserialize the thing into a C# object again.
            var reservation = JsonSerializer.Deserialize<ReservationModel>(message);

            // Do the actual work... 1s per book
            var numberOfBooks = reservation.BookIds.Split(',').Count();
            await Task.Delay(1000 * numberOfBooks);
            // even number books get approved, odd get denied.
            if(numberOfBooks % 2 == 0)
            {
                _logger.LogInformation($"Got a reservation for {reservation.For}. It is approved.");
                return await _service.MarkReservationReady(reservation);
            } else
            {
                _logger.LogInformation($"Got a reservation for {reservation.For}. It is DENIED.");
                return await _service.MarkReservationDenied(reservation);
            }
            
            // let the API know --- TODO: Set up something in our API for this.
            // if the api call is success, return True from this method, otherwise return False (this is  our Plan B)
            
        }
    }
}
