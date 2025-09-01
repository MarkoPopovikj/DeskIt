from django.shortcuts import render
from rest_framework import status
from rest_framework.permissions import IsAuthenticated
from rest_framework.response import Response
from rest_framework.views import APIView

import posts
from communities.models import Community
from posts.models import Post
from posts.serializers import PostSerializer, CreatePostSerializer
from users.models import User


# Create your views here.
class GetAllPostsView(APIView):
    permission_classes = [IsAuthenticated]

    def get(self, request):
        posts = Post.objects.all()
        serializer = PostSerializer(posts, many=True)

        return Response(serializer.data)

class CreatePostView(APIView):
    permission_classes = [IsAuthenticated]

    def post(self, request):
        serializer = CreatePostSerializer(data=request.data, context={'request': request})

        if serializer.is_valid():
            serializer.save()
            return Response(serializer.data, status=status.HTTP_201_CREATED)

        return Response(serializer.errors, status=status.HTTP_400_BAD_REQUEST)

class DeletePostView(APIView):
    permission_classes = [IsAuthenticated]

    def delete(self, request, post_id):
        try:
            post = Post.objects.get(id=post_id)
        except Community.DoesNotExist:
            return Response({"error": "Community not found."}, status=status.HTTP_404_NOT_FOUND)

        if request.user != post.author:
            return Response(
                {"error": "You do not have permission to delete this community."},
                status=status.HTTP_403_FORBIDDEN
            )

        post.delete()

        return Response(status=status.HTTP_204_NO_CONTENT)

class GetPostView(APIView):
    permission_classes = [IsAuthenticated]

    def get(self, request, post_id):
        post = Post.objects.get(id=post_id)
        serializer = PostSerializer(post)

        return Response(serializer.data)

class GetUserPostsView(APIView):
    permission_classes = [IsAuthenticated]

    def get(self, request, user_id):
        user = User.objects.get(id=user_id)
        posts = Post.objects.filter(author = user)

        serializer = PostSerializer(posts, many=True)

        return Response(serializer.data)

class GetCommunityPostsView(APIView):
    permission_classes = [IsAuthenticated]

    def get(self, request, community_id):
        community = Community.objects.get(id=community_id)
        posts = Post.objects.filter(community = community)

        serializer = PostSerializer(posts, many=True)

        return Response(serializer.data)

class UpvoteDownvotePostView(APIView):
    permission_classes = [IsAuthenticated]

    def put(self, request, post_id, upvote, downvote):
        post = Post.objects.get(id=post_id)
        user = User.objects.get(id=post.author.id)

        post.upvotes = upvote
        post.downvotes = downvote

        user.karma += 1

        user.save()
        post.save()

        return Response(status=status.HTTP_204_NO_CONTENT)

