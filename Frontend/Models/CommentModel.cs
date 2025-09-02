using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frontend.Models
{
    public class CommentModel
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public string AuthorName { get; set; }
        public int UpVotes { get; set; }
        public int DownVotes { get; set; }

        public string FormattedUpvotes
        {
            get
            {
                int number = UpVotes - DownVotes;
                if (number >= 1000)
                {
                    return $"{((double)number / 1000):0.0}k";
                }
                return number.ToString();
            }
        }

        public int VoteScore => UpVotes - DownVotes;

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
