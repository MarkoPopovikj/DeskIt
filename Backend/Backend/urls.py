# from django.contrib import admin
from django.urls import path, include

urlpatterns = [
    #    path('admin/', admin.site.urls),
    path('auth/', include('auth_form.urls')),
    path('user/', include('users.urls')),
    path('community/', include('communities.urls')),
]
