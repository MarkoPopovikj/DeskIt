from django.urls import path, include

from communities.views import GetCommunities, CreateCommunityView, GetTopicOptions

urlpatterns = [
    path('get/', GetCommunities.as_view(), name='get_communities'),
    # path('get_joined_communities/',),
    path('get_topics/', GetTopicOptions.as_view(), name='get_topics'),
    path('create/', CreateCommunityView.as_view(), name='create'),
    # path('update/',),
    # path('delete/',),
    #
    # path('join/'),
    # path('leave/',),
]