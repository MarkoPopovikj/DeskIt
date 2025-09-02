from rest_framework import serializers

from comments.models import Comment
from posts.models import Post


class CommentSerializer(serializers.ModelSerializer):
    author_name = serializers.CharField(source='author.username', read_only=True)

    class Meta:
        model = Comment
        fields = ('id', 'author_name', 'content', 'created_at', 'upvotes', 'downvotes')

class CreateCommentSerializer(serializers.ModelSerializer):
    post = serializers.PrimaryKeyRelatedField(queryset=Post.objects.all())

    class Meta:
        model = Comment
        fields = ['post', 'content']

    def create(self, validated_data):
        content = validated_data.pop('content')
        post = validated_data.pop('post')
        upvotes = 0
        downvotes = 0
        author = self.context['request'].user

        comment = Comment.objects.create(
            content=content,
            post=post,
            author=author,
            upvotes=upvotes,
            downvotes=downvotes,
        )

        return comment