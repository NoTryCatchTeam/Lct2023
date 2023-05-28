FROM python:latest as build
RUN pip install mkdocs
COPY ./docs/ ./docs/
WORKDIR /docs
RUN mkdocs build

FROM nginx:latest
COPY --from=build ./docs/site/ /usr/share/nginx/html/