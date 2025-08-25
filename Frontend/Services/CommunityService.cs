using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Frontend.Components.Pages;
using Frontend.Models;
using static Frontend.Components.Pages.CreateCommunity;

namespace Frontend.Services
{
    public class TopicResponse
    {
        [JsonPropertyName("value")]
        public string? Value { get; set; }

        [JsonPropertyName("label")]
        public string? Label { get; set; }
    }

    public class ApiResponse
    {
        [JsonPropertyName("message")]
        public string? Message { get; set; }
    }

    public class CommunityResponse
    {
        [JsonPropertyName("id")]
        public int? Id { get; set; }

        [JsonPropertyName("topic")]
        public string? Topic { get; set; }

        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("author_id")]
        public int? AuthorId { get; set; }

        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("member_count")]
        public int? MemberCount { get; set; }
    }

    public class CommunityService
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public List<TopicResponse>? TopicList { get; private set; }
        public List<CommunityModel>? CommunityList { get; private set; }


        public CommunityService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<bool> GetTopicsAsync()
        {
            try
            {
                var httpClient = _httpClientFactory.CreateClient("WebAPI");
                var response = await httpClient.GetAsync("community/get_topics/");

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadFromJsonAsync<List<TopicResponse>>();
                    TopicList = responseContent;

                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error fetching topics: {ex.Message}");
                return false;
            }
        }

        public async Task<string?> CreateCommunityAsync(CreateCommunityModel createCommunityModel)
        {
            try
            {
                var httpClient = _httpClientFactory.CreateClient("WebAPI");
                var jsonPayload = JsonSerializer.Serialize(createCommunityModel);
                var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

                var response = await httpClient.PostAsync("community/create/", content);

                if (response.IsSuccessStatusCode)
                {
                    return null;
                }

                return "Something went wrong.";
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error creating community: {ex.Message}");
                return null;
            }
        }

        public async Task GetCommunitiesAsync()
        {
            try
            {
                var httpClient = _httpClientFactory.CreateClient("WebAPI");
                var response = await httpClient.GetAsync("community/get/");

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadFromJsonAsync<List<CommunityResponse>>();

                    CommunityList = responseContent?.Select(c => new CommunityModel
                    {
                        Id = c.Id,
                        Topic = c.Topic,
                        Name = c.Name,
                        AuthorId = c.AuthorId,
                        Description = c.Description,
                        MemberCount = c.MemberCount
                    }).ToList();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error fetching communities: {ex.Message}");
            }
        }

    }
}
