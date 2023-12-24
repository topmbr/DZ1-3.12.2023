using System;
using System.Net.Sockets;
using System.Text;
namespace ConsoleApp2
{
    internal class Program
    {
        static void Main(string[] args)
        {
            try
            {
                // Подключаемся к серверу
                TcpClient client = new TcpClient("127.0.0.1", 8888);
                NetworkStream stream = client.GetStream();

                // Вводим имя игрока
                Console.Write("Введите свое имя: ");
                string playerName = Console.ReadLine();

                // Отправляем имя серверу
                byte[] playerNameBytes = Encoding.UTF8.GetBytes(playerName);
                stream.Write(playerNameBytes, 0, playerNameBytes.Length);

                // Получаем приветственное сообщение от сервера
                byte[] welcomeMessage = new byte[256];
                int bytesRead = stream.Read(welcomeMessage, 0, welcomeMessage.Length);
                Console.WriteLine(Encoding.UTF8.GetString(welcomeMessage, 0, bytesRead));

                while (true)
                {
                    // Вводим догадку
                    Console.Write("Введите число от 1 до 5: ");
                    string guess = Console.ReadLine();

                    // Отправляем догадку серверу
                    byte[] guessBytes = Encoding.UTF8.GetBytes(guess);
                    stream.Write(guessBytes, 0, guessBytes.Length);

                    // Получаем ответ от сервера
                    byte[] response = new byte[256];
                    bytesRead = stream.Read(response, 0, response.Length);
                    Console.WriteLine(Encoding.UTF8.GetString(response, 0, bytesRead));

                    // Проверяем, угадал ли игрок число
                    if (Encoding.UTF8.GetString(response, 0, bytesRead).Contains("Поздравляем"))
                    {
                        Console.WriteLine("Вы выиграли! Нажмите Enter для завершения.");
                        Console.ReadLine();
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }
    }
}