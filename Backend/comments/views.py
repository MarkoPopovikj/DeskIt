from django.shortcuts import render, get_object_or_404
from rest_framework import status
from rest_framework.permissions import IsAuthenticated
from rest_framework.response import Response
from rest_framework.views import APIView

from comments.models import Comment
from comments.serializers import CommentSerializer, CreateCommentSerializer
from posts.models import Post


# Create your views here.
class GetPostCommentsView(APIView):
    permission_classes = [IsAuthenticated]

    def get(self, request, post_id):
        post = get_object_or_404(Post, id=post_id)

        comments = Comment.objects.filter(post = post)
        serializer = CommentSerializer(comments, many=True)

        return Response(serializer.data)

class CreateCommentView(APIView):
    permission_classes = [IsAuthenticated]

    def post(self, request):
        serializer = CreateCommentSerializer(data=request.data, context={'request': request})

        if serializer.is_valid():
            serializer.save()
            return Response(serializer.data, status=status.HTTP_201_CREATED)

        return Response(serializer.errors, status=status.HTTP_400_BAD_REQUEST)