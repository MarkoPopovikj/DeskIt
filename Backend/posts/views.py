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


