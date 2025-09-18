FROM nginx:alpine

# Copy static site into nginx html directory
COPY frontend/ /usr/share/nginx/html/frontend/
COPY icons/ /usr/share/nginx/html/icons/

# Provide custom nginx config
COPY frontend/nginx.conf /etc/nginx/conf.d/default.conf

EXPOSE 80



