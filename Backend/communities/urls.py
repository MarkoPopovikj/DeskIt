from django.urls import path, include

from communities.views import GetCommunities, CreateCommunityView, GetTopicOptions, JoinCommunityView, \
    LeaveCommunityView, GetMembershipsView, UpdateCommunityView

urlpatterns = [
    path('get/', GetCommunities.as_view(), name='get_communities'),
    # path('get_joined_communities/',),
    path('get_topics/', GetTopicOptions.as_view(), name='get_topics'),
    path('create/', CreateCommunityView.as_view(), name='create'),
    path('<int:community_id>/update/', UpdateCommunityView.as_view(), name='update'),
    # path('delete/',),

    path('get_memberships/', GetMembershipsView.as_view(), name='get_memberships'),
    path('<int:community_id>/join/', JoinCommunityView.as_view(), name='join'),
    path('<int:community_id>/leave/', LeaveCommunityView.as_view(), name='leave'),
]