using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Frontend.Components.Pages;

namespace Frontend.Services
{
    public class TopicResponse
    {
        [JsonPropertyName("value")]
        public string? Value { get; set; }

        [JsonPropertyName("label")]
        public string? Label { get; set; }
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
    }

    public class CommunityService
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public CommunityService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<List<TopicResponse>?> GetTopicsAsync()
        {
            try
            {
                var httpClient = _httpClientFactory.CreateClient("WebAPI");
                var response = await httpClient.GetAsync("community/get_topics/");

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadFromJsonAsync<List<TopicResponse>>();
                    Debug.WriteLine(responseContent);

                    return responseContent;
                }

                return null;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error fetching topics: {ex.Message}");
                return null;
            }
        }

        public void CreateCommunityAsync()
        {
            
        }
    }
}
