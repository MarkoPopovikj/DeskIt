from django.db import models
from django.utils import timezone

from users.models import User


# Create your models here.
TOPIC_CHOICES = [
    ('GAMING', 'Gaming & Entertainment'),
    ('TECH', 'Technology & Science'),
    ('SPORTS', 'Sports & Fitness'),
    ('MUSIC', 'Music & Arts'),
    ('NEWS', 'News & Politics'),
    ('TRAVEL', 'Travel & Holidays'),
    ('ANIMALS', 'Pets & Animals')
]

class Community(models.Model):
    created_at = models.DateTimeField(default=timezone.now, editable=False)

    topic = models.CharField(
        max_length=20,
        choices=TOPIC_CHOICES,
        default='GAMING'
    )
    name = models.CharField(max_length=30)
    author = models.ForeignKey(User, on_delete=models.CASCADE)

    description = models.TextField(max_length=200)

    background_color = models.CharField(max_length=6, default='F43F5E')

    member_count = models.IntegerField(default=1)

    def __str__(self):
        return f"r/{self.name}"

class CommunityMemberships(models.Model):
    user = models.ForeignKey(User, on_delete=models.CASCADE)

    community = models.ForeignKey(Community, on_delete=models.CASCADE)

    def __str__(self):
        return f"d/{self.community}"





