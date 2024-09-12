using System.Net.Sockets;
using System.Net;
using System.Text;

namespace Task3_ChatApp_vSem2
{
    internal class Program
    {

        static void Server(string name)
        {
            UdpClient udpClient = new UdpClient(12345);
            IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);
            Console.WriteLine("UDP Сервер ожидает сообщений...");

            Thread serverThread = new Thread(() =>
            {

                while (true)
                {

                    byte[] receiveBytes = udpClient.Receive(ref remoteEndPoint);
                    string receivedData = Encoding.UTF8.GetString(receiveBytes);

                    new Thread(() =>
                    {
                        try
                        {
                            var message = Message.FromJson(receivedData);
                            Console.WriteLine($"Получено сообщение от {message?.FromName} - ({message?.Date}): {message?.Text}");
                            string replyMessage = "Сообщение получено";
                            var replyMessageJson = new Message()
                            {
                                FromName = name,
                                Date = DateTime.Now,
                                Text = replyMessage
                            }.ToJson();

                            byte[] replyBytes = Encoding.UTF8.GetBytes(replyMessageJson);
                            udpClient.Send(replyBytes, replyBytes.Length, remoteEndPoint);
                            Console.WriteLine("Ответ отправлен.");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Ошибка при обработке сообщения: " + ex.Message);
                        }
                    }).Start();
                }
            });

            serverThread.Start();
            Console.WriteLine("Нажмите любую клавишу для завершения работы сервера!");
            Console.ReadKey(true);
            udpClient.Close();
            Environment.Exit(0);
        }


        static void Client(string name, string ip)
        {
            UdpClient udpClient = new UdpClient();
            IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Parse(ip), 12345);
            Console.WriteLine("UDP клиент запущен...");           

            new Thread(() =>
            {
                while (true)
                {
                    try
                    {
                        Console.WriteLine("Введите сообщение или Exit для выхода из клиента");
                        string? message = Console.ReadLine();
                        if (message!.ToLower().Equals("exit"))
                        {
                            udpClient.Close();
                            Environment.Exit(0);
                        }

                        var messageJson = new Message()
                        {
                            FromName = name,
                            Date = DateTime.Now,
                            Text = message
                        }.ToJson();

                        byte[] replyBytes = Encoding.UTF8.GetBytes(messageJson);
                        udpClient.Send(replyBytes, replyBytes.Length, remoteEndPoint);
                        Console.WriteLine("Сообщение отправлено.");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Ошибка при обработке сообщения: " + ex.Message);
                    }

                    byte[] receiveBytes = udpClient.Receive(ref remoteEndPoint);
                    string receivedData = Encoding.UTF8.GetString(receiveBytes);
                    var messageReceived = Message.FromJson(receivedData);
                    Console.WriteLine($"Получено сообщение от {messageReceived?.FromName} - ({messageReceived?.Date}): {messageReceived?.Text}");
                }
            }).Start();
        }


        static void Main(string[] args)
        {
            if (args.Length == 1)
            {
                Server(args[0]);
            }
            else if (args.Length == 2)
            {
                Client(args[0], args[1]);
            }
            else
            {
                Console.WriteLine("Для запуска сервера введите ник-нейм как параметр запуска приложения");
                Console.WriteLine("Для запуска клиента введите ник-нейм и IP сервера как параметры запуска приложения");
            }
        }
    }
}
