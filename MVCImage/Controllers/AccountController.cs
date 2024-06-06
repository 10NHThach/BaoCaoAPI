using Microsoft.AspNetCore.Mvc;
using MVCImage.Models;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;

namespace MVCImage.Controllers
{
    public class AccountController : Controller
    {
        private readonly HttpClient _httpClient;

        public AccountController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(UserLogin userLogin)
        {
            if (ModelState.IsValid)
            {
                var content = new StringContent(JsonConvert.SerializeObject(userLogin), Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync("api/auth/register", content);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Login");
                }
                ModelState.AddModelError("", "Đăng ký thất bại");
            }
            return View(userLogin);
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(UserLogin userLogin)
        {
            if (ModelState.IsValid)
            {
                var content = new StringContent(JsonConvert.SerializeObject(userLogin), Encoding.UTF8, "application/json");
                _httpClient.BaseAddress = new Uri("https://localhost:7139/");
                var response = await _httpClient.PostAsync("api/Auth/login", content);

                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    var authResponse = JsonConvert.DeserializeObject<AuthResponse>(jsonResponse);

                    // Lưu token và tên người dùng vào session
                    HttpContext.Session.SetString("JWToken", authResponse.Token);
                    HttpContext.Session.SetString("Username", userLogin.Username);

                    return RedirectToAction("Index", "Categories");
                }
                ModelState.AddModelError("", "Đăng nhập thất bại");
            }
            return View(userLogin);
        }
        [HttpPost]
        public IActionResult Logout()
        {
            // Xóa token và tên người dùng khỏi session
            HttpContext.Session.Remove("JWToken");
            HttpContext.Session.Remove("Username");

            // Chuyển hướng đến trang đăng nhập
            return RedirectToAction("Login");
        }
    }
}
