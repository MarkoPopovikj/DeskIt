from django.db import models

from posts.models import Post
from users.models import User


# Create your models here.
class Comment(models.Model):
    post = models.ForeignKey(Post, on_delete=models.CASCADE)

    author = models.ForeignKey(User, on_delete=models.CASCADE)
    content = models.CharField(max_length=200)
    created_at = models.DateTimeField(auto_now_add=True, editable=False)

    upvotes = models.IntegerField(default=0)
    downvotes = models.IntegerField(default=0)

    def __str__(self):
        return self.content

class CommentVote(models.Model):
    comment = models.ForeignKey(Comment, on_delete=models.CASCADE)
    voter = models.ForeignKey(User, on_delete=models.CASCADE)

    vote_type = models.IntegerField(default=0) # 0 = nisto, 1 = upvote, -1 = downvote

    class Meta:
        unique_together = ('comment', 'voter')

    def __str__(self):
        return self.comment.content