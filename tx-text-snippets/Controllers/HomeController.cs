using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using tx_text_snippets.Models;

namespace tx_text_snippets.Controllers {
	public class HomeController : Controller {


		public IActionResult Index() {
			return View();
		}


	}
}