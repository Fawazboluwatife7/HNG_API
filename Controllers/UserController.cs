////using HNG_WEB_API.Models;
////using HNG_WEB_API.Service;
////using Microsoft.AspNetCore.Mvc;
////using Newtonsoft.Json.Linq;
////using System;
////using System.Net.Http;
////using System.Threading.Tasks;

////namespace YourNamespace.Controllers
////{
////    [Route("api/[controller]")]
////    [ApiController]
////    public class UserController : ControllerBase
////    {
////        private readonly IHngService _hngService;

////        //public UserController(IHngService hngService)
////        //{
////        //    _hngService = hngService;
////        //}
////       private readonly HttpClient _httpClient;

////        public UserController(HttpClient httpClient)
////       {
////            _httpClient = httpClient;
////        }

////        [HttpPost("Visitor")]
////        public async Task<IActionResult> CreateUserAsync([FromBody] Visitor visitor)
////        {
////            try
////            {

////                var getIpTask = _httpClient.GetStringAsync("https://ipapi.co/ip/");
////                var getCityTask = _httpClient.GetStringAsync("https://ipapi.co/city/");

////                // Await both tasks concurrently
////                await Task.WhenAll(getIpTask, getCityTask);

////                // Retrieve results from tasks
////                var ip = await getIpTask;
////                var city = await getCityTask;

////                // Trim any whitespace from results
////                ip = ip.Trim();
////                city = city.Trim();

////               // Fetch the weather data using the city
////                string apiKey = "73ee67e91e294ddfbe8124236240207"; // Replace with your OpenWeatherMap API key
////                string weatherApiUrl = $"http://api.weatherapi.com/v1/current.json?key={apiKey}&q={city}";

////                HttpResponseMessage weatherResponse = await _httpClient.GetAsync(weatherApiUrl);
////                weatherResponse.EnsureSuccessStatusCode();
////                string weatherResponseBody = await weatherResponse.Content.ReadAsStringAsync();
////               JObject weatherData = JObject.Parse(weatherResponseBody);

////               // Extract temperature from the weather data
////                string temperature = weatherData["current"]["temp_c"].ToString();

////                // Create the result message
////                var resultMessage = $" Hello, {visitor.visitorName}, you are in {city} and the temperature is {temperature} °C";

////                // Create an anonymous object to hold IP, City, and the result message
////                var result = new
////                {
////                    Ip = ip,
////                    City = city,
////                    Message = resultMessage
////                };

////                // Return the result object as a JSON response
////                return Ok(result);
////            }
////            catch (HttpRequestException httpEx)
////            {
////                return StatusCode(500, $"HTTP request error: {httpEx.Message}");
////            }
////            catch (Exception ex)
////            {
////                return StatusCode(500, $"Internal server error: {ex.Message}");
////            }

////        }

////        //[HttpGet]
////        //public async Task<ActionResult<Visitor>> GetVisitor (string ipStack2, string city, string name)
////        //{
////        //    return await _hngService.GetVisitor(ipStack2, city, name);
////        //}
////    }

////}





//using HNG_WEB_API.Models;
//using Microsoft.AspNetCore.Mvc;
//using Newtonsoft.Json.Linq;
//using System;
//using System.Net.Http;
//using System.Threading.Tasks;

//namespace YourNamespace.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class LocationController : ControllerBase
//    {
//        private readonly HttpClient _httpClient;

//        public LocationController(HttpClient httpClient)
//        {
//            _httpClient = httpClient;
//        }

//        [HttpGet("Visitor")]
//        public async Task<IActionResult> GetUserAsync(string visitorName)
//        {
//            try
//            {
//                // Fetch IP and City from ipapi.co
//                var getIpTask = _httpClient.GetStringAsync("https://ipapi.co/ip/");
//                var getCityTask = _httpClient.GetStringAsync("https://ipapi.co/city/");

//                // Await both tasks concurrently
//                await Task.WhenAll(getIpTask, getCityTask);

