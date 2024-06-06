using Microsoft.AspNetCore.Mvc;
using MVCImage.Services;
using MVCImage.Models;
using Microsoft.Extensions.Logging;

namespace MVCImage.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly ApiService _apiService;
        private readonly ILogger<CategoriesController> _logger;

        public CategoriesController(ApiService apiService, ILogger<CategoriesController> logger)
        {
            _apiService = apiService;
            _logger = logger;
        }

        // GET: Categories
        public async Task<IActionResult> Index()
        {
            var categories = await _apiService.GetCategoriesAsync();
            // Lấy tên người dùng từ session và truyền ra view
            var username = HttpContext.Session.GetString("Username");
            ViewBag.Username = username;
            return View(categories);
        }

        // GET: Categories/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _apiService.GetCategoryByIdAsync(id.Value);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // GET: Categories/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Categories/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CategoryId,Name,Description")] Category category)
        {
            if (ModelState.IsValid)
            {
                await _apiService.CreateCategoryAsync(category);
                await _apiService.LogHistoryAsync("Create", "Category", category.CategoryId, $"Đã tạo danh mục có tên: {category.Name}"); // Log history
                TempData["SuccessMessage"] = "Danh mục được tạo thành công.";
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        // GET: Categories/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _apiService.GetCategoryByIdAsync(id.Value);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CategoryId,Name,Description")] Category category)
        {
            if (id != category.CategoryId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                // Ensure Images is not null
                if (category.Images == null)
                {
                    category.Images = new List<Image>();
                }

                await _apiService.UpdateCategoryAsync(id, category);
                await _apiService.LogHistoryAsync("Edit", "Category", category.CategoryId, $"Đã sửa danh mục có tên: {category.Name}"); // Log history
                TempData["SuccessMessage"] = "Danh mục được cập nhật thành công.";
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }


        // GET: Categories/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _apiService.GetCategoryByIdAsync(id.Value);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // POST: Categories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var category = await _apiService.GetCategoryByIdAsync(id);
            await _apiService.DeleteCategoryAsync(id);
            await _apiService.LogHistoryAsync("Delete", "Category", id, $"Đã xóa danh mục có tên: {category.Name}"); // Log history
            TempData["SuccessMessage"] = "Đã xóa danh mục thành công.";
            return RedirectToAction(nameof(Index));
        }
    }
}
