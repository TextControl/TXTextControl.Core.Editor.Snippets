using HtmlAgilityPack;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using tx_text_snippets.Models;

namespace tx_text_snippets.Controllers {
	public class SnippetsController : Controller {

      private ApplicationOptions _applicationSettings;

      public SnippetsController(IOptions<ApplicationOptions> options) {
         _applicationSettings = options.Value;
      }

      public IActionResult Index() {

         var test = _applicationSettings.SnippetDirectory;

         return View(Directory.GetFiles(_applicationSettings.SnippetDirectory, "*.tx").Select(Path.GetFileName));         
		}

      public IActionResult New(string snippetName = null) {
         ViewBag.SnippetDirectory = _applicationSettings.SnippetDirectory;
         return View((object)(snippetName));
      }

      public IActionResult SnippetListPartial() {
         var snippets = Directory.GetFiles(_applicationSettings.SnippetDirectory, "*.tx").Select(Path.GetFileName);
         return PartialView("_SnippetListPartial", snippets);
      }

      [HttpPost]
      public bool Create(string document, string name) {
         
         var snippetName = _applicationSettings.SnippetDirectory + "/" + Path.GetInvalidFileNameChars().Aggregate(name, (f, c) => f.Replace(c, '_').Replace(' ', '_'));

         using (TXTextControl.ServerTextControl tx = new TXTextControl.ServerTextControl()) {
            tx.Create();
            tx.Load(Convert.FromBase64String(document), TXTextControl.BinaryStreamType.InternalUnicodeFormat);

            tx.Select(0, 200);

            string htmlString;

            tx.Selection.Save(out htmlString, TXTextControl.StringStreamType.HTMLFormat);

            TextReader reader = new StringReader(htmlString);
            HtmlDocument doc = new HtmlDocument();
            doc.Load(reader);
            HtmlNode bodyNode = doc.DocumentNode.SelectSingleNode("/html/body");
            System.IO.File.WriteAllText(snippetName + ".htm", bodyNode.InnerHtml);
         }

         System.IO.File.WriteAllBytes(snippetName + ".tx", Convert.FromBase64String(document));
         return true;
      }

      [HttpPost]
      public IEnumerable<string> List() {
         return Directory.GetFiles(_applicationSettings.SnippetDirectory, "*.tx").Select(Path.GetFileName);
      }

      [HttpPost]
      public bool Delete(string name) { 

         var snippetName = Path.GetFileNameWithoutExtension(name);

         System.IO.File.Delete(_applicationSettings.SnippetDirectory + "/" + snippetName + ".htm");
         System.IO.File.Delete(_applicationSettings.SnippetDirectory + "/" + snippetName + ".tx");
         return true;
      }

      [HttpPost]
      public string Content(string name, string format) {

         var snippetName = Path.GetFileNameWithoutExtension(name);

         if (format == "HTM") { 
            return System.IO.File.ReadAllText((_applicationSettings.SnippetDirectory + "/" + snippetName + ".htm"));
         }
         else {
            return Convert.ToBase64String(System.IO.File.ReadAllBytes((_applicationSettings.SnippetDirectory + "/" + snippetName + ".tx")));
         }
      }


   }
}
