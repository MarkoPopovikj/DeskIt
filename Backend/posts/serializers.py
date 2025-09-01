from datetime import timezone

from rest_framework import serializers

from communities.models import Community
from posts.models import Post
from users.models import User


class PostSerializer(serializers.ModelSerializer):
    community_name = serializers.CharField(source='community.name', read_only=True)
    author_name = serializers.CharField(source='author.username', read_only=True)

    class Meta:
        model = Post
        fields = ('id', 'community_name', 'author_name', 'title', 'content', 'created_at', 'upvotes', 'downvotes', 'comments_count', 'image_url')

class CreatePostSerializer(serializers.ModelSerializer):
    community = serializers.PrimaryKeyRelatedField(queryset=Community.objects.all())

    class Meta:
        model = Post
        fields = ['community', 'title', 'content', 'image_url']

    def create(self, validated_data):
        title = validated_data.pop('title')
        content = validated_data.pop('content')
        image_url = validated_data.pop('image_url')
        upvotes = 0
        downvotes = 0
        comments_count = 0
        community = validated_data.pop('community')

        author = self.context['request'].user

        post = Post.objects.create(
            title=title,
            content=content,
            community=community,
            author=author,
            upvotes=upvotes,
            downvotes=downvotes,
            comments_count=comments_count,
            image_url=image_url
        )

        return post