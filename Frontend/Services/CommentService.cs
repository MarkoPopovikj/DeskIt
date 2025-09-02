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
using static Frontend.Components.Pages.Post;

namespace Frontend.Services
{
    public class CommentResponse
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("author_name")]
        public string AuthorName { get; set; }

        [JsonPropertyName("content")]
        public string Content { get; set; }

        [JsonPropertyName("created_at")]
        public DateTime CreatedAt { get; set; }

        [JsonPropertyName("upvotes")]
        public int Upvotes { get; set; }

        [JsonPropertyName("downvotes")]
        public int Downvotes { get; set; }
    }

    public class CommentService
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public List<CommentModel> PostCommentsList { get; set; }

        public CommentService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;

            PostCommentsList = new List<CommentModel>();
        }

        public async Task<bool> GetPostCommentsAsync(int postId)
        {
            try
            {
                var httpClient = _httpClientFactory.CreateClient("WebAPI");
                var response = await httpClient.GetAsync($"comment/{postId}/get/");

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadFromJsonAsync<List<CommentResponse>>();

                    PostCommentsList.Clear();

                    foreach (CommentResponse c in responseContent)
                    {
                        PostCommentsList.Add(new CommentModel
                        {
                            Id = c.Id,
                            Content = c.Content,
                            CreatedAt = c.CreatedAt,
                            DownVotes = c.Downvotes,
                            UpVotes = c.Upvotes,
                            AuthorName = c.AuthorName,
                        });
                    }

                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return false;
            }
        }

        public async Task<bool> CreateCommentAsync(CreateCommentModel newComment)
        {
            try
            {
                var httpClient = _httpClientFactory.CreateClient("WebAPI");
                var jsonPayload = JsonSerializer.Serialize(newComment);
                var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

                var response = await httpClient.PostAsync("comment/create/", content);

                if (response.IsSuccessStatusCode)
                {
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error creating community: {ex.Message}");
                return true;
            }
        }
    }
}
