using System;
using System.Text.RegularExpressions;
using VkBotFramework.Models;
using VkBotFramework;
using VkNet.Enums.SafetyEnums;
using VkNet.Model.Keyboard;
using VkNet.Model.RequestParams;
using Microsoft.Extensions.Logging;
using System.Linq;

namespace VkMedBot
{
	internal class Program
	{

		private static string MateshaSubscribeCommand = "\\+матеша";
		private static string MateshaUnsubscribeCommand = "\\-матеша";
		private static string MateshaNumberCommand = @"\d+$";

		static void MateshaNumberHandler(VkBot sender, MessageReceivedEventArgs args)
		{
			int userAnswer = int.Parse(Regex.Match(args.Message.Text, MateshaNumberCommand).Value);

			var keyboard = new KeyboardBuilder()
				.AddButton("+матеша", "", KeyboardButtonColor.Positive)
				.AddButton("-матеша", "", KeyboardButtonColor.Negative);

			sender.Api.Messages.Send(new MessagesSendParams()
			{

			});
		}

		static void MateshaHandler(VkBot sender, MessageReceivedEventArgs args)
		{
			//правильный ответ уже установлен в диалоге, значит бот ожидает ответа в диалоге

			var message = args.Message;
			var rand = new Random();
			var firstNum = rand.Next(1, 100);
			var secondNum = rand.Next(1, 100);
			var validAnswer = firstNum + secondNum;


			//регистрируем новый обработчик для этого диалога, который будет чувствителен к числам

			//рисуем кнопочки
			int buttonsCount = 10;
			int maxButtonsCountInLine = 4;
			int validButtonIndex = rand.Next(buttonsCount);
			var keyboard = new KeyboardBuilder();
			for (int i = 0; i < buttonsCount; i++)
			{
				if (i == validButtonIndex)
					keyboard.AddButton((validAnswer).ToString(), "", KeyboardButtonColor.Primary);
				else
					keyboard.AddButton(rand.Next(200).ToString(), "", KeyboardButtonColor.Primary);

				if ((i + 1) % maxButtonsCountInLine == 0)
					keyboard.AddLine();
			}

			sender.Api.Messages.Send(new MessagesSendParams()
			{
				RandomId = Math.Abs(Environment.TickCount),
				PeerId = args.Message.PeerId,
				Message = $"сколько будет {firstNum} + {secondNum}?",
				Keyboard = keyboard.Build()
			});
		}

		static void Main(string[] args)
		{
			Console.WriteLine("Hello World!");
			string AccessToken = "66812f7530221570065c6ebce8f52af2d4c7dec8e774e56400d8276619d4788ca34062ea194b4711888f7";
			string GroupUrl = "vk.com/club213473609";
			VkBot bot = new VkBot(AccessToken, GroupUrl);
			bot.OnMessageReceived += MessageReceivedTest;
			//bot.OnGroupUpdateReceived += UpdateReceivedTest;


			bot.TemplateManager.Register(new RegexToActionTemplate(MateshaSubscribeCommand,
				"ну окей, вот тебе матеша"));

			//bot.TemplateManager.Register(new RegexToActionTemplate(MateshaSubscribeCommand, MateshaHandler));


			bot.TemplateManager.Register(new RegexToActionTemplate(MateshaUnsubscribeCommand,
				"ну окей, теперь не будет у вас этой кнопки",
				new KeyboardBuilder().SetOneTime().AddButton("пока", "").Build()));

			bot.Start();

			Console.ReadLine();
		}

		public static void MessageReceivedTest(object sender, MessageReceivedEventArgs eventArgs)
		{
			VkBot instanse = sender as VkBot;
			var peerId = eventArgs.Message.PeerId;
			var fromId = eventArgs.Message.FromId;
			var text = eventArgs.Message.Text;

			instanse.Logger.LogInformation($"new message captured. peerId: {peerId},userId: {fromId}, text: {text}");
			instanse.Api.Messages.Send(new MessagesSendParams()
			{
				RandomId = Environment.TickCount,
				PeerId = eventArgs.Message.PeerId,
				Message =
					$"{fromId.Value}, i have captured your message: '{text}'. its length is {text.Length}. number of spaces: {text.Count(x => x == ' ')}"
			});
		}
	}
}
