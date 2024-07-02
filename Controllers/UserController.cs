using HNG_WEB_API.Models;
using Microsoft.AspNetCore.Mvc;
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

        [HttpPost("createUser")]
        public async Task<IActionResult> CreateUserAsync([FromBody] User userRequest)
        {
            try
            {
                // Make asynchronous GET requests to fetch IP and City
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

                // Assuming a fixed temperature value for demonstration
                var temperature = "11 degrees";

                // Create the result message
                var resultMessage = $"{userRequest.Username}, you are in {city} and the temperature is {temperature}";

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
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        
    }
}
