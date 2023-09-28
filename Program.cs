﻿using System;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.ReplyMarkups;

namespace Telegram_Bot
{
    class Program
    {
        private static string token { get; } = "6523896361:AAFNzToDOLyp5vSc1j3fIjdHT9-4KyyuDRg";
        private static TelegramBotClient client;
        static void Main(string[] args)
        {
            client = new TelegramBotClient(token);
            client.StartReceiving();
            client.OnMessage += OnMessageHandler;
            Console.ReadLine();
            client.StopReceiving();
        }

        private static async void OnMessageHandler(object? sender, MessageEventArgs e)
        {
            var msg = e.Message;
            if (msg.Text != null)
            {
                Console.WriteLine($"Прийшло повiдомлення з текстом: {msg.Text}");
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
                    case "Контакти":
                        await client.SendTextMessageAsync(msg.Chat.Id, "Адміністратор:\n<Прізвище та ім'я> <номер телефону>\n\n" +
                            "У разі виникнення помилки звертайтеся на електронну скриньку або за номером::\n<email@example.com>, <номер телефону>", replyMarkup: GetButtons());
                        break;
                    case "Графік роботи":
                        {
                            await client.SendTextMessageAsync(msg.Chat.Id, "Пн 09:00 - 20:00\nВт  09:00 - 20:00\nСр 10:00 - 20:00\nЧт  09:00 - 20:00\nПт  09:00 - 20:00" +
                                "\nСб  13:00 - 18:00\nНд  14:00 - 18:00", replyMarkup: GetButtons()); break;
                        }

                    default: await client.SendTextMessageAsync(msg.Chat.Id, "Виберiть команду: ", replyMarkup: GetButtons()); break;

                }
                //await client.SendTextMessageAsync(msg.Chat.Id, msg.Text, replyToMessageId: msg.MessageId);

                //await client.SendTextMessageAsync(msg.Chat.Id, msg.Text, replyMarkup: GetButtons());
            }
        }

        private static IReplyMarkup GetButtons()
        {
            return new ReplyKeyboardMarkup
            {
                Keyboard = new List<List<KeyboardButton>>
                {
                    new List<KeyboardButton>{ new KeyboardButton { Text = "Підібрати лижі"}, new KeyboardButton { Text = "Забронювати"} },
                    new List<KeyboardButton>{ new KeyboardButton { Text = "Контакти"}, new KeyboardButton { Text = "Графік роботи" } }
                }
            };
        }
    }
}
