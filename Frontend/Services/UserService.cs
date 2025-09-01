using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Frontend.Constants;
using Frontend.Models;
using static Frontend.Components.Pages.EditProfile;

namespace Frontend.Services
{
    public class PasswordChangeResponse
    {
        [JsonPropertyName("message")]
        public string? Message { get; set; }

        [JsonPropertyName("access_token")]
        public string? AccessToken { get; set; }
    }

    public class UserResponse
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("username")]
        public string? Username { get; set; }

        [JsonPropertyName("background_color")]
        public string? BackgroundColor { get; set; }

        [JsonPropertyName("about_me")]
        public string? AboutMe { get; set; }

        [JsonPropertyName("created_at")]
        public DateTime CreatedAt { get; set; }
    }

    public class UserService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly AuthService _authService;

        public UserModel SelectedUser { get; set; }

        public UserService(IHttpClientFactory httpClientFactory, AuthService authService)
        {
            _httpClientFactory = httpClientFactory;
            _authService = authService;
        }

        public async Task<bool> GetUserAsync(string username)
        {
            try
            {
                var httpClient = _httpClientFactory.CreateClient("WebAPI");
                var response = await httpClient.GetAsync($"user/{username}/get_user/");

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadFromJsonAsync<UserResponse>();

                    SelectedUser = new UserModel
                    {
                        UserId = responseContent?.Id ?? 0,
                        Username = responseContent?.Username ?? "Unknown",
                        BackgroundColor = responseContent?.BackgroundColor ?? "#FFFFFF",
                        Email = "",
                        AboutMe = responseContent?.AboutMe ?? "",
                        CreatedAt = responseContent?.CreatedAt ?? DateTime.UtcNow
                    };

                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Get other user error: {ex.Message}");
                return false;
            }
        }

        public async Task<string?> UpdateSimpleUserDataAsync(ProfileSimpleData profileSimpleData)
        {
            try
            {
                var httpClient = _httpClientFactory.CreateClient("WebAPI");
                var jsonPayload = JsonSerializer.Serialize(profileSimpleData);
                var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

                var response = await httpClient.PutAsync("user/update_simple_data/", content);

                if (response.IsSuccessStatusCode)
                {
                    var accessToken = await SecureStorage.GetAsync(TokenConstants.AccessToken);

                    await _authService.LoadUser(accessToken);
                    return null;
                }

                return "Something went wrong.";
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Update simple data error: {ex.Message}");
                return "An error occurred. Please check your connection and try again.";
            }
        }

        public async Task<string?> UpdateUserPasswordAsync(string password)
        {
            try
            {
                var httpClient = _httpClientFactory.CreateClient("WebAPI");

                var payload = new { password = password };
                var jsonPayload = JsonSerializer.Serialize(payload);
                var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

                var response = await httpClient.PutAsync("user/update_password/", content);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadFromJsonAsync<PasswordChangeResponse>();
                    
                    await SecureStorage.SetAsync(TokenConstants.AccessToken, responseContent?.AccessToken ?? string.Empty);

                    return null;
                }
                else
                {
                    return "Something went wrong";
                }

            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Update password error: {ex.Message}");
                return "An error occurred. Please check your connection and try again.";
            }
        }
    }
}
