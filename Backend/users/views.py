from rest_framework import status
from rest_framework.permissions import IsAuthenticated
from rest_framework.response import Response
from rest_framework.views import APIView
from rest_framework_simplejwt.tokens import RefreshToken

from users.models import User
from users.serializers import UserSerializer, UserSimpleDataSerializer, OtherUserSerializer


# Get
class GetUserView(APIView):
    permission_classes = [IsAuthenticated]

    def get(self, request):
        serializer = UserSerializer(request.user)
        return Response(serializer.data)

class GetOtherUserView(APIView):
    permission_classes = [IsAuthenticated]

    def get(self, request, username):
        user = User.objects.get(username=username)
        serializer = OtherUserSerializer(user)

        return Response(serializer.data)

class GetBulkUsersView(APIView):
    permission_classes = [IsAuthenticated]

    def post(self, request):
        usernames = request.data.get('usernames', [])

        users = User.objects.filter(username__in=usernames)
        serializer = OtherUserSerializer(users, many=True)

        return Response(serializer.data)

#Crud
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