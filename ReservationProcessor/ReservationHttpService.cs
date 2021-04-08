using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ReservationProcessor
{
    public class ReservationHttpService
    {
        private readonly HttpClient _client;

        public ReservationHttpService(HttpClient client, IConfiguration config)
        {
            client.BaseAddress = new Uri(config.GetValue<string>("apiUrl"));
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            client.DefaultRequestHeaders.Add("User-Agent", "ReservationProcessor");
            _client = client;
        }

        public async Task<bool> MarkReservationDenied(ReservationModel reservation) // Denied
        {
            return await PostItToTheRightBucket(reservation, "denied");
        }

        public async Task<bool> MarkReservationReady(ReservationModel reservation) // ready
        {
            return await PostItToTheRightBucket(reservation, "ready");
        }

        private async Task<bool> PostItToTheRightBucket(ReservationModel reservation, string bucket)
        {
            
            var reservationJson = JsonSerializer.Serialize(reservation);
            var content = new StringContent(reservationJson);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var response = await _client.PostAsync($"reservations/{bucket}", content);
            return response.IsSuccessStatusCode;
        }
    }
}
