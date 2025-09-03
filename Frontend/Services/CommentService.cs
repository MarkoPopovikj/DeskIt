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
using Frontend.Services;
using static Frontend.Components.Pages.Post;
using static Frontend.Components.Shared.CommentCard;

namespace Frontend.Services
{
    public class CommentVotesResponse
    {
        [JsonPropertyName("comment_id")]
        public int CommentId { get; set; }

        [JsonPropertyName("vote_type")]
        public int VoteValue { get; set; }
    }

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
        private readonly UserService _userService; // Add a UserService instance

        public Dictionary<CommentModel, UserModel> PostCommentsDictionary { get; set; }
        public Dictionary<int, int> UserCommentVotes { get; private set; }

        public CommentService(IHttpClientFactory httpClientFactory, UserService userService)
        {
            _httpClientFactory = httpClientFactory;
            _userService = userService; 

            PostCommentsDictionary = new Dictionary<CommentModel, UserModel>();
            UserCommentVotes = new Dictionary<int, int>();
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

                    PostCommentsDictionary.Clear();

                    List<string> AuthorNamesList = new List<string>();
                    List<CommentModel> CommentList = new List<CommentModel>();

                    foreach (CommentResponse c in responseContent)
                    {
                        CommentModel newComment = new CommentModel
                        {
                            Id = c.Id,
                            Content = c.Content,
                            CreatedAt = c.CreatedAt,
                            DownVotes = c.Downvotes,
                            UpVotes = c.Upvotes,
                            AuthorName = c.AuthorName,
                        };

                        CommentList.Add(newComment);
                        AuthorNamesList.Add(newComment.AuthorName);
                    }

                    if(AuthorNamesList.Count == 0)
                        return true;

                    List<UserModel> users = await _userService.GetBulkUsersAsync(AuthorNamesList);

                    if(users != null)
                    {
                        foreach(CommentModel c in CommentList)
                        {
                            foreach(UserModel u in users)
                            {
                                if (c.AuthorName == u.Username)
                                {
                                    PostCommentsDictionary.Add(c, u);
                                    break;
                                }
                            }
                        }
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

                    await GetUserCommentVotesAsync();

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

        public async Task<bool> UpdateCommentAsync(int CommentId, UpdateCommentModel newComment)
        {
            try
            {
                var httpClient = _httpClientFactory.CreateClient("WebAPI");
                var jsonPayload = JsonSerializer.Serialize(newComment);
                var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

                var response = await httpClient.PutAsync($"comment/{CommentId}/edit/", content);

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

        public async Task<bool> DeleteCommentAsync(int commentId)
        {
            try
            {
                var httpClient = _httpClientFactory.CreateClient("WebAPI");
                var response = await httpClient.DeleteAsync($"comment/{commentId}/delete/");

                if (response.IsSuccessStatusCode)
                {
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private async Task<bool> GetUserCommentVotesAsync()
        {
            try
            {
                var httpClient = _httpClientFactory.CreateClient("WebAPI");
                var response = await httpClient.GetAsync("comment/get_user_votes/");

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadFromJsonAsync<List<CommentVotesResponse>>();

                    if (responseContent != null)
                    {
                        UserCommentVotes = responseContent.ToDictionary(vote => vote.CommentId, vote => vote.VoteValue);
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

        public async Task<bool> UpdateCommentUpvotesDownvotesAsync(int commentId, int action)
        {
            UserCommentVotes[commentId] = action;

            try
            {
                var httpClient = _httpClientFactory.CreateClient("WebAPI");
                var response = await httpClient.PostAsync($"comment/{commentId}/{action.ToString()}/vote/", null);

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
    }
}
