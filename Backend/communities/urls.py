from django.urls import path, include

from communities.views import GetSimpleCommunitiesView, CreateCommunityView, GetTopicOptions, JoinCommunityView, \
    LeaveCommunityView, GetMembershipsView, UpdateCommunityView, DeleteCommunityView, GetUserSimpleCommunitiesView, \
    GetDetailedCommunitiView

urlpatterns = [
    #Get
    path('get_topics/', GetTopicOptions.as_view(), name='get_topics'),
    path('get_simple/', GetSimpleCommunitiesView.as_view(), name='get_communities'),
    path('<int:user_id>/get_user_simple/', GetUserSimpleCommunitiesView.as_view(), name='get_other_communities'),
    path('<int:community_id>/get_detailed/', GetDetailedCommunitiView.as_view(), name='get_detailed'),

    #Crud
    path('create/', CreateCommunityView.as_view(), name='create'),
    path('<int:community_id>/update/', UpdateCommunityView.as_view(), name='update'),
    path('<int:community_id>/delete/', DeleteCommunityView.as_view(), name='delete'),

    #Memberships
    path('get_memberships/', GetMembershipsView.as_view(), name='get_memberships'),
    path('<int:community_id>/join/', JoinCommunityView.as_view(), name='join'),
    path('<int:community_id>/leave/', LeaveCommunityView.as_view(), name='leave'),


]