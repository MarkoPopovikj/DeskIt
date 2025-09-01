from rest_framework import serializers

from users.models import User


class UserSerializer(serializers.ModelSerializer):
    class Meta:
        model = User
        fields = ('id', 'username', 'email', 'background_color', 'about_me', 'created_at')

class UserSimpleDataSerializer(serializers.ModelSerializer):
    class Meta:
        model = User
        fields = ('username', 'email', 'background_color', 'about_me')

    def update(self, instance, validated_data):
        instance.username = validated_data.get('username', instance.username)
        instance.email = validated_data.get('email', instance.email)
        instance.about_me = validated_data.get('about_me', instance.about_me)
        instance.background_color = validated_data.get('background_color', instance.background_color)
        instance.save()

        return instance

class OtherUserSerializer(serializers.ModelSerializer):
    class Meta:
        model = User
        fields = ('id', 'username', 'background_color', 'about_me', 'created_at')
