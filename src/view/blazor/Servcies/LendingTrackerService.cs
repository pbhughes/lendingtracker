using LendingView.Models;
using System.Text.Json;
using System.Text;

namespace LendingView.Servcies
{
    public class LendingTrackerService
    {

        private readonly IConfiguration _config;
        private readonly ILogger<LendingTrackerService> _logger;
        private readonly HttpClient _httpClient;
        public LendingTrackerService(IConfiguration config, ILogger<LendingTrackerService> logger, HttpClient httpClient)
        {
            _config = config;
            _logger = logger;
            _httpClient = httpClient;
        }



        // Generic helper method to make API requests
        private async Task<T> GetAsync<T>(string url)
        {
            var response = await _httpClient.GetAsync($"{_config["ApiHost:BaseUrl"]}{url}");
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<T>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        private async Task<bool> PostAsync<T>(string url, T data)
        {
            var json = JsonSerializer.Serialize(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync($"{_config["ApiHost:BaseUrl"]}{url}", content);
            return response.IsSuccessStatusCode;
        }

        private async Task<bool> PutAsync<T>(string url, T data)
        {
            var json = JsonSerializer.Serialize(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync($"{_config["ApiHost:BaseUrl"]}{url}", content);
            return response.IsSuccessStatusCode;
        }

        private async Task<bool> DeleteAsync(string url)
        {
            var response = await _httpClient.DeleteAsync($"{_config["ApiHost:BaseUrl"]}{url}");
            return response.IsSuccessStatusCode;
        }

        // --- Admin Endpoints ---
        public Task<List<Item>> GetAdminItemsAsync() => GetAsync<List<Item>>("/admin/items");
        public Task<bool> UpdateAdminItemAsync(int id, Item item) => PutAsync($"/admin/items/{id}", item);

        // --- Borrowers Endpoints ---
        public Task<List<Borrower>> GetBorrowersAsync() => GetAsync<List<Borrower>>("/borrowers");
        public Task<Borrower> GetBorrowerByIdAsync(string id) => GetAsync<Borrower>($"/borrowers/{id}");
        public Task<bool> CreateBorrowerAsync(Borrower borrower, bool duplicate = false) =>
            PostAsync($"/borrowers?duplicate={duplicate}", borrower);
        public Task<bool> UpdateBorrowerAsync(string id, Borrower borrower) => PutAsync($"/borrowers/{id}", borrower);
        public Task<bool> DeleteBorrowerAsync(string id) => DeleteAsync($"/borrowers/{id}");
        public Task<Borrower> ConfirmBorrowerAsync(string borrowerId, string apiKey) =>
            GetAsync<Borrower>($"/borrowers/confirm/{borrowerId}?apikey={apiKey}");
        public Task<List<Transaction>> GetBorrowerTransactionsAsync(string borrowerId) =>
            GetAsync<List<Transaction>>($"/borrowers/transactions/{borrowerId}");

        // --- Items Endpoints ---
        public Task<List<Item>> GetItemsAsync() => GetAsync<List<Item>>("/items");
        public Task<Item> GetItemByIdAsync(int id) => GetAsync<Item>($"/items/{id}");
        public Task<bool> CreateItemAsync(Item item) => PostAsync("/items", item);
        public Task<bool> UpdateItemAsync(int id, Item item) => PutAsync($"/items/{id}", item);
        public Task<bool> DeleteItemAsync(int id) => DeleteAsync($"/items/{id}");

        // --- Users Endpoints ---
        public Task<User> GetUsersAsync() => GetAsync<User>("/users");
        public Task<User> GetUserByIdAsync(int id) => GetAsync<User>($"/users/{id}");
        public Task<bool> CreateUserAsync(User user) => PostAsync("/users", user);
        public Task<bool> UpdateUserAsync(string id, User user) => PutAsync($"/users/{id}", user);
        public Task<bool> DeleteUserAsync(int id) => DeleteAsync($"/users/{id}");

        // --- Transactions Endpoints ---
        public Task<List<Transaction>> GetTransactionsAsync() => GetAsync<List<Transaction>>("/transactions");
        public Task<Transaction> GetTransactionByIdAsync(int id) => GetAsync<Transaction>($"/transactions/{id}");
        public Task<bool> CreateTransactionAsync(Transaction transaction) => PostAsync("/transactions", transaction);
        public Task<bool> UpdateTransactionAsync(int id, Transaction transaction) => PutAsync($"/transactions/{id}", transaction);
        public Task<bool> DeleteTransactionAsync(int id) => DeleteAsync($"/transactions/{id}");

        // --- Messages Endpoints ---
        public Task<bool> SendMessageAsync(Transaction transaction, string message, string phone, string method, string direction) =>
            PostAsync($"/messages?message={message}&phone={phone}&method={method}&direction={direction}", transaction);
        public Task<Message> GetMessageByTransactionIdAsync(string transactionId) =>
            GetAsync<Message>($"/messages/{transactionId}");

        // --- Image Upload ---
        public async Task<bool> UploadImageAsync(byte[] imageBytes)
        {
            var content = new ByteArrayContent(imageBytes);
            var response = await _httpClient.PostAsync($"{_config["ApiHost:BaseUrl"]}/images", content);
            return response.IsSuccessStatusCode;
        }
    }
}
