from django.urls import path, include

from posts.views import GetAllPostsView, CreatePostView, GetUserPostsView, GetCommunityPostsView

urlpatterns = [
    #Get
    path('get_all/', GetAllPostsView.as_view(), name='get_all_posts'),
    path('<int:user_id>/get_user/', GetUserPostsView.as_view(), name='get_user_posts'),
    path('<int:community_id>/get_community/', GetCommunityPostsView.as_view(), name='get_community_posts'),

    #Crud
    path('create/', CreatePostView.as_view(), name='create_post'),
    # path('<int:post_id>/delete/',)
]