using HNG_WEB_API.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace YourNamespace.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LocationController : ControllerBase
    {
        private readonly HttpClient _httpClient;

        public LocationController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        [HttpPost("Visitor")]
        public async Task<IActionResult> CreateUserAsync([FromBody] Visitor visitor)
        {
            try
            {
              
                var getIpTask = _httpClient.GetStringAsync("https://ipapi.co/ip/");
                var getCityTask = _httpClient.GetStringAsync("https://ipapi.co/city/");

                // Await both tasks concurrently
                await Task.WhenAll(getIpTask, getCityTask);

                // Retrieve results from tasks
                var ip = await getIpTask;
                var city = await getCityTask;

                // Trim any whitespace from results
                ip = ip.Trim();
                city = city.Trim();

                // Fetch the weather data using the city
                string apiKey = "73ee67e91e294ddfbe8124236240207"; // Replace with your OpenWeatherMap API key
                string weatherApiUrl = $"http://api.weatherapi.com/v1/current.json?key={apiKey}&q={city}";

                HttpResponseMessage weatherResponse = await _httpClient.GetAsync(weatherApiUrl);
                weatherResponse.EnsureSuccessStatusCode();
                string weatherResponseBody = await weatherResponse.Content.ReadAsStringAsync();
                JObject weatherData = JObject.Parse(weatherResponseBody);

                // Extract temperature from the weather data
                string temperature = weatherData["current"]["temp_c"].ToString();

                // Create the result message
                var resultMessage = $" Hello, {visitor.visitorName}, you are in {city} and the temperature is {temperature} °C";

                // Create an anonymous object to hold IP, City, and the result message
                var result = new
                {
                    Ip = ip,
                    City = city,
                    Message = resultMessage
                };

                // Return the result object as a JSON response
                return Ok(result);
            }
            catch (HttpRequestException httpEx)
            {
                return StatusCode(500, $"HTTP request error: {httpEx.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }

        }


    }
}
