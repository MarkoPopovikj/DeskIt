from django.urls import path

from users.views import GetUserView, UpdateSimpleDataView, UpdateUserPasswordView

urlpatterns = [
    path('get_user/', GetUserView.as_view(), name="get_user"),
    path('update_simple_data/', UpdateSimpleDataView.as_view(), name="update_simple_data"),
    path('update_password/', UpdateUserPasswordView.as_view(), name="update_password"),
]