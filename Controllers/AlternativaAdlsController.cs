using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RIAT.DAL.Entity.Data;
using RIAT.DAL.Entity.Models;

namespace RIAT.UI.Web.Controllers
{
    [Authorize]
    public class AlternativaAdlsController : Controller
    {
        private readonly RIATContext _context;

        public AlternativaAdlsController(RIATContext context)
        {
            _context = context;
        }

        // GET: AlternativaAdls
        public async Task<IActionResult> Index()
        {
            return View(await _context.AlternativaAdls.ToListAsync());
        }

        // GET: AlternativaAdls/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var alternativaAdl = await _context.AlternativaAdls
                .FirstOrDefaultAsync(m => m.Id == id);
            if (alternativaAdl == null)
            {
                return NotFound();
            }

            return View(alternativaAdl);
        }

        // GET: AlternativaAdls/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: AlternativaAdls/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Codigo,Nome,Descricao,TipoConceito")] AlternativaAdl alternativaAdl)
        {
            if (ModelState.IsValid)
            {
                _context.Add(alternativaAdl);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(alternativaAdl);
        }

        // GET: AlternativaAdls/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var alternativaAdl = await _context.AlternativaAdls.FindAsync(id);
            if (alternativaAdl == null)
            {
                return NotFound();
            }
            return View(alternativaAdl);
        }

        // POST: AlternativaAdls/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Codigo,Nome,Descricao,TipoConceito")] AlternativaAdl alternativaAdl)
        {
            if (id != alternativaAdl.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(alternativaAdl);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AlternativaAdlExists(alternativaAdl.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(alternativaAdl);
        }

        // GET: AlternativaAdls/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var alternativaAdl = await _context.AlternativaAdls
                .FirstOrDefaultAsync(m => m.Id == id);
            if (alternativaAdl == null)
            {
                return NotFound();
            }

            return View(alternativaAdl);
        }

        // POST: AlternativaAdls/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var alternativaAdl = await _context.AlternativaAdls.FindAsync(id);
            _context.AlternativaAdls.Remove(alternativaAdl);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AlternativaAdlExists(int id)
        {
            return _context.AlternativaAdls.Any(e => e.Id == id);
        }
    }
}
