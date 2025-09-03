from django.urls import path, include

from posts.views import GetAllPostsView, CreatePostView, GetUserPostsView, GetCommunityPostsView, DeletePostView, \
    GetPostView, VotePostView, GetVotePostsView, UpdatePostView

urlpatterns = [
    #Get
    path('get_all/', GetAllPostsView.as_view(), name='get_all_posts'),
    path('<int:user_id>/get_user/', GetUserPostsView.as_view(), name='get_user_posts'),
    path('<int:community_id>/get_community/', GetCommunityPostsView.as_view(), name='get_community_posts'),
    path('<int:post_id>/get/', GetPostView.as_view(), name='get'),

    #Crud
    path('create/', CreatePostView.as_view(), name='create_post'),
    path('<int:post_id>/delete/', DeletePostView.as_view(), name='delete_post'),
    path('<int:post_id>/update/', UpdatePostView.as_view(), name='update_post'),

    #Upvote - downvote
    path('get_user_votes/', GetVotePostsView.as_view(), name='get_user_votes'),
    path('<int:post_id>/<str:action>/vote/', VotePostView.as_view(), name='upvote_post'),
]