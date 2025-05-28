
# Build stage
FROM node:20 AS build
WORKDIR /app
COPY package*.json ./
RUN npm install
COPY . .
RUN npm run build --prod


# Service stage (with Nginx)
FROM nginx:alpine
COPY --from=build /app/dist/text-captcha /usr/share/nginx/html
COPY ./nginx.conf /etc/nginx/conf.d/default.conf
EXPOSE 8080
CMD [ "nginx", "-g", "daemon off;" ]

