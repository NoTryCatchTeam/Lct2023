# Установка серверной части
Для развертывания серверной части необходим сервер на базе Unix с установленным Docker или кластер [Kubernetes](https://kubernetes.io/ru/).  
Далее в примерах будет использоваться сервер Ubuntu 22.04 с установленным [Docker Engine](https://docs.docker.com/engine/install/ubuntu/).  
Каждый образ указанный на этой страницу можно собрать самостоятельно из [репозитория](https://github.com/NoTryCatchTeam/Lct2023)    
` 
git clone https://github.com/NoTryCatchTeam/Lct2023.git
` 
и воспользовавшись командой [docker build](https://docs.docker.com/engine/reference/commandline/build/).  
Развернуть API Gateway можно по [спецификации](https://github.com/NoTryCatchTeam/Lct2023/blob/master/deployment/api-gateway.json)  

## Развертывание API
Скачайте образ с API из репозитория [Docker Hub](https://hub.docker.com/)
```  
docker pull swamp1820/lct2023:latest
```  
Запустите образ на сервере  
```
docker run --name api -d --restart always -p 8080:80 \  
                -e "DbConnectionString=${{ DB_CONNECTION_STRING }}" \
                -e "Jwt__AccessTokenExpiresInDays=${{ JWT_ACCESS_TOKEN_DAYS }}" \
                -e "Jwt__Audience=${{ JWT_AUDIENCE }}" \
                -e "Jwt__Issuer=${{ JWT_ISSUER }}" \
                -e "Jwt__RefreshTokenExpiresInDays=${{ JWT_REFRESH_TOKEN_DAYS }}" \
                -e "Jwt__Secret=${{ JWT_SECRET }}" \
                -e "Vk__ClientId=${{ CLIENT_ID }}" \
                -e "Vk__ClientSecret=${{ CLIENT_SECRET }}" \
                -e "Cms__ApiKey=${{ CMS_API_KEY }}" \
                swamp1820/lct2023 
```                

Переменные и секреты  
- DbConnectionString - Строка подключение к базе данных `User ID=root;Password=myPassword;Host=localhost;Port=5432;Database=myDataBase;`   
- Jwt__AccessTokenExpiresInDays -  Сколько дней активен токен доступа мобильного приложения `30`  
- Jwt__Audience - имя аудиенции jwt
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
```
docker pull swamp1820/lct2023-cms:latest
```
Запустите образ на сервере  
```
docker run --name cms -d --restart always -p 80:1337 \  
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
                swamp1820/lct2023-cms
```
Переменные и секреты  
- -p 80:1337 - внешний порт для CMS  
- -v /upload:/opt/app/public/uploads - локальный диск для хранения данных (если не используется внешнее хранилище, например S3)  
- ADMIN_JWT_SECRET    
- API_TOKEN_SALT -  Сид-фраза для генерации API ключей `123абв456`
- APP_KEYS  
- DATABASE_CLIENT - тип СУБД `postgres`  
- DATABASE_HOST - адрес сервера БД  
- DATABASE_NAME - имя БД  
- DATABASE_PASSWORD - пароль пользователя БД  
- DATABASE_PORT - порт БД  
- DATABASE_USERNAME - логин пользователя БД  
- FORCE_MIGRATION - удаляет таблицы БД для которых не созданы каталоги `false`  
- JWT_SECRET  
- SMTP_HOST - адрес сервера SMTP для почтовой рассылки   
- SMTP_PASSWORD - пароль пользователя для почтовой рассылки  
- SMTP_PORT - порт сервера SMTP для почтовой рассылки  
- SMTP_USERNAME - логин пользователя для почтовой рассылки  

После этого CMS будет доступна по порту 80 по ссылке /admin

## Развертывания сервиса презентаций PDF

Скачайте образ с сервисом презентаций из репозитория Docker Hub
```
docker pull swamp1820/lct2023-pdf:latest
```
Запустите образ на сервере  

```
docker run --name pdf -d --restart always -p 8081:80 \  
                -v /upload:/usr/share/nginx/html/files:ro \  
                swamp1820/lct2023-pdf 
```

Переменные и секреты  
- -p 8081:80 - внешний порт сервиса  
- -v /upload:/usr/share/nginx/html/files:ro - локальный диск для хранения данных (если не используется внешнее хранилище, например S3)

---


