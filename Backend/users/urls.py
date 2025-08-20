from django.urls import path

from users.views import GetUserView

urlpatterns = [
    path('get_user/', GetUserView.as_view(), name="get_user"),
    # path('update_simple_data/',),
    # path('update_password', )
]