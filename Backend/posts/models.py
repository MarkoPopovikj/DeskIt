from django.db import models

from communities.models import Community
from users.models import User


# Create your models here.
class Post(models.Model):
    community = models.ForeignKey(
        Community,
        on_delete=models.CASCADE,
        related_name='community'
    )
    author = models.ForeignKey(
        User,
        on_delete=models.CASCADE,
        related_name='author'
    )

    title = models.CharField(max_length=170)
    content = models.TextField(blank=True)

    upvotes = models.IntegerField(default=0)
    downvotes = models.IntegerField(default=0)
    comments_count = models.IntegerField(default=0)

    image_url = models.CharField(max_length=20, blank=True)

    created_at = models.DateTimeField(auto_now_add=True, editable=False)

    def __str__(self):
        return self.title

class PostVote(models.Model):
    post = models.ForeignKey(Post, on_delete=models.CASCADE)
    voter = models.ForeignKey(User, on_delete=models.CASCADE)

    vote_type = models.IntegerField(default=0) # 0 = nisto, 1 = upvote, -1 = downvote

    class Meta:
        unique_together = ('post', 'voter')

    def __str__(self):
        return self.post.title