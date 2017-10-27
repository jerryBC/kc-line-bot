using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;

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
		public string Post(HttpRequestMessage request)
		{
			return "value";
		}
	}
}