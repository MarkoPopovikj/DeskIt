using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Frontend.Models;
using static Frontend.Components.Pages.CreatePost;
using static Frontend.Components.Pages.EditPost;

namespace Frontend.Services
{
    public class PostVotesResponse
    {
        [JsonPropertyName("post_id")]
        public int PostId { get; set; }

        [JsonPropertyName("vote_type")]
        public int VoteValue { get; set; }
    }

    public class PostResponse
    {
        [JsonPropertyName("id")]
        public int? Id { get; set; }

        [JsonPropertyName("community_name")]
        public string? CommunityName { get; set; }

        [JsonPropertyName("author_name")]
        public string? AuthorName { get; set; }

        [JsonPropertyName("title")]
        public string? Title { get; set; }

        [JsonPropertyName("content")]
        public string? Content { get; set; }

        [JsonPropertyName("created_at")]
        public DateTime? CreatedAt { get; set; }

        [JsonPropertyName("upvotes")]
        public int? Upvotes { get; set; }

        [JsonPropertyName("downvotes")]
        public int? Downvotes { get; set; }

        [JsonPropertyName("comments_count")]
        public int? CommentsCount { get; set; }

        [JsonPropertyName("image_url")]
        public string? ImageUrl { get; set; }
    }
    public class PostService
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public Dictionary<int,int> UserPostVotes { get; private set; }
        public PostModel CurrentPost { get; set; }
        public List<PostModel> PostList { get; set; }
        // Za 1 user
        public List<PostModel> UserPostList { get; private set; }
        // Za 1 community
        public List<PostModel> CommunityPostList { get; private set; }

        public PostService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;

