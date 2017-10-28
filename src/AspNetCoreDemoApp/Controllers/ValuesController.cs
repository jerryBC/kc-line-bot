using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Threading.Tasks;
using AspNetCoreDemoApp.Models;

namespace AspNetCoreDemoApp.Controllers
{
	[Route("api/[controller]")]
	public class ValuesController : Controller
	{
		// GET: api/values
		[HttpGet]
		public IEnumerable<string> Get()
		{
			return new[] { "value1", "value2" };
		}

		// GET api/values/5
		[HttpGet("{id}")]
		public string Get(int id)
		{
			return "value";
		}

		[HttpPost]
		public async Task<HttpResponseMessage> Post(HttpRequestMessage request)
		{
			// do some thing
			if (request != null)
			{
				if (request.Content != null)
				{
					string temp = request.Content.ToString();

					Event ev = new Event();

					ev.Message = "{{\"id\": \"325708\",\"type\": \"text\",\"text\": \"Hello, world\"}}";
					ev.ReplyToken = @"VC/sPEIz8O7WYo0THlaM5laorgw+GifJmN8cFR5eZ0seYxNsR3ZOVblQgOeI8xNaCOyTsUz2VsajbtAyt8hj7+NdP2/oYB+7eQ/FGnEAN/ICGCPj5nX36E848piCYi16BPkXlDR3N0CDiPnrfbpPcAdB04t89/1O/w1cDnyilFU=";


					LineMessageHandler handler1 = new LineMessageHandler(ev);

					if (ev != null)
					{
						return new HttpResponseMessage(System.Net.HttpStatusCode.Ambiguous);
					}
					else
					{
						return new HttpResponseMessage(System.Net.HttpStatusCode.BadGateway);
					}

					//temp = (await request.Content.ReadAsStringAsync()).ToString();
					//await handler1.HandleTextMessage();

					//return new HttpResponseMessage(System.Net.HttpStatusCode.OK);
				}
				else
				{
					return new HttpResponseMessage(System.Net.HttpStatusCode.NotImplemented);
				}
			}
			else
			{
				return new HttpResponseMessage(System.Net.HttpStatusCode.NotAcceptable);
			}
			
		}
	}
}