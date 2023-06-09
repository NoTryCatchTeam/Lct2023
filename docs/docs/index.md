 
Это документация к прототипу решения задачи "Мобильное приложение  для образования в сфере искусства" хакатона "Лидеры цифровой трансформации" 2023

*Краткое руководство* описано чуть ниже  
*Контент-менеджерам* и *администраторам* будет интересен раздел [Руководство](manual.md)  
Для *технических специалистов* представлен раздел [Техническая документация](tech.md)  

# Презентация решения
С презентацией решения можно ознакомиться [здесь](https://disk.yandex.ru/d/cgqwC5OehWgy3g)  
APK, дамп БД и учетные записи для системы управления контентом вместе с [презентацией](https://disk.yandex.ru/d/cgqwC5OehWgy3g)

# Краткое руководство пользователя

## Вход и регистрация
При первом запуске приложения вам необходимо зарегистрироваться
### Регистрация через почту  
- укажите почту и придумайте пароль  
<img src="pictures/screenshots/singup1.png" width="200">  
- заполните имя, фамилию и дату рождения  
<img src="pictures/screenshots/singup2.png" width="200">  
- нажмите "Регистрация"
### Вход через почту
- введите почту и пароль, с которыми зарегистрировались ранее  
<img src="pictures/screenshots/signin.png" width="200">  
- нажмите "Войти"  

### Вход через ВКонтакте
- нажмите "Войти с помощью VK"   
<img src="pictures/screenshots/signinempty.png" width="200">  
- войдите в учетную запись VK, а затем нажмите "Войти через VK ID"  
<img src="pictures/screenshots/signinvk.png" width="200">  


## Прохождение заданий
- перейдите в раздел "Задания" в меню приложения и выберите задание  
<img src="pictures/screenshots/tasks.png" width="200">  
- в текстовом задании необходимо выбрать правильный вариант из 4х предложенных  
<img src="pictures/screenshots/task1.png" width="200"> 
- правильный ответ будет подсвечен зеленым цветом  
<img src="pictures/screenshots/task2.png" width="200">  
- если ответ неправильный - ваш выбор будет подсвечен красным цветом  
<img src="pictures/screenshots/task3.png" width="200">  


- в заданиях с аудио ответами необходимо выбрать подходящее аудио к предложенному видео, чтобы обозначить ответ необходимо нажать кнопку "Выбрать" во время проигрывания аудио   
<img src="pictures/screenshots/task4.png" width="200">   

## Карта школ и мероприятий
- перейдите в раздел "Карта" в меню приложения  
<img src="pictures/screenshots/mapmas.png" width="200">  
- воспользуйтесь фильтром для поиска направления или воспользуйтесь полем "поиск"  
<img src="pictures/screenshots/mapfilter1.png" width="200">
<img src="pictures/screenshots/mapfilter2.png" width="200">
<img src="pictures/screenshots/search.png" width="200">  
- выберите и нажмите на точку, чтобы посмотреть информацию об учебном заведении  
<img src="pictures/screenshots/school1.png" width="200"> 


- на карте мероприятий воспользуйтесь фильтром для поиска направлений/района или воспользуйтесь полем "поиск"  
<img src="pictures/screenshots/mapfilter1.png" width="200">
<img src="pictures/screenshots/mapfilter2.png" width="200">
<img src="pictures/screenshots/eventsearch.png" width="200">  
- выберите мероприятие и прокрутите вниз для покупки билета  
<img src="pictures/screenshots/buyticket.png" width="200">  


# О приложении

[Репозиторий](https://github.com/NoTryCatchTeam/Lct2023)  

Мобильное приложение выполнено на базе [Xamatin.Native](https://dotnet.microsoft.com/en-us/apps/xamarin) для платформы Android.   
Серверная часть представлена несколькими компонентами:  
- API, на базе [.NET](https://dotnet.microsoft.com/en-us/)  
- CMS, на базе [Strapi](https://strapi.io/)  
- Сервис публикации презентаций PDF, на базе [PDFJS](https://mozilla.github.io/pdf.js/)  
- СУБД [PostgreSQL](https://www.postgresql.org/)  



