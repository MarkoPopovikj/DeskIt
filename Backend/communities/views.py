from rest_framework import status
from rest_framework.permissions import IsAuthenticated
from rest_framework.response import Response
from rest_framework.views import APIView

from communities.models import Community, TOPIC_CHOICES
from communities.serializers import CommunitySerializer, CreateCommunitySerializer


# Create your views here.
class GetCommunities(APIView):
    permission_classes = [IsAuthenticated]

    def get(self):
        communities = Community.objects.all()
        serializer = CommunitySerializer(communities, many=True)

        if serializer.is_valid():
            return Response(serializer.data)

        return Response(serializer.errors, status=status.HTTP_400_BAD_REQUEST)

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
