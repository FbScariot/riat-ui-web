using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RIAT.DAL.Entity.Data;
using RIAT.DAL.Entity.Models;

namespace RIAT.UI.Web.Controllers
{
    public class SintomasController1 : Controller
    {
        private readonly RIATContext _context;

        public SintomasController1(RIATContext context)
        {
            _context = context;
        }

        // GET: Sintomas
        public async Task<IActionResult> Index()
        {
            var rIATContext = _context.Sintomas.Include(s => s.DoenIdDoencaNavigation);
            return View(await rIATContext.ToListAsync());
        }

        // GET: Sintomas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sintoma = await _context.Sintomas
                .Include(s => s.DoenIdDoencaNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (sintoma == null)
            {
                return NotFound();
            }

            return View(sintoma);
        }

        // GET: Sintomas/Create
        public IActionResult Create()
        {
            ViewData["DoenIdDoenca"] = new SelectList(_context.Doencas, "Id", "DoenCodDoenca");
            return View();
        }

        // POST: Sintomas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administradores, Profissionais")]
        public async Task<IActionResult> Create([Bind("Id,SintCodSintoma,SintNomeSintoma,SintDsSintoma,DoenIdDoenca")] Sintoma sintoma)
        {
            if (ModelState.IsValid)
            {
                _context.Add(sintoma);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["DoenIdDoenca"] = new SelectList(_context.Doencas, "Id", "DoenCodDoenca", sintoma.DoenIdDoenca);
            return View(sintoma);
        }

        // GET: Sintomas/Edit/5
        [Authorize(Roles = "Administradores, Profissionais")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sintoma = await _context.Sintomas.FindAsync(id);
            if (sintoma == null)
            {
                return NotFound();
            }
            ViewData["DoenIdDoenca"] = new SelectList(_context.Doencas, "Id", "DoenCodDoenca", sintoma.DoenIdDoenca);
            return View(sintoma);
        }

        // POST: Sintomas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administradores, Profissionais")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,SintCodSintoma,SintNomeSintoma,SintDsSintoma,DoenIdDoenca")] Sintoma sintoma)
        {
            if (id != sintoma.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(sintoma);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SintomaExists(sintoma.Id))
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
            ViewData["DoenIdDoenca"] = new SelectList(_context.Doencas, "Id", "DoenCodDoenca", sintoma.DoenIdDoenca);
            return View(sintoma);
        }

        // GET: Sintomas/Delete/5
        [Authorize(Roles = "Administradores, Profissionais")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sintoma = await _context.Sintomas
                .Include(s => s.DoenIdDoencaNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (sintoma == null)
            {
                return NotFound();
            }

            return View(sintoma);
        }

        // POST: Sintomas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administradores, Profissionais")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var sintoma = await _context.Sintomas.FindAsync(id);
            _context.Sintomas.Remove(sintoma);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SintomaExists(int id)
        {
            return _context.Sintomas.Any(e => e.Id == id);
        }
    }
}
