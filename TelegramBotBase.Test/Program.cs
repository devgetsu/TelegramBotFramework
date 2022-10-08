﻿using System;
using System.Linq;
using System.Threading.Tasks;
using TelegramBotBase.Args;
using TelegramBotBase.Builder;
using TelegramBotBase.Commands;
using TelegramBotBase.Enums;
using TelegramBotBaseTest.Tests;

namespace TelegramBotBaseTest;

internal class Program
{
    private static void Main(string[] args)
    {
        var apiKey = "";

        var bb = BotBaseBuilder
                 .Create()
                 .WithAPIKey(apiKey)
                 .DefaultMessageLoop()
                 .WithStartForm<Start>()
                 .NoProxy()
                 .CustomCommands(a =>
                 {
                     a.Start("Starts the bot");
                     a.Add("myid", "Returns my Device ID");
                     a.Help("Should show you some help");
                     a.Settings("Should show you some settings");
                     a.Add("form1", "Opens test form 1");
                     a.Add("form2", "Opens test form 2");
                     a.Add("params", "Returns all send parameters as a message.");
                 })
                 .NoSerialization()
                 .UseEnglish()
                 .Build();


        bb.BotCommand += Bb_BotCommand;

        //Update Bot commands to botfather
        bb.UploadBotCommands().Wait();

        bb.SetSetting(ESettings.LogAllMessages, true);

        bb.Message += (s, en) =>
        {
            Console.WriteLine(en.DeviceId + " " + en.Message.MessageText + " " + (en.Message.RawData ?? ""));
        };

        bb.Start();

        Console.WriteLine("Telegram Bot started...");

        Console.WriteLine("Press q to quit application.");


        Console.ReadLine();

        bb.Stop();
    }

    private static async Task Bb_BotCommand(object sender, BotCommandEventArgs en)
    {
        switch (en.Command)
        {
            case "/start":


                var start = new Menu();

                await en.Device.ActiveForm.NavigateTo(start);

                break;
            case "/form1":

                var form1 = new TestForm();

                await en.Device.ActiveForm.NavigateTo(form1);


                break;


            case "/form2":

                var form2 = new TestForm2();

                await en.Device.ActiveForm.NavigateTo(form2);

                break;

            case "/myid":

                await en.Device.Send($"Your Device ID is: {en.DeviceId}");

                en.Handled = true;

                break;

            case "/params":

                var m = en.Parameters.DefaultIfEmpty("").Aggregate((a, b) => a + " and " + b);

                await en.Device.Send("Your parameters are: " + m, replyTo: en.Device.LastMessageId);

                en.Handled = true;

                break;
        }
    }
}
