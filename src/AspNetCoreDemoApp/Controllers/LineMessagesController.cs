using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Net;
using Newtonsoft.Json;
using System.Security.Cryptography;
using System.Text;

using Microsoft.AspNetCore.Http;
using AspNetCoreDemoApp.Models;

namespace AspNetCoreDemoApp.Controllers

{
	[Route("api/[controller]")]
	public class LineMessagesController : Controller
	{
		public static string temp = "";

		/// <summary>
		/// POST: api/Messages
		/// Receive a message from a user and reply to it
		/// </summary>
		[HttpPost]
		public async Task<HttpResponseMessage> Post(HttpRequestMessage request)
		{
			//if (!await VaridateSignature(request))
			//	return new HttpResponseMessage(HttpStatusCode.BadRequest);


			Activity activity = JsonConvert.DeserializeObject<Activity>
				(await request.Content.ReadAsStringAsync());

			if(activity==null)
			{
				activity = new Activity();
			}
			if (activity.Events == null || activity.Events.Length==0) 
			{
				Event lineEvent_temp = new Event();
				lineEvent_temp.Type = EventType.Message;



				temp = (await request.Content.ReadAsStringAsync()).ToString();
				//activity.Events[0] = lineEvent_temp;

			}

			// Line may send multiple events in one message, so need to handle them all.
			if (activity.Events != null && activity.Events.Length>0)
			{
				
				foreach (Event lineEvent in activity.Events)
				{
					LineMessageHandler handler = new LineMessageHandler(lineEvent);

					Profile profile = await handler.GetProfile(lineEvent.Source.UserId);

					switch (lineEvent.Type)
					{
						case EventType.Beacon:
							await handler.HandleBeaconEvent();
							break;
						case EventType.Follow:
							await handler.HandleFollowEvent();
							break;
						case EventType.Join:
							await handler.HandleJoinEvent();
							break;
						case EventType.Leave:
							await handler.HandleLeaveEvent();
							break;
						case EventType.Message:
							Message message = new Message();

							message = JsonConvert.DeserializeObject<Message>(lineEvent.Message.ToString());

							if (lineEvent.Message == null)
							{

								message.Id = "325708";
								message.Type = MessageType.Text;
							}

							switch (message.Type)
							{
								case MessageType.Text:
									await handler.HandleTextMessage();
									break;
								case MessageType.Audio:
								case MessageType.Image:
								case MessageType.Video:
									await handler.HandleMediaMessage();
									break;
								case MessageType.Sticker:
									await handler.HandleStickerMessage();
									break;
								case MessageType.Location:
									await handler.HandleLocationMessage();
									break;
							}
							break;
						case EventType.Postback:
							await handler.HandlePostbackEvent();
							break;
						case EventType.Unfollow:
							await handler.HandleUnfollowEvent();
							break;
					}
				}
			}else
			{
				Event ev = new Event();

				ev.Message = "{{\"id\": \"325708\",\"type\": \"text\",\"text\": \"Hello, world\"}}";
				ev.ReplyToken = @"VC/sPEIz8O7WYo0THlaM5laorgw+GifJmN8cFR5eZ0seYxNsR3ZOVblQgOeI8xNaCOyTsUz2VsajbtAyt8hj7+NdP2/oYB+7eQ/FGnEAN/ICGCPj5nX36E848piCYi16BPkXlDR3N0CDiPnrfbpPcAdB04t89/1O/w1cDnyilFU=";


				LineMessageHandler handler1 = new LineMessageHandler(ev);

				temp = (await request.Content.ReadAsStringAsync()).ToString();
				await handler1.HandleTextMessage();
			}

			return new HttpResponseMessage(HttpStatusCode.OK);
		}

		private async Task<bool> VaridateSignature(HttpRequestMessage request)
		{
			var hmac = new HMACSHA256(Encoding.UTF8.GetBytes("9833045df93888c8ed9f4924389d9cdc"));
			var computeHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(await request.Content.ReadAsStringAsync()));
			var contentHash = Convert.ToBase64String(computeHash);
			var headerHash = Request.Headers["X-Line-Signature"];
			//Request.Headers.GetValues("X-Line-Signature").First();

