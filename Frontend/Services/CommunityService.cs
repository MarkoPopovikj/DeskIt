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
using static Frontend.Components.Pages.EditCommunity;

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

        [JsonPropertyName("background_color")]
        public string? BackgroundColor { get; set; }

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

        public Dictionary<string,string> TopicDictionary { get; private set; }
        public Dictionary<string, List<CommunityModel>>? CommunityDictionary { get; private set; }
        public List<int> JoinedCommunities { get; set; }


        public CommunityService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;

            TopicDictionary = new Dictionary<string, string>();
            CommunityDictionary = new Dictionary<string, List<CommunityModel>>();
            JoinedCommunities = new List<int>();
        }

        public CommunityModel GetCommunity(string Name)
        {
            foreach(CommunityModel community in CommunityDictionary.Values.SelectMany(list => list))
            {
                if (community.Name == Name)
                {
                    return community;
                }
            }

            return null;
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
                    
                    foreach(TopicResponse topic in responseContent)
                    {
                        TopicDictionary[topic.Value] = topic.Label;
                    }

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

        public async Task<string?> UpdateCommunityAsync(EditCommunityModel editCommunityModel, int communityId)
        {
            if(communityId == -1)
            {
                return "Invalid";
            }

            try
            {
                var httpClient = _httpClientFactory.CreateClient("WebAPI");
                var jsonPayload = JsonSerializer.Serialize(editCommunityModel);
                var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

                var response = await httpClient.PutAsync($"community/{communityId}/update/", content);
                if (response.IsSuccessStatusCode)
                {
                    return null;
                }

                return "Something went wrong.";
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error updating community: {ex.Message}");
                return null;
            }
        }

        public async Task<bool> GetCommunitiesAsync()
        {
            try
            {
                var httpClient = _httpClientFactory.CreateClient("WebAPI");
                var response = await httpClient.GetAsync("community/get/");

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadFromJsonAsync<List<CommunityResponse>>();

                    foreach(KeyValuePair<string,string> topicPair in TopicDictionary)
                    {
                        CommunityDictionary[topicPair.Value] = new List<CommunityModel>();
                    }

                    foreach(CommunityResponse c in responseContent)
                    {
                        CommunityModel newCommunity = new CommunityModel
                        {
                            Id = c.Id,
                            Topic = c.Topic,
                            Name = c.Name,
                            AuthorId = c.AuthorId,
                            Description = c.Description,
                            BackgroundColor = c.BackgroundColor,
                            MemberCount = c.MemberCount
                        };

                        string rightTopic = TopicDictionary[newCommunity.Topic];
                        CommunityDictionary[rightTopic].Add(newCommunity);
                    }

                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error fetching communities: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> GetJoinedCommunitiesAsync()
        {
            try
            {
                var httpClient = _httpClientFactory.CreateClient("WebAPI");
                var response = await httpClient.GetAsync("community/get_memberships/");

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadFromJsonAsync<List<int>>();
                    JoinedCommunities = responseContent;

                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error fetching joined communities: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> JoinCommunityAsync(int communityId)
        {
            if(communityId == -1)
            {
                return false;
            }

            try
            {
                var httpClient = _httpClientFactory.CreateClient("WebAPI");
                var response = await httpClient.PostAsync($"community/{communityId}/join/", null);

                if (response.IsSuccessStatusCode)
                {
                    JoinedCommunities.Add(communityId);
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error joining community: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> LeaveCommunityAsync(int communityId)
        {
            if(communityId == -1)
            {
                return false;
            }   

            try
            {
                var httpClient = _httpClientFactory.CreateClient("WebAPI");
                var response = await httpClient.PostAsync($"community/{communityId}/leave/", null);

                if (response.IsSuccessStatusCode)
                {
                    JoinedCommunities.Remove(communityId);
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error joining community: {ex.Message}");
                return false;
            }
        }

    }
}
