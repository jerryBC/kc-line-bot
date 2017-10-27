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
		public string Post34(HttpRequestMessage request)
		{
			return "valuePOST1";
		}

		[HttpPost]
		async Task<HttpResponseMessage> Post(HttpRequestMessage request)
		{
			return new HttpResponseMessage(System.Net.HttpStatusCode.OK);
		}

		[HttpPost]
		async Task<HttpResponseMessage> Post3(HttpRequestMessage request)
		{
			return new HttpResponseMessage(System.Net.HttpStatusCode.OK);
		}
	}
}