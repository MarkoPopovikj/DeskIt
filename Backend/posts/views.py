from django.shortcuts import render, get_object_or_404
from rest_framework import status
from rest_framework.permissions import IsAuthenticated
from rest_framework.response import Response
from rest_framework.views import APIView

import posts
from communities.models import Community
from posts.models import Post, PostVote
from posts.serializers import PostSerializer, CreatePostSerializer
from users.models import User


# GET
class GetAllPostsView(APIView):
    permission_classes = [IsAuthenticated]

    def get(self, request):
        posts = Post.objects.all().order_by('-created_at')
        serializer = PostSerializer(posts, many=True)

        return Response(serializer.data)

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

#CRUD
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

#VOTE
class GetVotePostsView(APIView):
    permission_classes = [IsAuthenticated]

    def get(self, request):
        user = request.user
        post_votes = PostVote.objects.filter(voter = user).values('post_id', 'vote_type')

        return Response(post_votes)

class VotePostView(APIView):
    permission_classes = [IsAuthenticated]

    def post(self, request, post_id, action):
        post = get_object_or_404(Post, id=post_id)
        author = User.objects.get(id=post.author.id)
        user = request.user

        post_vote = PostVote.objects.filter(post=post, voter=user)

        action = int(action)

        if not post_vote.exists():
            post_vote = PostVote.objects.create(
                post=post,
                voter=user,
                vote_type=action
            )

            if action == 1:
                post.upvotes += 1
            else:
                post.downvotes += 1

            author.karma += 1

            author.save()
            post_vote.save()
            post.save()

            return Response({"success": True}, status=status.HTTP_200_OK)
        else:
            post_vote = PostVote.objects.get(post=post, voter=user)

            if post_vote.vote_type == 1: # bilo upvoted
                if action == 1: # sega pak = go vadi upvote
                    post.upvotes -= 1

                    post_vote.vote_type = 0
                elif action == -1:  # klika downvote = upvote -= 1, downvote += 1
                    post.downvotes += 1
                    post.upvotes -= 1

                    post_vote.vote_type = action

            elif post_vote.vote_type == -1: # bilo downvoted
                if action == -1: #pak downvote = go trga
                    post.downvotes -= 1

                    post_vote.vote_type = 0
                elif action == 1: #upvote = downvote -= 1, upvote += 1
                    post.downvotes -= 1
                    post.upvotes += 1

                    post_vote.vote_type = action

            else: # 0
                if action == 1:
                    post.upvotes += 1
                    post_vote.vote_type = 1

                else:
                    post.downvotes += 1
                    post_vote.vote_type = -1

            user.save()
            post_vote.save()
            post.save()

            return Response({"success": True}, status=status.HTTP_200_OK)
