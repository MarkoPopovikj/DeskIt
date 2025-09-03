from django.shortcuts import render, get_object_or_404
from rest_framework import status
from rest_framework.permissions import IsAuthenticated
from rest_framework.response import Response
from rest_framework.views import APIView

from comments.models import Comment, CommentVote
from comments.serializers import CommentSerializer, CreateCommentSerializer, UpdateCommentSerializer
from posts.models import Post
from users.models import User


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

class UpdateCommentView(APIView):
    permission_classes = [IsAuthenticated]

    def put(self, request, comment_id):
        comment = get_object_or_404(Comment, id=comment_id)

        serializer = UpdateCommentSerializer(data=request.data, instance=comment)

        if serializer.is_valid():
            serializer.save()

            return Response(serializer.data)

        return Response(serializer.errors, status=status.HTTP_400_BAD_REQUEST)

class DeleteCommentView(APIView):
    permission_classes = [IsAuthenticated]

    def delete(self, request, comment_id):
        comment = get_object_or_404(Comment, id=comment_id)
        comment.delete()

        return Response(status=status.HTTP_204_NO_CONTENT)

class GetVoteCommentView(APIView):
    permission_classes = [IsAuthenticated]

    def get(self, request):
        user = request.user
        comment_votes = CommentVote.objects.filter(voter = user).values('comment_id', 'vote_type')

        return Response(comment_votes)

class VoteCommentView(APIView):
    permission_classes = [IsAuthenticated]

    def post(self, request, comment_id, action):
        comment = get_object_or_404(Comment, id=comment_id)
        author = User.objects.get(id=comment.author.id)
        user = request.user

        comment_vote = CommentVote.objects.filter(comment=comment, voter=user)

        action = int(action)

        if not comment_vote.exists():
            comment_vote = CommentVote.objects.create(
                comment=comment,
                voter=user,
                vote_type=action
            )

            if action == 1:
                comment.upvotes += 1
            else:
                comment.downvotes += 1

            author.karma += 1

            author.save()
            comment_vote.save()
            comment.save()

            return Response({"success": True}, status=status.HTTP_200_OK)
        else:
            comment_vote = CommentVote.objects.get(comment=comment, voter=user)

            if comment_vote.vote_type == 1: # bilo upvoted
                if action == 1: # sega pak = go vadi upvote
                    comment.upvotes -= 1

                    comment_vote.vote_type = 0
                elif action == -1:  # klika downvote = upvote -= 1, downvote += 1
                    comment.downvotes += 1
                    comment.upvotes -= 1

                    comment_vote.vote_type = action

            elif comment_vote.vote_type == -1: # bilo downvoted
                if action == -1: #pak downvote = go trga
                    comment.downvotes -= 1

                    comment_vote.vote_type = 0
                elif action == 1: #upvote = downvote -= 1, upvote += 1
                    comment.downvotes -= 1
                    comment.upvotes += 1

                    comment_vote.vote_type = action

            else: # 0
                if action == 1:
                    comment.upvotes += 1
                    comment_vote.vote_type = 1

                else:
                    comment.downvotes += 1
                    comment_vote.vote_type = -1

            user.save()
            comment_vote.save()
            comment.save()

            return Response({"success": True}, status=status.HTTP_200_OK)

