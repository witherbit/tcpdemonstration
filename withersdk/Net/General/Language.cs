using System;
using System.Collections.Generic;
using System.Text;

namespace withersdk.Net.General
{
    public class Language
    {
        public static LanguageType Type { get; set; } = LanguageType.RU;
        public static string INVALID_MAC_MESSAGE_EX { 
            get
            {
                switch (Type)
                {
                    case LanguageType.EN:
                        return "Couldn't check the MAC";
                    default:
                        return "Не удалось проверить MAC";
                }
            }
        }
        public static string INVALID_SIGNATURE_MESSAGE_EX
        {
            get
            {
                switch (Type)
                {
                    case LanguageType.EN:
                        return "The electronic signature could not be verified";
                    default:
                        return "Не удалось проверить электронную подпись";
                }
            }
        }
        public static string TRY_START_SERVER
        {
            get
            {
                switch (Type)
                {
                    case LanguageType.EN:
                        return "Starting the host";
                    default:
                        return "Запуск хоста";
                }
            }
        }
        public static string TRY_START_SERVER_EX
        {
            get
            {
                switch (Type)
                {
                    case LanguageType.EN:
                        return "Error when starting the host";
                    default:
                        return "Ошибка при запуске хоста";
                }
            }
        }
        public static string START_SERVER
        {
            get
            {
                switch (Type)
                {
                    case LanguageType.EN:
                        return "The host is running";
                    default:
                        return "Хост запущен";
                }
            }
        }
        public static string NEW_CONNECTION
        {
            get
            {
                switch (Type)
                {
                    case LanguageType.EN:
                        return "New connection from";
                    default:
                        return "Новое подключение с";
                }
            }
        }
        public static string LISTENER_EX
        {
            get
            {
                switch (Type)
                {
                    case LanguageType.EN:
                        return "Listening error";
                    default:
                        return "Ошибка при прослушивании";
                }
            }
        }
        public static string LISTENER_CLIENT_EX
        {
            get
            {
                switch (Type)
                {
                    case LanguageType.EN:
                        return "Client listening error";
                    default:
                        return "Ошибка при прослушивании клиента";
                }
            }
        }
        public static string READ_BUFFER_EX
        {
            get
            {
                switch (Type)
                {
                    case LanguageType.EN:
                        return "Error reading the stream";
                    default:
                        return "Ошибка при чтении потока";
                }
            }
        }
        public static string CLOSE_SERVER
        {
            get
            {
                switch (Type)
                {
                    case LanguageType.EN:
                        return "The host is stopped";
                    default:
                        return "Хост остановлен";
                }
            }
        }
        public static string CLOSE_CLIENT
        {
            get
            {
                switch (Type)
                {
                    case LanguageType.EN:
                        return "Connection was interrupted at";
                    default:
                        return "Подключение прервано у";
                }
            }
        }
        public static string SERVER_MSG_SEND
        {
            get
            {
                switch (Type)
                {
                    case LanguageType.EN:
                        return "The package was sent to the recipient";
                    default:
                        return "Пакет отправлен получателю";
                }
            }
        }
        public static string SERVER_MSG_SEND_EX
        {
            get
            {
                switch (Type)
                {
                    case LanguageType.EN:
                        return "An error occurred while sending the package";
                    default:
                        return "При отправке пакета, произошла ошибка";
                }
            }
        }
        public static string SERVER_TUNNEL_CREATE
        {
            get
            {
                switch (Type)
                {
                    case LanguageType.EN:
                        return "Creating a secure tunnel with";
                    default:
                        return "Создание защищенного туннеля с";
                }
            }
        }
        public static string CLIENT_TUNNEL_CREATE
        {
            get
            {
                switch (Type)
                {
                    case LanguageType.EN:
                        return "Creating a secure tunnel";
                    default:
                        return "Создание защищенного туннеля";
                }
            }
        }
        public static string SERVER_TUNNEL_CREATE1
        {
            get
            {
                switch (Type)
                {
                    case LanguageType.EN:
                        return "Creating keys with";
                    default:
                        return "Создание ключей с";
                }
            }
        }
        public static string CLIENT_TUNNEL_CREATE1
        {
            get
            {
                switch (Type)
                {
                    case LanguageType.EN:
                        return "Creating keys";
                    default:
                        return "Создание ключей";
                }
            }
        }
        public static string SERVER_TUNNEL_CREATE2
        {
            get
            {
                switch (Type)
                {
                    case LanguageType.EN:
                        return "Creating MAC with";
                    default:
                        return "Создание MAC с";
                }
            }
        }
        public static string CLIENT_TUNNEL_CREATE2
        {
            get
            {
                switch (Type)
                {
                    case LanguageType.EN:
                        return "Creating MAC";
                    default:
                        return "Создание MAC";
                }
            }
        }
        public static string SERVER_TUNNEL_CREATE3
        {
            get
            {
                switch (Type)
                {
                    case LanguageType.EN:
                        return "Checking MAC with";
                    default:
                        return "Проверка MAC с";
                }
            }
        }
        public static string CLIENT_TUNNEL_CREATE3
        {
            get
            {
                switch (Type)
                {
                    case LanguageType.EN:
                        return "Checking MAC";
                    default:
                        return "Проверка MAC";
                }
            }
        }
        public static string CLIENT_START_CONNECT
        {
            get
            {
                switch (Type)
                {
                    case LanguageType.EN:
                        return "Connecting to the host";
                    default:
                        return "Подключение к хосту";
                }
            }
        }
        public static string CLIENT_START_CONNECT_EX
        {
            get
            {
                switch (Type)
                {
                    case LanguageType.EN:
                        return "Error connecting to the host";
                    default:
                        return "Ошибка при подключении к хосту";
                }
            }
        }
        public static string CLIENT_STOP
        {
            get
            {
                switch (Type)
                {
                    case LanguageType.EN:
                        return "Connection interrupted";
                    default:
                        return "Подключение прервано";
                }
            }
        }
        public static string CLIENT_RECEIVE_EX
        {
            get
            {
                switch (Type)
                {
                    case LanguageType.EN:
                        return "Error listening to the host";
                    default:
                        return "Ошибка при прослушивании хоста";
                }
            }
        }
        public static string CLIENT_MSG_SEND
        {
            get
            {
                switch (Type)
                {
                    case LanguageType.EN:
                        return "The package has been sent to the host";
                    default:
                        return "Пакет отправлен на хост";
                }
            }
        }
        public static string CLIENT_MSG_SEND_EX
        {
            get
            {
                switch (Type)
                {
                    case LanguageType.EN:
                        return "An error occurred while sending the package";
                    default:
                        return "При отправке пакета, произошла ошибка";
                }
            }
        }
        public static string CLIENT_TUNNEL_COMPLETE
        {
            get
            {
                switch (Type)
                {
                    case LanguageType.EN:
                        return "A secure tunnel has been created!";
                    default:
                        return "Защищенный туннель создан!";
                }
            }
        }
        public static string SERVER_TUNNEL_COMPLETE
        {
            get
            {
                switch (Type)
                {
                    case LanguageType.EN:
                        return "A secure tunnel has been created";
                    default:
                        return "Защищенный туннель создан";
                }
            }
        }
        public static string SERVER_BUFFER_READ_START
        {
            get
            {
                switch (Type)
                {
                    case LanguageType.EN:
                        return "Starting an exchange with";
                    default:
                        return "Запуск обмена с";
                }
            }
        }
        public static string CLIENT_BUFFER_READ_START
        {
            get
            {
                switch (Type)
                {
                    case LanguageType.EN:
                        return "Starting an exchange";
                    default:
                        return "Запуск обмена";
                }
            }
        }
        public static string CLIENT_TUNNEL_DENY
        {
            get
            {
                switch (Type)
                {
                    case LanguageType.EN:
                        return "The host rejected the request to create a secure connection";
                    default:
                        return "Хост отклонил запрос на создание защищенного подключения";
                }
            }
        }
        public static string CLIENT_TUNNEL_DENY_MAC
        {
            get
            {
                switch (Type)
                {
                    case LanguageType.EN:
                        return "The client's MAC does not match the host's MAC";
                    default:
                        return "MAC клиента не совпадает с MAC хоста";
                }
            }
        }
        public static string CLIENT_TUNNEL_DENY_PEER
        {
            get
            {
                switch (Type)
                {
                    case LanguageType.EN:
                        return "The server sent incorrect data";
                    default:
                        return "Сервер отправил некорректные данные";
                }
            }
        }
        public static string CLIENT_BUFFER_READ_EX
        {
            get
            {
                switch (Type)
                {
                    case LanguageType.EN:
                        return "An error occurred while reading the stream";
                    default:
                        return "При чтении потока произошла ошибка";
                }
            }
        }
        public static string SERVER_TUNNEL_DENY_MAC
        {
            get
            {
                switch (Type)
                {
                    case LanguageType.EN:
                        return "The host's MAC does not match the client's MAC";
                    default:
                        return "MAC хоста не совпадает с MAC клиента";
                }
            }
        }
        public static string SERVER_TUNNEL_DENY_PEER
        {
            get
            {
                switch (Type)
                {
                    case LanguageType.EN:
                        return "The client sent incorrect data";
                    default:
                        return "Клиент отправил некорректные данные";
                }
            }
        }
        public static string SERVER_GET_PACKET
        {
            get
            {
                switch (Type)
                {
                    case LanguageType.EN:
                        return "The package was accepted from";
                    default:
                        return "Пакет принят с ";
                }
            }
        }
        public static string CLIENT_GET_PACKET
        {
            get
            {
                switch (Type)
                {
                    case LanguageType.EN:
                        return "The package was accepted from host";
                    default:
                        return "Пакет принят с хоста";
                }
            }
        }
        public static string SQL_TRY_OPEN
        {
            get
            {
                switch (Type)
                {
                    case LanguageType.EN:
                        return "Opening a connection with SQL";
                    default:
                        return "Открытие соединения с SQL";
                }
            }
        }
        public static string SQL_OPEN
        {
            get
            {
                switch (Type)
                {
                    case LanguageType.EN:
                        return "The connection to SQL is open";
                    default:
                        return "Соединение с SQL открыто";
                }
            }
        }
        public static string SQL_CLOSE
        {
            get
            {
                switch (Type)
                {
                    case LanguageType.EN:
                        return "The connection to SQL is closed";
                    default:
                        return "Соединение с SQL закрыто";
                }
            }
        }
        public static string SQL_OPEN_EX
        {
            get
            {
                switch (Type)
                {
                    case LanguageType.EN:
                        return "Error when trying to connect to SQL";
                    default:
                        return "Ошибка при попытке cоединения с SQL";
                }
            }
        }
        public static string CLIENT_TUNNEL_STATE_PARAM
        {
            get
            {
                switch (Type)
                {
                    case LanguageType.EN:
                        return "State of tunnel creation";
                    default:
                        return "Состояние создания туннеля";
                }
            }
        }
    }

    public enum LanguageType
    {
        EN,
        RU
    }
}
