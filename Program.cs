﻿using Telegram.Bot.Args;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot;
using System;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using System.Data;
using MySql.Data;
using MySql.Data.MySqlClient;
using MySqlX.XDevAPI.Common;
using System.Text;
using static Org.BouncyCastle.Asn1.Cmp.Challenge;

namespace Telegram_Bot
{
    class Program
    {
        private static string token { get; } = "6523896361:AAFNzToDOLyp5vSc1j3fIjdHT9-4KyyuDRg";
        private static TelegramBotClient client;
        static string connectionString = "server=194.44.236.9; database=sqlkns23_1_gryu;user=sqlkns23_1_gryu; password=kns23_gryu; charset=cp1251;";
        static Random random = new Random();

        public static string ConStr { get; set; }
        public static DataTable myfunDt(string commandString)
        {
            DataTable dataTable = new DataTable();

            using (MySqlConnection connection = new MySqlConnection(ConStr))
            {
                MySqlCommand mySqlCommand = new MySqlCommand(commandString, connection);
                try
                {
                    connection.Open();
                    using (MySqlDataReader dataReader = mySqlCommand.ExecuteReader())
                    {
                        if (dataReader.HasRows)
                        {
                            dataTable.Load(dataReader);
                        }
                    }
                    connection.Close();
                }
                catch (Exception e)
                {
                }
            }
            return dataTable;
        }

        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            client = new TelegramBotClient(token);
            client.StartReceiving();
            client.OnMessage += OnMessageHandler;
            Console.ReadLine();
            client.StopReceiving();
        }
        static Dictionary<long, SkiData> userData = new Dictionary<long, SkiData>();
        static bool calculate = false;
        static bool chooseRoom = false;
        static bool takeRoom = false;
        private static async void OnMessageHandler(object? sender, MessageEventArgs e)
        {
            var msg = e.Message;
            if (msg.Text != null)
            {
                Console.WriteLine($"Прийшло повiдомлення з текстом: {msg.Text}");
                var chatId = msg.Chat.Id;

                if (!userData.ContainsKey(chatId))
                {
                    userData[chatId] = new SkiData();
                }
                var userSkiData = userData[chatId];
                switch (msg.Text)
                {
                    //case "Стiкер":
                    //    var stic = await client.SendStickerAsync(
                    //    chatId: msg.Chat.Id,
                    //    sticker: "https://tlgrm.eu/_/stickers/275/bd2/275bd274-2409-4c87-91d1-851dbab04d5f/1.webp", replyMarkup: GetButtons());
                    //    break;
                    //case "Картинка":
                    //    var pic = await client.SendPhotoAsync(
                    //    chatId: msg.Chat.Id,
                    //    photo: "https://images.unsplash.com/photo-1560710990-9f5d4197b5a2?ixlib=rb-4.0.3&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D&auto=format&fit=crop&w=2070&q=80",
                    //    replyMarkup: GetButtons());
                    //    break;
                    case "Забронювати":
                        {
                            chooseRoom = true;
                            await client.SendTextMessageAsync(e.Message.Chat.Id, "Введіть номер кімнати для бронювання");
                            break;
                        }
                    case "Підібрати лижі":
                        {
                            calculate = true;
                            if (userSkiData.Height == -1)
                            {
                                await client.SendTextMessageAsync(chatId, "Введіть ваш ріст (у см):", replyMarkup: new ReplyKeyboardRemove());
                            }
                            //else if (userSkiData.Weight == -1)
                            //{
                            //    await client.SendTextMessageAsync(chatId, "Введіть вашу вагу (у кг):", replyMarkup: new ReplyKeyboardRemove());
                            //}
                            //else if (userSkiData.FootSize == -1)
                            //{
                            //    await client.SendTextMessageAsync(chatId, "Введіть ваш розмір стопи (у см):", replyMarkup: new ReplyKeyboardRemove());
                            //}
                            else
                            {
                                await client.SendTextMessageAsync(chatId, PickSkis(userSkiData.Height, userSkiData.Weight, userSkiData.FootSize), replyMarkup: new ReplyKeyboardRemove());
                                userData.Remove(chatId);
                            }   
                            break;
                        }
                    case "Контакти":
                        await client.SendTextMessageAsync(msg.Chat.Id, "Адміністратор:\n<Прізвище та ім'я> <номер телефону>\n\n" +
                            "У разі виникнення помилки звертайтеся на електронну скриньку або за номером::\n<email@example.com>, <номер телефону>", replyMarkup: GetButtons());
                        await client.SendTextMessageAsync(msg.Chat.Id, "Пн 09:00 - 20:00\nВт  09:00 - 20:00\nСр 10:00 - 20:00\nЧт  09:00 - 20:00\nПт  09:00 - 20:00" +
                            "\nСб  13:00 - 18:00\nНд  14:00 - 18:00", replyMarkup: GetButtons());
                        await client.SendPhotoAsync(chatId: msg.Chat.Id, photo: "https://i.postimg.cc/W361kMyj/qr.jpg", replyMarkup: GetButtons());
                        break;
                    case "Вільні номери кімнат":
                        {
                            GetAvaibleRooms(e);
                            break;
                        }
                    default:
                        if (int.TryParse(msg.Text, out int parsedValue) && parsedValue > 0 && calculate == true || chooseRoom == true)
                        {
                            if (userSkiData.Height == -1 && calculate == true)
                            {
                                userSkiData.Height = parsedValue;
                                await client.SendTextMessageAsync(chatId, "Введіть вашу вагу (у кг):", replyMarkup: new ReplyKeyboardRemove());
                            }
                            else if (userSkiData.Weight == -1 && calculate == true)
                            {
                                userSkiData.Weight = parsedValue;
                                await client.SendTextMessageAsync(chatId, "Введіть ваш розмір стопи (у см):", replyMarkup: new ReplyKeyboardRemove());
                            }
                            else if (userSkiData.FootSize == -1 && calculate == true)
                            {
                                userSkiData.FootSize = parsedValue;
                                await client.SendTextMessageAsync(chatId, PickSkis(userSkiData.Height, userSkiData.Weight, userSkiData.FootSize), replyMarkup: GetButtons());
                                userData.Remove(chatId);
                                //GetAvaibleRooms(e);
                            }
                            //if (userSkiData.Room == -1 && chooseRoom == true)
                            //{
                            //    userSkiData.Room = parsedValue;
                            //    if (CheckRoom(e, userSkiData.Room) != 0)
                            //    {
                            //        await client.SendTextMessageAsync(chatId, $"Номер вашої кімнати: {CheckRoom(e, userSkiData.Room)}", replyMarkup: GetButtons());
                            //        userData.Remove(chatId);
                            //    }
                            //    else
                            //    {
                            //        await client.SendTextMessageAsync(chatId, $"Номера такої кімнати не існує або уже заброньовано", replyMarkup: GetButtons());
                            //    }

                            //}
                        }
                        else if (calculate == true)
                        {
                            await client.SendTextMessageAsync(chatId, "Введіть коректне ціле додатнє число.");
                        }
                        else await client.SendTextMessageAsync(chatId, "Виберіть команду.");

                        if (userSkiData.Room == -1 && chooseRoom == true)
                        {
                            userSkiData.Room = parsedValue;
                            int result = CheckRoom(e, userSkiData.Room);
                            if (result != 0)
                            {
                                await client.SendTextMessageAsync(chatId, $"Номер вашої кімнати: {result}", replyMarkup: GetButtons());
                                int randomNumber = random.Next(100000, 999999);
                                TakeRoom(result, randomNumber);
                                Console.WriteLine($"--------------\nКімната - {result}, Код - {randomNumber} | Додано до бази даних\n--------------");
                                userData.Remove(chatId);
                            }
                            else
                            {
                                await client.SendTextMessageAsync(chatId, $"Номера такої кімнати не існує або уже заброньовано\nНатисніть на кнопку\"Вільні номери кімнат\"",
                                    replyMarkup: GetButtons());
                                userSkiData.Room = -1;
                            }

                        }
                        break;
                }
            }
        }
        private static async Task<Telegram.Bot.Types.Message> GetAvaibleRooms(MessageEventArgs e)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "SELECT * FROM IT_HotelRoom";
                    MySqlCommand command = new MySqlCommand(query, connection);

                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        string result = "";
                        while (reader.Read())
                        {
                            if (reader.GetString(3) == "1")
                            {
                                result += $"Вільні кімнати: {reader.GetInt32(1)}\n"; // (номер колонки)
                                                                              //result += $"room: {reader.GetInt32(1)} - access: {reader.GetString(3)}\n"; // (номер колонки)
                            }

                        }
                        return await client.SendTextMessageAsync(e.Message.Chat.Id, result);
                    }
                }
            }
            catch (Exception ex)
            {
                return await client.SendTextMessageAsync(e.Message.Chat.Id, "Error");
                //Console.WriteLine(ex.Message);
            }
        }

        private static int CheckRoom(MessageEventArgs e, int room)
        {
            
            // access - 2 
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "SELECT * FROM IT_HotelRoom";
                    MySqlCommand command = new MySqlCommand(query, connection);

                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        int result = 0;
                        while (reader.Read())
                        {
                            if (reader.GetString(3) == "1" && reader.GetInt32(1) == room)
                            {
                                //result += $"Вільні кімнати: {reader.GetInt32(1)}\n"; // (номер колонки)
                                result = reader.GetInt32(1);
                                //result += $"room: {reader.GetInt32(1)} - access: {reader.GetString(3)}\n"; // (номер колонки)
                                chooseRoom = false;
                                //takeRoom = true;
                            }

                        }
                        
                        return result;
                        //await client.SendTextMessageAsync(e.Message.Chat.Id, result);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return 0;
            }
        }

        private static void TakeRoom(int room, int code)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    string query = $"INSERT INTO IT_Reservation (code_bot, secret_code) VALUES ('{room}', '{code}');";
                    MySqlCommand command = new MySqlCommand(query, connection);
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                
            }
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    string query = $"UPDATE IT_HotelRoom SET access = '2' WHERE room = {room}";
                    MySqlCommand command = new MySqlCommand(query, connection);
                    command.ExecuteNonQuery();
                }
                
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.Message);
            }
        }

        private static string PickSkis(int height, int width, int footSize)
        {
            calculate = false;
            return $"Розмір лиж: <розмір>, за такими параметрами\nВисота: {height}, Вага: {width}, Розмір стопи: {footSize}";
        }

        private static IReplyMarkup GetButtons()
        {
            return new ReplyKeyboardMarkup
            {
                Keyboard = new List<List<KeyboardButton>>
                {
                    new List<KeyboardButton>{ new KeyboardButton { Text = "Підібрати лижі"}, new KeyboardButton { Text = "Забронювати"} },
                    new List<KeyboardButton>{ new KeyboardButton { Text = "Контакти"}, new KeyboardButton { Text = "Вільні номери кімнат" } }
                }
            };
        }
    }
}