from rest_framework import status
from rest_framework.permissions import IsAuthenticated
from rest_framework.response import Response
from rest_framework.views import APIView

from communities.models import Community, TOPIC_CHOICES, CommunityMemberships
from communities.serializers import CommunitySerializer, CreateCommunitySerializer, UpdateCommunitySerializer
from users.models import User


# Create your views here.
class GetCommunities(APIView):
    permission_classes = [IsAuthenticated]

    def get(self, request):
        communities = Community.objects.all()
        serializer = CommunitySerializer(communities, many=True)

        return Response(serializer.data)

class GetTopicOptions(APIView):
    permission_classes = [IsAuthenticated]

    def get(self, request, format=None):
        choices_dict = [{'value': choice[0], 'label': choice[1]} for choice in TOPIC_CHOICES]
        return Response(choices_dict)

class CreateCommunityView(APIView):
    permission_classes = [IsAuthenticated]

    def post(self, request):
        serializer = CreateCommunitySerializer(data=request.data, context={'request': request})

        if serializer.is_valid():
            serializer.save()

            return Response({"message": "Community created successfully!"})

        return Response(serializer.errors, status=status.HTTP_400_BAD_REQUEST)

class GetMembershipsView(APIView):
    permission_classes = [IsAuthenticated]

    def get(self, request):
        user = request.user

        community_ids = CommunityMemberships.objects.filter(user=user).values_list('community_id', flat=True)

        return Response(list(community_ids))

class JoinCommunityView(APIView):
    permission_classes = [IsAuthenticated]

    def post(self, request, community_id):
        # Crash ako ne e so try
        try:
            community = Community.objects.get(id=community_id)
        except Community.DoesNotExist:
            return Response(
                {"error": "Community not found."},
                status=status.HTTP_404_NOT_FOUND
            )

        membership, created = CommunityMemberships.objects.get_or_create(
            user=request.user,
            community=community
        )

        if created:
            community.member_count += 1
            community.save()

            return Response(
                {"message": "Successfully joined the community!"},
                status=status.HTTP_201_CREATED
            )
        else:
            return Response(
                {"message": "You are already a member of this community."},
                status=status.HTTP_200_OK
            )

class LeaveCommunityView(APIView):
    permission_classes = [IsAuthenticated]

    def post(self, request, community_id):
        try:
            community = Community.objects.get(id=community_id)
        except Community.DoesNotExist:
            return Response(
                {"error": "Community not found."},
                status=status.HTTP_404_NOT_FOUND
            )

        membership = CommunityMemberships.objects.filter(
            user=request.user,
            community=community
        )

        if membership.exists():
            membership.delete()

            community.member_count -= 1
            community.save()

            return Response(
                {"message": "You have successfully left the community."},
                status=status.HTTP_200_OK
            )
        else:
            return Response(
                {"error": "You are not a member of this community."},
                status=status.HTTP_400_BAD_REQUEST
            )

class UpdateCommunityView(APIView):
    permission_classes = [IsAuthenticated]

    def put(self, request, community_id):
        try:
            community = Community.objects.get(id=community_id)
        except Community.DoesNotExist:
            return Response({"error": "Community not found."}, status = 404)

        serializer = UpdateCommunitySerializer(data=request.data, instance=community)

        if serializer.is_valid():
            serializer.save()

            return Response(serializer.data)

        return Response(serializer.errors, status=status.HTTP_400_BAD_REQUEST)