//                // Retrieve results from tasks
//                var ip = await getIpTask;
//                var city = await getCityTask;

//                // Trim any whitespace from results
//                ip = ip.Trim();
//                city = city.Trim();

//                // Fetch the weather data using the city
//                string apiKey = "73ee67e91e294ddfbe8124236240207"; // Replace with your OpenWeatherMap API key
//                string weatherApiUrl = $"http://api.weatherapi.com/v1/current.json?key={apiKey}&q={city}";

//                HttpResponseMessage weatherResponse = await _httpClient.GetAsync(weatherApiUrl);
//                weatherResponse.EnsureSuccessStatusCode();
//                string weatherResponseBody = await weatherResponse.Content.ReadAsStringAsync();
//                JObject weatherData = JObject.Parse(weatherResponseBody);

//                // Extract temperature from the weather data
//                string temperature = weatherData["current"]["temp_c"].ToString();

//                // Create the result message
//                var resultMessage = $"Hello, {visitorName}, you are in {city} and the temperature is {temperature} °C";

//                // Create an anonymous object to hold IP, City, and the result message
//                var result = new
//                {
//                    Ip = ip,
//                    City = city,
//                    Message = resultMessage
//                };

//                // Return the result object as a JSON response
//                return Ok(result);
//            }
//            catch (HttpRequestException httpEx)
//            {
//                return StatusCode(500, $"HTTP request error: {httpEx.Message}");
//            }
//            catch (Exception ex)
//            {
//                return StatusCode(500, $"Internal server error: {ex.Message}");
//            }
//        }
//    }
//}
using HNG_WEB_API.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace YourNamespace.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class helloController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        private readonly IpApiClient _ipApiClient;

        public helloController(HttpClient httpClient, IpApiClient ipApiClient)
        {
            _httpClient = httpClient;
            _ipApiClient = ipApiClient;
        }

        [HttpGet]
        public async Task<IActionResult> GetVisitorInfo(string visitor_name, CancellationToken ct)
        {
            try
            {
                // Decode the visitor_name parameter to handle URL-encoded quotation marks
                visitor_name = HttpUtility.UrlDecode(visitor_name);

                // Remove surrounding quotation marks, if any
                if (!string.IsNullOrEmpty(visitor_name) && visitor_name.StartsWith("\"") && visitor_name.EndsWith("\""))
                {
                    visitor_name = visitor_name.Substring(1, visitor_name.Length - 2);
                }

                // Get the client's IP address
                var ipAddress = HttpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault()
                                ?? HttpContext.Connection.RemoteIpAddress?.ToString();
                var ipAddressWithoutPort = ipAddress?.Split(':')[0];

                // Get the city from the IP address
                var ipApiResponse = await _ipApiClient.Get(ipAddressWithoutPort, ct);

                var response = new
                {
                    IpAddress = ipAddressWithoutPort,
                    City = ipApiResponse?.city,
                };

                if (string.IsNullOrEmpty(response.City))
                {
                    return BadRequest("Could not determine city from IP address.");
                }

                // Fetch the weather data using the city
                string apiKey = "73ee67e91e294ddfbe8124236240207"; // Replace with your WeatherAPI key
                string weatherApiUrl = $"http://api.weatherapi.com/v1/current.json?key={apiKey}&q={response.City}";

                HttpResponseMessage weatherResponse = await _httpClient.GetAsync(weatherApiUrl);
                weatherResponse.EnsureSuccessStatusCode();
                string weatherResponseBody = await weatherResponse.Content.ReadAsStringAsync();
                JObject weatherData = JObject.Parse(weatherResponseBody);

                // Extract temperature from the weather data
                string temperature = weatherData["current"]["temp_c"].ToString();

                // Create the result message
                var resultMessage = $"Hello, {visitor_name}!, the temperature is {temperature} degrees Celcius  in {response.City}";

                // Create an anonymous object to hold IP, City, and the result message
                var result = new
                {
                    client_ip = response.IpAddress,
                    location = response.City,
                    greeting = resultMessage
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
