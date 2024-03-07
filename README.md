## 2й этап
Проект cmd - 2й этап. Работает в командной строке. Точка входа - метод Main в классе Program.

# 4й этап
Проект client - клиент. Интерфейс построен с использованием .NET MAUI. Точка входа - метод CreateMauiApp(), в классе MauiProgram. Паттерн - MVVM. Для этого используется библиотека: CommunityToolkit.Mvvm (https://www.nuget.org/packages/CommunityToolkit.Mvvm).

Проект server - сервер. Работает на ASP .NET Core. Авторизация работает через систему Identity с базой данных. Файл конфигурации со строкой подключения к БД - appSettings.json.

Используются api контроллеры для авторизации, регистрации и получения данных. 

## Про БД
Проще всего создать базу данных с помошью миграции:

Для этого, в консоли диспетчера пакетов VS нужно прописать:
* Add-Migration Initial - Создаст миграцию
* Update-Database - Применит изменения в БД

Строк подключения к БД указана в appsettings.json. База данных будет создана с названием, указанным в строке подключения.

Ip и port сервера указывается в appsettings.json.
Ip и port для подключения к api сервера устанавливается в конструкторе класса Utils.NetUtils у клиента (потом будет переделано)

## Логи
В папке решения (там где лежит project.sln) появится папка logs, в которой будут 2 папки:
* AspNetCore - лог работы сервера ASP .NET Core
* Simulator - лог работы модели из 2-го этапа

## Замечание по appsettings.json для клиента
В .NET Maui нормально не реализована работа с appsettings.json, поэтому использутся костыль: 
* AppSettings.json - обозначается как EmbededResource, т.е. компилируется в итоговый бинарный файл

## Не сделано
* Задрежка между действиями в модели
* Форма для подключения к серверу (сейчас ip захардкожен)
* Обработка исключений при работе с сервером
* Отображение текущего состояния подключения к серверу
* В клиенте, при получении логов нет фокусировки на новых логах + логи не старые логи не очищаются
* Нет дополнительной формы регистрации
* В коде: нет комментариев, нет регионов, да и сам код написан на скорую руку
* IP для api захардкожено в клиенте, а не в конфигурации или отдельном классе
* Не вырезана web страничка сервера


