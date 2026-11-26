using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TaskAuthenticationAuthorization.Models;

namespace TaskAuthenticationAuthorization.Controllers
{
    [Authorize(Roles = Roles.Admin)]
    public class AdminController : Controller
    {
        private readonly ShoppingContext _context;

        public AdminController(ShoppingContext context)
        {
            _context = context;
        }

        // GET: Admin
        public async Task<IActionResult> Index()
        {
            var users = await _context.Users
                .Include(u => u.Customer)
                .OrderBy(u => u.Email)
                .AsNoTracking()
                .ToListAsync();
            return View(users);
        }

        // GET: Admin/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            FillSelectLists();
            return View(new EditUserViewModel
            {
                Id = user.Id,
                Email = user.Email,
                Role = user.Role,
                BuyerType = user.BuyerType
            });
        }

        // POST: Admin/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EditUserViewModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (!Roles.All.Contains(model.Role))
            {
                ModelState.AddModelError(nameof(model.Role), "Unknown role");
            }
            if (!BuyerTypes.All.Contains(model.BuyerType))
            {
                ModelState.AddModelError(nameof(model.BuyerType), "Unknown buyer type");
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                model.Email = user.Email;
                FillSelectLists();
                return View(model);
            }

            user.Role = model.Role;
            user.BuyerType = model.BuyerType;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private void FillSelectLists()
        {
            ViewData["Roles"] = new SelectList(Roles.All);
            ViewData["BuyerTypes"] = new SelectList(BuyerTypes.All);
        }
    }
}
