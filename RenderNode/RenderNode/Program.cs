using System;


class Program
{
    static void Main(string[] args)
    {
        Tools.OverrideLogToFile();// активируем запись консольных сообщений в файл

        RenderNodeSettings RNS = new RenderNodeSettings();// запускаем ноду и создаем первичный файл настроек если его нет

        LocalServer.StartListen();// запускаем локальный сервер для прослушки команд с клиента

    }
}