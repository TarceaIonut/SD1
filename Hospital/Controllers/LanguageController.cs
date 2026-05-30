using Hospital.Models.ViewModels;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;

namespace Hospital.Controllers;

public class LanguageController : Controller
{
    [HttpPost]
    public IActionResult SetLanguage(string culture, string returnUrl) {
        Response.Cookies.Append(
            CookieRequestCultureProvider.DefaultCookieName,
            CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
            new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
        );
        return LocalRedirect(returnUrl);
    }
    [HttpGet]
    public IActionResult Index() {
        var rqf = Request.HttpContext.Features.Get<IRequestCultureFeature>();
        string currentCulture = rqf?.RequestCulture.Culture.Name ?? "en-US";
        
        string friendlyName = currentCulture switch
        {
            "ro-RO" => "Română",
            "de-DE" => "Deutsch",
            _ => "English"
        };
        return View("Language", new LanguageView{Language = friendlyName});
    }
}