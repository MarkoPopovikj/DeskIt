from rest_framework import serializers

from communities.models import Community


class CommunitySerializer(serializers.ModelSerializer):
    author_id = serializers.IntegerField(source='author.id', read_only=True)

    class Meta:
        model = Community
        fields = ('id', 'author_id', 'topic', 'name', 'description')

class CreateCommunitySerializer(serializers.ModelSerializer):
    class Meta:
        model = Community
        fields = ('name', 'description', 'topic', 'author')

    def validate_name(self, value):
        if Community.objects.filter(name=value).exists():
            raise serializers.ValidationError("A restaurant with that name already exists!")
        return value

    def create(self, validated_data):
        name = validated_data.pop('name')
        description = validated_data.pop('description')
        topic = validated_data.pop('topic')
        author = self.context['request'].user

        community = Community.objects.create(name=name, description=description, topic=topic, author=author)

        return community