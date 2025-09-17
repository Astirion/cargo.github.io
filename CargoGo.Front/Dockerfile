FROM nginx:alpine

# Copy static site into nginx html directory
COPY . /usr/share/nginx/html/frontend/
COPY ../icons/ /usr/share/nginx/html/icons/

# Provide custom nginx config
COPY nginx.conf /etc/nginx/conf.d/default.conf

EXPOSE 80


