using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Diagnostics;
using WebApplicationCompareImages.Data;
using WebApplicationCompareImages.Models;

namespace WebApplicationCompareImages.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var images = await _context.Image.ToListAsync();

            Random rnd = new Random();

            int n = images.Count;
            while (n > 1)
            {
                n--;
                int k = rnd.Next(n + 1);
                Image value = images[k];
                images[k] = images[n];
                images[n] = value;
            }

            images = images.OrderBy(i => i.TimesShown).ToList();

            if (images.Count < 2)
                return RedirectToAction(nameof(NoImages));

            return View(new List<Image>() { images[0] , images[1] });
        }

        [HttpPost]
        public async Task<IActionResult> UpdateWinner(int id1, int id2)
        {
            var selected = await _context.Image.FirstOrDefaultAsync(m => m.Id == id1);
            var other = await _context.Image.FirstOrDefaultAsync(m => m.Id == id2);

            if (selected == null || other == null)
                return NotFound();

            selected.TimesSelected++;

            selected.TimesShown++;
            other.TimesShown++;

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult NoImages()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
