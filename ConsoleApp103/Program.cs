using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
namespace ConsoleApp103
{
    internal class Program
    {

        static void Main(string[] args)
        {
            TcpListener server = null;
            try
            {
                // Устанавливаем порт для прослушивания клиентов
                int port = 8888;
                IPAddress localAddr = IPAddress.Parse("127.0.0.1");

                server = new TcpListener(localAddr, port);

                server.Start();

                Console.WriteLine("Ожидание подключений...");

                // Генерируем случайное число от 1 до 10
                Random random = new Random();
                int secretNumber = random.Next(1, 5);

                while (true)
                {
                    TcpClient client = server.AcceptTcpClient();
                    Console.WriteLine("Клиент подключен!");

                    NetworkStream stream = client.GetStream();

                    // Приветственное сообщение
                    byte[] welcomeMessage = Encoding.UTF8.GetBytes("Добро пожаловать в игру 'Угадай число'!\n");
                    stream.Write(welcomeMessage, 0, welcomeMessage.Length);

                    // Получаем имя игрока
                    byte[] data = new byte[256];
                    int bytes = stream.Read(data, 0, data.Length);
                    string playerName = Encoding.UTF8.GetString(data, 0, bytes);
                    Console.WriteLine($"Игрок {playerName} присоединился к игре.");

                    // Отправляем сообщение секретного числа
                    byte[] secretNumberMessage = Encoding.UTF8.GetBytes($"Загаданное число от 1 до 5. Попробуйте угадать!\n");
                    stream.Write(secretNumberMessage, 0, secretNumberMessage.Length);

                    bool guessed = false;
                    while (!guessed)
                    {
                        // Получаем догадку игрока
                        bytes = stream.Read(data, 0, data.Length);
                        string guess = Encoding.UTF8.GetString(data, 0, bytes);

                        // Проверяем догадку
                        int playerGuess;
                        if (int.TryParse(guess, out playerGuess))
                        {
                            if (playerGuess == secretNumber)
                            {
                                // Игрок угадал число
                                byte[] successMessage = Encoding.UTF8.GetBytes($"Поздравляем, {playerName}! Вы угадали число {secretNumber}!\n");
                                stream.Write(successMessage, 0, successMessage.Length);
                                guessed = true;
                            }
                            else
                            {
                                // Сравниваем числа и отправляем подсказку
                                string hint = (playerGuess < secretNumber) ? "Число больше." : "Число меньше.";
                                byte[] hintMessage = Encoding.UTF8.GetBytes(hint + "\n");
                                stream.Write(hintMessage, 0, hintMessage.Length);
                            }
                        }
                        else
                        {
                            // Некорректный ввод
                            byte[] errorMessage = Encoding.UTF8.GetBytes("Некорректный ввод. Пожалуйста, введите число от 1 до 5.\n");
                            stream.Write(errorMessage, 0, errorMessage.Length);
                        }
                    }

                    // Закрываем соединение с клиентом
                    client.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                // Закрываем сервер
                server?.Stop();
            }
        }
    }
}