from django.urls import path

from users.views import GetUserView, UpdateSimpleData

urlpatterns = [
    path('get_user/', GetUserView.as_view(), name="get_user"),
    path('update_simple_data/', UpdateSimpleData.as_view(), name="update_simple_data"),
    # path('update_password', )
]