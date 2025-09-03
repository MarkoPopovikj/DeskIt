from django.urls import path, include

from comments.views import GetPostCommentsView, CreateCommentView, UpdateCommentView, DeleteCommentView, \
    GetVoteCommentView, VoteCommentView

urlpatterns = [
    # Get
    path('<int:post_id>/get/', GetPostCommentsView.as_view(), name='get_comments'),

    # Crud
    path('create/', CreateCommentView.as_view(), name='create_comment'),
    path('<int:comment_id>/edit/', UpdateCommentView.as_view(), name='edit_comment'),
    path('<int:comment_id>/delete/', DeleteCommentView.as_view(), name='delete_comment'),

    # Votes
    # #Upvote - downvote
    path('get_user_votes/', GetVoteCommentView.as_view(), name='get_user_comment_votes'),
    path('<int:comment_id>/<str:action>/vote/', VoteCommentView.as_view(), name='vote_comment'),
]