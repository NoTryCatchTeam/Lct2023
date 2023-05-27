# Установка серверной части
Для развертывания серверной части необходим сервер на базе Unix с установленным Docker или кластер Kubernetes  
Далее в примерах будет использоваться сервер Ubuntu 22.04 с установленным 

## Развертывание API
Скачайте образ с API из репозитория Docker Hub

`docker pull swamp1820/lct2023:latest`

Запустите образ на сервере  
``docker run --name api -d --restart always -p 8080:80 \  
                -e "DbConnectionString=${{ DB_CONNECTION_STRING }}" \
                -e "Jwt__AccessTokenExpiresInDays=${{ JWT_ACCESS_TOKEN_DAYS }}" \
                -e "Jwt__Audience=${{ JWT_AUDIENCE }}" \
                -e "Jwt__Issuer=${{ JWT_ISSUER }}" \
                -e "Jwt__RefreshTokenExpiresInDays=${{ JWT_REFRESH_TOKEN_DAYS }}" \
                -e "Jwt__Secret=${{ JWT_SECRET }}" \
                -e "Vk__ClientId=${{ CLIENT_ID }}" \
                -e "Vk__ClientSecret=${{ CLIENT_SECRET }}" \
                -e "Cms__ApiKey=${{ CMS_API_KEY }}" \
                swamp1820/lct2023 ``

Переменные и секреты  
- DbConnectionString - Строка подключение к базе данных `фывфыв`   
- Jwt__AccessTokenExpiresInDays -  Сколько дней активен токен доступа мобильного приложения `30`  
- Jwt__Audience  
- Jwt__Issuer  
- Jwt__RefreshTokenExpiresInDays  
- Jwt__Secret  
- Vk__ClientId  
- Vk__ClientSecret  
- Cms__ApiKey  
- -p 8080:80 - внешний порт для API  

После этого API будет доступна по порту 8080, а по ссылке /swagger/index.html будет доступна документация OpenAPI



## Развертывание CMS
Скачайте образ с CMS из репозитория Docker Hub

`docker pull swamp1820/lct2023-cms:latest`

Запустите образ на сервере  
``docker run --name cms -d --restart always -p 80:1337 \  
                -v /upload:/opt/app/public/uploads \  
                -e "ADMIN_JWT_SECRET=${{ ADMIN_JWT_SECRET }}" \
                -e "API_TOKEN_SALT=${{ API_TOKEN_SALT }}" \  
                -e "APP_KEYS=${{ APP_KEYS }}" \  
                -e "DATABASE_CLIENT=${{ DATABASE_CLIENT }}" \  
                -e "DATABASE_HOST=${{ DATABASE_HOST }}" \  
                -e "DATABASE_NAME=${{ DATABASE_NAME }}" \  
                -e "DATABASE_PASSWORD=${{ DATABASE_PASSWORD }}" \  
                -e "DATABASE_PORT=5432" \  
                -e "DATABASE_USERNAME=${{ DATABASE_USERNAME }}" \  
                -e "FORCE_MIGRATION=false" \  
                -e "JWT_SECRET=${{ JWT_SECRET }}" \  
                -e "SMTP_HOST=smtp.mail.ru" \  
                -e "SMTP_PASSWORD=${{ SMTP_PASSWORD }}" \  
                -e "SMTP_PORT=465" \
                -e "SMTP_USERNAME=${{ SMTP_USERNAME }}" \  
                swamp1820/lct2023-cms``

Переменные и секреты  
- -p 80:1337 - внешний порт для CMS  
- -v /upload:/opt/app/public/uploads  
- ADMIN_JWT_SECRET - Строка подключение к базе данных `фывфыв`   
- API_TOKEN_SALT -  Сколько дней активен токен доступа мобильного приложения `30`  
- APP_KEYS  
- DATABASE_CLIENT  
- DATABASE_HOST  
- DATABASE_NAME  
- DATABASE_PASSWORD  
- DATABASE_PORT
- DATABASE_USERNAME  
- CMS_API_KEY  

После этого CMS будет доступна по порту 80 по ссылке /admin

## Развертывания сервиса презентаций PDF

Скачайте образ с сервисом презентаций из репозитория Docker Hub

`docker pull swamp1820/lct2023-pdf:latest`

Запустите образ на сервере  
`docker run --name pdf -d --restart always -p 8081:80 \  
                -v /upload:/usr/share/nginx/html/files:ro \  
                swamp1820/lct2023-pdf `

Переменные и секреты  
- -p 8081:80 - внешний порт сервиса  
- -v /upload:/usr/share/nginx/html/files:ro  



---

# (Опционально) Сборка 

  Для сборки из исходных файлов необходимо клонировать репозиторий  
``git clone https://github.com/NoTryCatchTeam/Lct2023.git``
### Сборка API

