using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WebApplicationCompareImages.Data;
using WebApplicationCompareImages.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace WebApplicationCompareImages.Controllers
{
    public class ImagesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ImagesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Images
        public async Task<IActionResult> Index()
        {
            return View(await _context.Image.OrderByDescending(i => i.TimesSelected).ToListAsync());
        }

        // GET: Images/Compare
        public async Task<IActionResult> Search()
        {
            return View();
        }

        // GET: Images/ShowSearchResults
        public async Task<IActionResult> ShowSearchResults(string SearchPhrase)
        {
            return View("Index", await _context.Image.Where( j => j.Name.Contains(SearchPhrase)).ToListAsync());
        }

        // GET: Images/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var image = await _context.Image
                .FirstOrDefaultAsync(m => m.Id == id);
            if (image == null)
            {
                return NotFound();
            }

            return View(image);
        }

        // GET: Images/Create

        [Authorize]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Images/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Image image) //[Bind("Id,Name,TimesShown,TimesSelected,FileData")]
        {
            var file = image.FileData;

            if (file == null || file.Length <= 0)
            {
                ModelState.AddModelError("FileData", "Please upload a valid image file.");
                return View(image);
            }


            //check extention
            List<string> validExtentions = new List<string>() { ".jpg", ".png", ".gif", ".jpeg"};
            string extention = Path.GetExtension(file.FileName);
            if (!validExtentions.Contains(extention.ToLower()))
            {
                ModelState.AddModelError("FileData", "Invalid file type. Allowed types: .jpg, .jpeg, .png, .gif");
                return View(image);
            }

            //check size
            long size = file.Length;
            if(size > (20*1024*1024))
            {
                ModelState.AddModelError("FileData", "File size exceeds 20MB limit.");
                return View(image);
            }

            using (var memoryStream = new MemoryStream())
            {
                await file.CopyToAsync(memoryStream);
                image.ImageData = memoryStream.ToArray();
            }

            foreach (var entry in ModelState)
            {
                foreach (var error in entry.Value.Errors)
                {
                    Console.WriteLine($"ModelState Error for {entry.Key}: {error.ErrorMessage}");
                }
            }

            if (ModelState.IsValid)
            {
                _context.Add(image);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError("FileData", "ModelState is not valid");

            return View(image);
        }

        // GET: Images/Delete/5
        [Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var image = await _context.Image.FirstOrDefaultAsync(m => m.Id == id);

            if (image == null)
            {
                return NotFound();
            }

            return View(image);
        }

        // POST: Images/Delete/5
        [Authorize]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var image = await _context.Image.FindAsync(id);
            if (image != null)
            {
                _context.Image.Remove(image);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ImageExists(int id)
        {
            return _context.Image.Any(e => e.Id == id);
        }
    }
}
