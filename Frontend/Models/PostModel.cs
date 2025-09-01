using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frontend.Models
{
    public class PostModel
    {
        public int Id { get; set; }
        public required string CommunityName {  get; set; }
        public required string AuthorName { get; set; }
        public required string Title { get; set; }
        public required string Content { get; set; }
        public required DateTime CreatedAt { get; set; }
        public int UpVotes { get; set; }
        public int DownVotes { get; set; }
        public int CommentsCount { get; set; }
        public string? ImageUrl { get; set; }

        public string FormattedUpvotes
        {
            get
            {
                if (UpVotes >= 1000)
                {
                    return $"{((double)UpVotes / 1000):0.0}k";
                }
                return UpVotes.ToString();
            }
        }

        public string TimeAgo
        {
            get
            {
                var timespan = DateTime.Now - CreatedAt;
                if (timespan.TotalMinutes < 60) return $"{(int)timespan.TotalMinutes} minutes ago";
                if (timespan.TotalHours < 24) return $"{(int)timespan.TotalHours} hours ago";
                return $"{(int)timespan.TotalDays} days ago";
            }
        }

    }
}
