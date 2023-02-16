using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using tx_text_snippets.Models;

namespace tx_text_snippets.Controllers {
	public class BuilderController : Controller {

      public IActionResult Index() {
         return View();
      }
   }
}