			return contentHash == headerHash;
		}
	}

	public class LineMessageHandler
	{

		private Event lineEvent;
		private LineClient lineClient = new LineClient(@"VC/sPEIz8O7WYo0THlaM5laorgw+GifJmN8cFR5eZ0seYxNsR3ZOVblQgOeI8xNaCOyTsUz2VsajbtAyt8hj7+NdP2/oYB+7eQ/FGnEAN/ICGCPj5nX36E848piCYi16BPkXlDR3N0CDiPnrfbpPcAdB04t89/1O/w1cDnyilFU="); //ConfigurationManager.AppSettings["ChannelToken"].ToString());

		public LineMessageHandler(Event lineEvent)
		{
			this.lineEvent = lineEvent;
		}

		public async Task HandleBeaconEvent()
		{
		}

		public async Task HandleFollowEvent()
		{
		}

		public async Task HandleJoinEvent()
		{
		}

		public async Task HandleLeaveEvent()
		{
		}

		public async Task HandlePostbackEvent()
		{
			var replyMessage = new TextMessage(lineEvent.Postback.Data);
			await Reply(replyMessage);
		}

		public async Task HandleUnfollowEvent()
		{
		}

		public async Task<Profile> GetProfile(string mid)
		{
			return await lineClient.GetProfile(mid);
		}

		public async Task HandleTextMessage()
		{
			var textMessage = JsonConvert.DeserializeObject<TextMessage>(lineEvent.Message.ToString());
			if(textMessage.Text== null  || textMessage.Text == "")
			{
				textMessage.Text = LineMessagesController.temp;
				textMessage.Id = "325708";
				textMessage.Type = MessageType.Text;
			}
			Message replyMessage = null;
			if (textMessage.Text.ToLower() == "buttons")
			{
				List<TemplateAction> actions = new List<TemplateAction>();
				actions.Add(new MessageTemplateAction("Message Label", "sample data"));
				actions.Add(new PostbackTemplateAction("Postback Label", "sample data", "sample data"));
				actions.Add(new UriTemplateAction("Uri Label", "https://github.com/kenakamu"));
				ButtonsTemplate buttonsTemplate = new ButtonsTemplate("https://github.com/apple-touch-icon.png", "Sample Title", "Sample Text", actions);

				replyMessage = new TemplateMessage("Buttons", buttonsTemplate);
			}
			else if (textMessage.Text.ToLower() == "confirm")
			{
				List<TemplateAction> actions = new List<TemplateAction>();
				actions.Add(new MessageTemplateAction("Yes", "yes"));
				actions.Add(new MessageTemplateAction("No", "no"));
				ConfirmTemplate confirmTemplate = new ConfirmTemplate("Confirm Test", actions);
				replyMessage = new TemplateMessage("Confirm", confirmTemplate);
			}
			else if (textMessage.Text.ToLower() == "carousel")
			{
				List<TemplateColumn> columns = new List<TemplateColumn>();
				List<TemplateAction> actions = new List<TemplateAction>();
				actions.Add(new MessageTemplateAction("Message Label", "sample data"));
				actions.Add(new PostbackTemplateAction("Postback Label", "sample data", "sample data"));
				actions.Add(new UriTemplateAction("Uri Label", "https://github.com/kenakamu"));
				columns.Add(new TemplateColumn() { Title = "Casousel 1 Title", Text = "Casousel 1 Text", ThumbnailImageUrl = "https://github.com/apple-touch-icon.png", Actions = actions });
				columns.Add(new TemplateColumn() { Title = "Casousel 2 Title", Text = "Casousel 2 Text", ThumbnailImageUrl = "https://github.com/apple-touch-icon.png", Actions = actions });
				CarouselTemplate carouselTemplate = new CarouselTemplate(columns);
				replyMessage = new TemplateMessage("Carousel", carouselTemplate);
			}
			else if (textMessage.Text.ToLower() == "imagemap")
			{

				var imageUrl = "";
				List<ImageMapAction> actions = new List<ImageMapAction>();
				actions.Add(new UriImageMapAction("http://github.com", new ImageMapArea(0, 0, 520, 1040)));
				actions.Add(new MessageImageMapAction("I love LINE!", new ImageMapArea(520, 0, 520, 1040)));
				replyMessage = new ImageMapMessage(imageUrl, "GitHub", new BaseSize(1040, 1040), actions);
			}
			else
			{
				if (LineMessagesController.temp != "")
				{
					replyMessage = new TextMessage(LineMessagesController.temp);
				}else
					replyMessage = new TextMessage(textMessage.Text);
			}
			await Reply(replyMessage);
		}

		public string MyMethod(HttpContext context)
		{
			var imageUrl = $"{context.Request.Scheme}://{context.Request.Host}/images/githubicon";
			return imageUrl;
		}

		public async Task HandleMediaMessage()
		{
			Message message = JsonConvert.DeserializeObject<Message>(lineEvent.Message.ToString());
			// Get media from Line server.
			Media media = await lineClient.GetContent(message.Id);
			Message replyMessage = null;

			// Reply Image 
			switch (message.Type)
			{
				case MessageType.Image:
				case MessageType.Video:
				case MessageType.Audio:
					replyMessage = new ImageMessage("https://github.com/apple-touch-icon.png", "https://github.com/apple-touch-icon.png");
					break;
			}

			await Reply(replyMessage);
		}

		public async Task HandleStickerMessage()
		{
			//https://devdocs.line.me/files/sticker_list.pdf
			var stickerMessage = JsonConvert.DeserializeObject<StickerMessage>(lineEvent.Message.ToString());
			var replyMessage = new StickerMessage("1", "1");
			await Reply(replyMessage);
		}

		public async Task HandleLocationMessage()
		{
			var locationMessage = JsonConvert.DeserializeObject<LocationMessage>(lineEvent.Message.ToString());
			LocationMessage replyMessage = new LocationMessage(
				locationMessage.Title,
				locationMessage.Address,
				locationMessage.Latitude,
				locationMessage.Longitude);
			await Reply(replyMessage);
		}

		private async Task Reply(Message replyMessage)
		{
			try
			{
				await lineClient.ReplyToActivityAsync(lineEvent.CreateReply(message: replyMessage));
			}
			catch
			{
				await lineClient.PushAsync(lineEvent.CreatePush(message: replyMessage));
			}
		}
	}
}