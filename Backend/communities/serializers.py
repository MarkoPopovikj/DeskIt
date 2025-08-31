from rest_framework import serializers

from communities.models import Community


class CommunitySerializer(serializers.ModelSerializer):
    author_id = serializers.IntegerField(source='author.id', read_only=True)

    class Meta:
        model = Community
        fields = ('id', 'author_id', 'topic', 'name', 'description', 'background_color', 'member_count')

class CreateCommunitySerializer(serializers.ModelSerializer):
    class Meta:
        model = Community
        fields = ('name', 'description', 'topic', 'background_color')

    def validate_name(self, value):
        if Community.objects.filter(name=value).exists():
            raise serializers.ValidationError("A restaurant with that name already exists!")
        return value

    def create(self, validated_data):
        name = validated_data.pop('name')
        description = validated_data.pop('description')
        topic = validated_data.pop('topic')
        background_color = validated_data.pop('background_color')
        author = self.context['request'].user
        member_count = 1

        community = Community.objects.create(
            name=name,
            description=description,
            topic=topic,
            author=author,
            background_color=background_color
        )

        return community

class UpdateCommunitySerializer(serializers.ModelSerializer):

    class Meta:
        model = Community
        fields = ('name', 'description', 'topic', 'background_color')

    def update(self, instance, validated_data):
        instance.name = validated_data.pop('name')
        instance.description = validated_data.pop('description')
        instance.topic = validated_data.pop('topic')
        instance.background_color = validated_data.pop('background_color')
        instance.save()

        return instance

