from rest_framework import status
from rest_framework.permissions import IsAuthenticated
from rest_framework.response import Response
from rest_framework.views import APIView

from users.serializers import UserSerializer, UserSimpleDataSerializer


# Create your views here.
class GetUserView(APIView):
    permission_classes = [IsAuthenticated]

    def get(self, request):
        serializer = UserSerializer(request.user)
        return Response(serializer.data)

class UpdateSimpleData(APIView):
    permission_classes = [IsAuthenticated]

    def put(self, request):
        serializer = UserSimpleDataSerializer(instance=request.user, data=request.data, partial=True)

        if serializer.is_valid():
            user = serializer.save()

            return Response({"message": "User updated successfully!"})

        return Response(serializer.errors, status=status.HTTP_400_BAD_REQUEST)