            PostList = new List<PostModel>();
            UserPostList = new List<PostModel>();
            CommunityPostList = new List<PostModel>();
            CurrentPost = new PostModel();
            UserPostVotes = new Dictionary<int, int>();
        }

        public async Task<bool> GetPostAsync(int postId)
        {
            try
            {
                var httpClient = _httpClientFactory.CreateClient("WebAPI");
                var response = await httpClient.GetAsync($"post/{postId}/get/");

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadFromJsonAsync<PostResponse>();
                    if (responseContent != null)
                    {
                        CurrentPost = new PostModel
                        {
                            Id = responseContent.Id ?? 0,
                            CommunityName = responseContent.CommunityName ?? "Unknown",
                            AuthorName = responseContent.AuthorName ?? "Unknown",
                            Title = responseContent.Title ?? "No Title",
                            Content = responseContent.Content ?? "",
                            CreatedAt = responseContent.CreatedAt ?? DateTime.UtcNow,
                            UpVotes = responseContent.Upvotes ?? 0,
                            DownVotes = responseContent.Downvotes ?? 0,
                            CommentsCount = responseContent.CommentsCount ?? 0,
                            ImageUrl = responseContent.ImageUrl
                        };

                        return true;
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error fetching post: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> GetPostsAsync()
        {
            try
            {
                var httpClient = _httpClientFactory.CreateClient("WebAPI");
                var response = await httpClient.GetAsync("post/get_all/");

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadFromJsonAsync<List<PostResponse>>();

                    PostList.Clear();

                    foreach (PostResponse postResponse in responseContent)
                    {
                        var newPost = new PostModel
                        {
                            Id = postResponse.Id ?? 0,
                            CommunityName = postResponse.CommunityName ?? "Unknown",
                            AuthorName = postResponse.AuthorName ?? "Unknown",
                            Title = postResponse.Title ?? "No Title",
                            Content = postResponse.Content ?? "",
                            CreatedAt = postResponse.CreatedAt ?? DateTime.UtcNow,
                            UpVotes = postResponse.Upvotes ?? 0,
                            DownVotes = postResponse.Downvotes ?? 0,
                            CommentsCount = postResponse.CommentsCount ?? 0,
                            ImageUrl = postResponse.ImageUrl
                        };

                        PostList.Add(newPost);
                    }

                    await GetUsesPostVotesAsync();

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

        public async Task<bool> GetUserPostsAsync(int userId)
        {
            try
            {
                var httpClient = _httpClientFactory.CreateClient("WebAPI");
                var response = await httpClient.GetAsync($"post/{userId}/get_user/");

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadFromJsonAsync<List<PostResponse>>();

                    UserPostList.Clear();

                    foreach (PostResponse postResponse in responseContent)
                    {
                        var newPost = new PostModel
                        {
                            Id = postResponse.Id ?? 0,
                            CommunityName = postResponse.CommunityName ?? "Unknown",
                            AuthorName = postResponse.AuthorName ?? "Unknown",
                            Title = postResponse.Title ?? "No Title",
                            Content = postResponse.Content ?? "",
                            CreatedAt = postResponse.CreatedAt ?? DateTime.UtcNow,
                            UpVotes = postResponse.Upvotes ?? 0,
                            DownVotes = postResponse.Downvotes ?? 0,
                            CommentsCount = postResponse.CommentsCount ?? 0,
                            ImageUrl = postResponse.ImageUrl
                        };

                        Debug.WriteLine(newPost.Title);

                        UserPostList.Add(newPost);
                    }

                    await GetUsesPostVotesAsync();

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

        public async Task<bool> GetCommunityPostsAsync(int communityId)
        {
            try
            {
                var httpClient = _httpClientFactory.CreateClient("WebAPI");
                var response = await httpClient.GetAsync($"post/{communityId}/get_community/");

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadFromJsonAsync<List<PostResponse>>();

                    CommunityPostList.Clear();

                    foreach (PostResponse postResponse in responseContent)
                    {
                        var newPost = new PostModel
                        {
                            Id = postResponse.Id ?? 0,
                            CommunityName = postResponse.CommunityName ?? "Unknown",
                            AuthorName = postResponse.AuthorName ?? "Unknown",
                            Title = postResponse.Title ?? "No Title",
                            Content = postResponse.Content ?? "",
                            CreatedAt = postResponse.CreatedAt ?? DateTime.UtcNow,
                            UpVotes = postResponse.Upvotes ?? 0,
                            DownVotes = postResponse.Downvotes ?? 0,
                            CommentsCount = postResponse.CommentsCount ?? 0,
                            ImageUrl = postResponse.ImageUrl
                        };

                        CommunityPostList.Add(newPost);
                    }

                    await GetUsesPostVotesAsync();

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

        public async Task<string?> CreatePostAsync(CreatePostModel createPost)
        {
            try
            {
                var httpClient = _httpClientFactory.CreateClient("WebAPI");
                var jsonPayload = JsonSerializer.Serialize(createPost);
                var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

                var response = await httpClient.PostAsync("post/create/", content);

                if (response.IsSuccessStatusCode)
                {
                    return null;
                }

                return "Something went wrong";
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error creating community: {ex.Message}");
                return "Something went wrong";
            }
        }

        public async Task<bool> UpdatePostAsync(int postId, EditPostModel updatePost)
        {
            try
            {
                var httpClient = _httpClientFactory.CreateClient("WebAPI");
                var jsonPayload = JsonSerializer.Serialize(updatePost);
                var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
                var response = await httpClient.PutAsync($"post/{postId}/update/", content);

                if (response.IsSuccessStatusCode)
                {
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error updating post: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> DeletePostAsync(int postId)
        {
            try
            {
                var httpClient = _httpClientFactory.CreateClient("WebAPI");
                var response = await httpClient.DeleteAsync($"post/{postId}/delete/");
                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error deleting post: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> UpdatePostUpvotesDownvotesAsync(int postId, int action)
        {
            UserPostVotes[postId] = action;

            try
            {
                var httpClient = _httpClientFactory.CreateClient("WebAPI");
                var response = await httpClient.PostAsync($"post/{postId}/{action.ToString()}/vote/", null);

                if (response.IsSuccessStatusCode)
                {
                    return true;
                }

                return false;
            }catch(Exception ex)
            {
                Debug.WriteLine($"Error updating post: {ex.Message}");
                return false;
            }
        }

        private async Task<bool> GetUsesPostVotesAsync()
        {
            try
            {
                var httpClient = _httpClientFactory.CreateClient("WebAPI");
                var response = await httpClient.GetAsync("post/get_user_votes/");

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadFromJsonAsync<List<PostVotesResponse>>();

                    if (responseContent != null)
                    {
                        UserPostVotes = responseContent.ToDictionary(vote => vote.PostId, vote => vote.VoteValue);
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
    }
}
