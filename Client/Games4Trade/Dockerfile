# build stage
FROM node:24-alpine AS build-stage
WORKDIR /app
ARG VITE_API_URL
ARG VITE_MESSAGE_HUB_URL
ENV VITE_API_URL=${VITE_API_URL}
ENV VITE_MESSAGE_HUB_URL=${VITE_MESSAGE_HUB_URL}
COPY package*.json ./
RUN npm ci
COPY . .
RUN npm run build

# production stage
FROM nginx:stable-alpine AS production-stage
COPY --from=build-stage /app/dist /usr/share/nginx/html
COPY nginx.conf /etc/nginx/conf.d/default.conf
EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]
