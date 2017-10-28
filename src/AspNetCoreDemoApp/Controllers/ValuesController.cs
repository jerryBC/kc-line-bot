using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Threading.Tasks;

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
					return new HttpResponseMessage(System.Net.HttpStatusCode.OK);
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