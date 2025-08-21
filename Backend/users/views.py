from rest_framework import status
from rest_framework.permissions import IsAuthenticated
from rest_framework.response import Response
from rest_framework.views import APIView
from rest_framework_simplejwt.tokens import RefreshToken

from users.serializers import UserSerializer, UserSimpleDataSerializer


# Create your views here.
class GetUserView(APIView):
    permission_classes = [IsAuthenticated]

    def get(self, request):
        serializer = UserSerializer(request.user)
        return Response(serializer.data)

class UpdateSimpleDataView(APIView):
    permission_classes = [IsAuthenticated]

    def put(self, request):
        serializer = UserSimpleDataSerializer(instance=request.user, data=request.data, partial=True)

        if serializer.is_valid():
            serializer.save()

            return Response({"message": "User updated successfully!"})

        return Response(serializer.errors, status=status.HTTP_400_BAD_REQUEST)

class UpdateUserPasswordView(APIView):
    permission_classes = [IsAuthenticated]

    def put(self, request):
        user = request.user
        user.set_password(request.data['password'])
        user.save()

        if user is None:
            return Response({"message": "User not found"})

        refresh = RefreshToken.for_user(user)
        access_token = str(refresh.access_token)

        return Response({
            "message": "User updated successfully!",
            "access_token": access_token
        })