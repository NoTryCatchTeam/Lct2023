
# Управление контентом
Все операции по добавлению, публикации и удалению контента выполняются в [CMS](http://45.9.27.2/admin) в разделе "Редактор контента"  
![](pictures/content editor.png)

## Создание записи

- Выбрать тип коллекции, например "location"   
- Нажать кнопку "Создать новую запись"  
![](pictures/create location.png)  
- Заполните запись необходимыми данными (см. [Типы коллекций](./#_5))  
- Нажмите "Сохранить"  
![](pictures/save.png)  

Созданные записи не будут отображаться в мобильном приложении, если они не опубликованны.
## Публикация записи

- Выбрать сохраненную запись, которую хотите опубликовать  
- Нажать кнопку "Опубликовать"  
![](pictures/publish.png)  

Опубликованная запись будет отображаться в мобильном приложении  

- При желании можно отменить публикацию записи, нажав кнопку "Отменить публикацию"  
![](pictures/unpublish.png)   

Запись перестанет отображаться в мобильном приложении, но будет сохранена в базе данных.  

## Удаление записи

- Выбрать сохраненную запись, которую хотите удалить  
- Удалить ее из представления коллекции, нажав иконку корзины  
![](pictures/delete from collection.png)   
- Или удалить ее из представления записи, нажав кнопку "Удалить эту запись"  
![](pictures/delete from item.png)    

## Добавление медиа-файлов

# Типы коллекций
## Учебное заведение
Колекция "location"
![](pictures/location.png)    

name: Название школы (полное)  
shortName: Название школы (короткое)  
lat: Широта  
lon: Долгота  
address: Адрес  
email: Email адрес  
phone: Телефон  
district: Район  
streams: Направления обучения (программы)

## Мероприятие
Коллекция "event"
![](pictures/event.png)    
name:
description:
eventDate:
cover:
gallery:
link:
ticketLink:
place:
streams:

## Новость
Коллекция "article"
![](pictures/article.png)  

title:
text:
cover:
art_categories:
createDate:
location:
link:
rubric:

## Курс
Коллекция "course"
![](pictures/course.png)  

name:
price:
type:
level:
isOnline:
location:
description:
streams:
author:
lessons:
## Урок
Коллекция "lesson"
![](pictures/lesson.png)  

name:
lessonNumber:
description:
additionalMaterial:
content:
homework:
isLocked:
lessonChapter:


## Автор
Коллекция "author"
![](pictures/author.png)  

name:
description:
avatar: