using System.Net.Http.Headers;
using System.Text;
using MVCImage.Models;
using Newtonsoft.Json;
namespace MVCImage.Services
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ApiService> _logger;


        public ApiService(HttpClient httpClient , ILogger<ApiService> logger)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("https://localhost:7139/api/");
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _logger = logger;
        }
        public async Task LogHistoryAsync(string action, string entity, int entityId)
        {
            var history = new History
            {
                Action = action,
                Entity = entity,
                EntityId = entityId,
                Timestamp = DateTime.UtcNow
            };

            var json = JsonConvert.SerializeObject(history);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("histories", content);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Failed to log history for action {Action} on entity {Entity} with id {EntityId}. Status Code: {StatusCode}", action, entity, entityId, response.StatusCode);
            }
        }

        public async Task<IEnumerable<History>> GetHistoriesAsync()
        {
            var response = await _httpClient.GetAsync("histories");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<IEnumerable<History>>(content);
        }
        public async Task ClearHistoriesAsync()
        {
            var response = await _httpClient.DeleteAsync($"Histories/clear");
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"Error clearing histories: {response.StatusCode}, {errorContent}");
            }
        }

        public async Task<IEnumerable<Category>> GetCategoriesAsync()
        {
            var response = await _httpClient.GetAsync("Categories");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<IEnumerable<Category>>(content);
        }

        public async Task<Category> GetCategoryByIdAsync(int id)
        {
            var response = await _httpClient.GetAsync($"Categories/{id}");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<Category>(content);
        }

        public async Task CreateCategoryAsync(Category category)
        {
            if (category.Images == null)
            {
                category.Images = new List<Image>();
            }

            var response = await _httpClient.PostAsJsonAsync("Categories", category);
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"Error creating category: {response.StatusCode}, {errorContent}");
            }
        }

        public async Task UpdateCategoryAsync(int id, Category category)
        {
            var json = JsonConvert.SerializeObject(category);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync($"Categories/{id}", content);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"Error updating category: {response.StatusCode}, {errorContent}");
            }
        }

        public async Task DeleteCategoryAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"Categories/{id}");

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"Error deleting category: {response.StatusCode}, {errorContent}");
            }
        }

        public async Task<IEnumerable<Image>> GetImagesAsync()
        {
            var response = await _httpClient.GetAsync("Images");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<IEnumerable<Image>>(content);
        }

        public async Task<Image> GetImageByIdAsync(int id)
        {
            var response = await _httpClient.GetAsync($"Images/{id}");
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Failed to retrieve image with id {Id}. Status Code: {StatusCode}", id, response.StatusCode);
                return null;
            }

            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<Image>(content);
        }

        public async Task CreateImageAsync(ImageUploadDto imageDto)
        {
            var json = JsonConvert.SerializeObject(imageDto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("Images", content);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"Error creating image: {response.StatusCode}, {errorContent}");
            }
        }

        public async Task UpdateImageAsync(int id, Image image)
        {
            var json = JsonConvert.SerializeObject(image);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync($"Images/{id}", content);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"Error updating image: {response.StatusCode}, {errorContent}");
            }
        }

        public async Task DeleteImageAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"Images/{id}");

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"Error deleting image: {response.StatusCode}, {errorContent}");
            }
        }

        public async Task<IEnumerable<Image>> SearchImagesAsync(string title, string description)
        {
            var response = await _httpClient.GetAsync($"Images/Search?title={title}&description={description}");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<IEnumerable<Image>>(content);
        }
    }
}
