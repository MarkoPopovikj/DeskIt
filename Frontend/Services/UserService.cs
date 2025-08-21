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

    public class UserService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly AuthService _authService;

        public UserService(IHttpClientFactory httpClientFactory, AuthService authService)
        {
            _httpClientFactory = httpClientFactory;
            _authService = authService;
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

                    return null;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Update password error: {ex.Message}");
                return "An error occurred. Please check your connection and try again.";
            }
        }
    }
}
