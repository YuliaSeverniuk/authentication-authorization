using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskAuthenticationAuthorization.Models;

namespace TaskAuthenticationAuthorization.Controllers
{
    [Authorize(Policy = Startup.DiscountPolicy)]
    public class DiscountController : Controller
    {
        // GET: Discount
        public IActionResult Index()
        {
            var buyerType = User.FindFirst(BuyerTypes.ClaimType)?.Value;
            ViewData["BuyerType"] = buyerType;
            ViewData["Discount"] = buyerType == BuyerTypes.Wholesale ? 15 : 10;
            return View();
        }
    }
}
