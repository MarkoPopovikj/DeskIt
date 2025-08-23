using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Frontend.Components.Pages;
using Frontend.Models;
using Microsoft.AspNetCore.Components;
using static Frontend.Components.Pages.Login;
using Frontend.Constants;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using System.Net.Http.Headers;
using static Frontend.Components.Pages.Register;
using System.Diagnostics;

namespace Frontend.Services
{
    public class AuthResponse
    {
        [JsonPropertyName("access")]
        public string? Access { get; set; }

        [JsonPropertyName("refresh")]
        public string? Refresh { get; set; }
    }

    public class UserInfo
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("username")]
        public string? Username { get; set; }

        [JsonPropertyName("email")]
        public string? Email { get; set; }

        [JsonPropertyName("background_color")]
        public string? BackgroundColor { get; set; }

        [JsonPropertyName("about_me")]
        public string? AboutMe { get; set; }

        [JsonPropertyName("created_at")]
        public DateTime CreatedAt { get; set; }
    }

    public class AuthService
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public UserModel? CurrentUser { get; private set; }
        public bool isLoggedIn => CurrentUser != null;

        public event Action? OnAuthStateChanged;

        public AuthService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task InitializeAsync()
        {
            var token = await SecureStorage.GetAsync(TokenConstants.AccessToken);

            if (!string.IsNullOrEmpty(token))
            {
                await LoadUser(token);
            }
        }

        public async Task LoadUser(string token)
        {
            try
            {
                var httpClient = _httpClientFactory.CreateClient("WebAPI");
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var response = await httpClient.GetAsync("user/get_user/");

                if (response.IsSuccessStatusCode) {
                    var responseContent = await response.Content.ReadFromJsonAsync<UserInfo>();

                    CurrentUser = new UserModel
                    {
                        UserId = responseContent?.Id ?? -1,
                        Username = responseContent?.Username,
                        Email = responseContent?.Email,
                        BackgroundColor = responseContent?.BackgroundColor,
                        AboutMe = responseContent?.AboutMe,
                        CreatedAt = responseContent?.CreatedAt ?? DateTime.MinValue
                    };

                    OnAuthStateChanged?.Invoke();
                }
                
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Load error: {ex.Message}");
            }
        }

        public void UpdateCurrentUser(UserModel user)
        {
            CurrentUser = user;
            OnAuthStateChanged?.Invoke();
        }

        public async Task<string?> RegisterAsync(RegisterModel registerModel)
        {
            try
            {
                var httpClient = _httpClientFactory.CreateClient("WebAPI");
                var jsonPayload = JsonSerializer.Serialize(registerModel);
                var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

                var response = await httpClient.PostAsync("auth/register/", content);

                if (response.IsSuccessStatusCode)
                {
                    var registerResult = await response.Content.ReadFromJsonAsync<AuthResponse>();

                    if (registerResult != null && !string.IsNullOrEmpty(registerResult.Access))
                    {
                        await SecureStorage.SetAsync(TokenConstants.AccessToken, registerResult.Access);
                        await SecureStorage.SetAsync(TokenConstants.RefreshToken, registerResult.Refresh);

                        await LoadUser(registerResult.Access);

                        OnAuthStateChanged?.Invoke();

                        return null;
                    }
                    return "Login failed: Invalid response from server.";
                }
                else
                {
                    var errorContent = await response.Content.ReadFromJsonAsync<Dictionary<string, List<string>>>();
                    if (errorContent != null && errorContent.Any())
                    {
                        // Find the first error list and return the first message from it.
                        Debug.WriteLine($"Error content: {JsonSerializer.Serialize(errorContent)}");
                        return errorContent.Values.FirstOrDefault()?.FirstOrDefault();
                    }
                    return "An unknown error occurred.";
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Register error: {ex.Message}");
                return ex.Message;
            }
        }

        public async Task<string?> LoginAsync(LoginModel loginModel)
        {
            try
            {
                var httpClient = _httpClientFactory.CreateClient("WebAPI");
                var jsonPayload = JsonSerializer.Serialize(loginModel);
                var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

                var response = await httpClient.PostAsync("auth/login/", content);

                if (response.IsSuccessStatusCode)
                {
                    var loginResult = await response.Content.ReadFromJsonAsync<AuthResponse>();

                    if (loginResult != null && !string.IsNullOrEmpty(loginResult.Access))
                    {
                        await SecureStorage.SetAsync(TokenConstants.AccessToken, loginResult.Access);
                        await SecureStorage.SetAsync(TokenConstants.RefreshToken, loginResult.Refresh);

                        await LoadUser(loginResult.Access);

                        OnAuthStateChanged?.Invoke();

                        return null;
                    }
                    return "Login failed: Invalid response from server.";
                }
                else
                {
                    var errorContent = await response.Content.ReadFromJsonAsync<Dictionary<string, object>>();
                    if (errorContent != null && errorContent.TryGetValue("detail", out var detail))
                    {
                        return detail.ToString();
                    }
                    return "Invalid email or password.";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Login error: {ex.Message}");
                return "An error occurred. Please check your connection and try again.";
            }
        }

        public async Task LogoutAsync()
        {
            try
            {
                var refreshToken = await SecureStorage.GetAsync(TokenConstants.RefreshToken);
                if (!string.IsNullOrEmpty(refreshToken))
                {
                    var httpClient = _httpClientFactory.CreateClient("WebAPI");
                    var requestBody = new { refresh = refreshToken };
                    var content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");
                    await httpClient.PostAsync("auth/logout/", content);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during logout API call: {ex.Message}");
            }
            finally
            {
                CurrentUser = null;
                SecureStorage.Remove(TokenConstants.AccessToken);
                SecureStorage.Remove(TokenConstants.RefreshToken);
                OnAuthStateChanged?.Invoke();
            }
        }
    }
}
