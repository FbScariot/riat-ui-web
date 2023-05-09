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
    public class DoencaHistoriaClinicasController : Controller
    {
        private readonly RIATContext _context;

        public DoencaHistoriaClinicasController(RIATContext context)
        {
            _context = context;
        }

        // GET: DoencaHistoriaClinicas
        public async Task<IActionResult> Index()
        {
            var rIATContext = _context.DoencaHistoriaClinicas.Include(d => d.DoenIdDoencaNavigation)                                                              
                                                             .Include(d => d.HiclIdHistoriaClinicaNavigation)
                                                                .ThenInclude(h => h.PaciIdPacienteNavigation)
                                                                    .ThenInclude(p => p.IdPessoaNavigation)
                                                                        .ThenInclude(p => p.AspuIdAspnetuserNavigation);
            return View(await rIATContext.ToListAsync());
        }

        // GET: DoencaHistoriaClinicas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var doencaHistoriaClinica = await _context.DoencaHistoriaClinicas.Include(d => d.DoenIdDoencaNavigation)
                                                                             .Include(d => d.HiclIdHistoriaClinicaNavigation)
                                                                                .ThenInclude(h => h.PaciIdPacienteNavigation)
                                                                                    .ThenInclude(p => p.IdPessoaNavigation)
                                                                                        .ThenInclude(p => p.AspuIdAspnetuserNavigation)
                                                                             .FirstOrDefaultAsync(m => m.Id == id);
            if (doencaHistoriaClinica == null)
            {
                return NotFound();
            }

            return View(doencaHistoriaClinica);
        }

        // GET: DoencaHistoriaClinicas/Create
        public IActionResult Create()
        {
            ViewData["DoenIdDoenca"] = new SelectList(_context.Doencas, "Id", "DoenCodDoenca");
            ViewData["HiclIdHistoriaClinica"] = new SelectList(_context.HistoriaClinicas, "Id", "HiclStAtivo");
            return View();
        }

        // POST: DoencaHistoriaClinicas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administradores, Profissionais")]
        public async Task<IActionResult> Create([Bind("Id,HiclIdHistoriaClinica,DoenIdDoenca")] DoencaHistoriaClinica doencaHistoriaClinica)
        {
            if (ModelState.IsValid)
            {
                _context.Add(doencaHistoriaClinica);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["DoenIdDoenca"] = new SelectList(_context.Doencas, "Id", "DoenCodDoenca", doencaHistoriaClinica.DoenIdDoenca);
            ViewData["HiclIdHistoriaClinica"] = new SelectList(_context.HistoriaClinicas, "Id", "HiclStAtivo", doencaHistoriaClinica.HiclIdHistoriaClinica);
            return View(doencaHistoriaClinica);
        }

        // GET: DoencaHistoriaClinicas/Edit/5
        [Authorize(Roles = "Administradores, Profissionais")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var doencaHistoriaClinica = await _context.DoencaHistoriaClinicas.FindAsync(id);
            if (doencaHistoriaClinica == null)
            {
                return NotFound();
            }
            ViewData["DoenIdDoenca"] = new SelectList(_context.Doencas, "Id", "DoenCodDoenca", doencaHistoriaClinica.DoenIdDoenca);
            ViewData["HiclIdHistoriaClinica"] = new SelectList(_context.HistoriaClinicas, "Id", "HiclStAtivo", doencaHistoriaClinica.HiclIdHistoriaClinica);
            return View(doencaHistoriaClinica);
        }

        // POST: DoencaHistoriaClinicas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administradores, Profissionais")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,HiclIdHistoriaClinica,DoenIdDoenca")] DoencaHistoriaClinica doencaHistoriaClinica)
        {
            if (id != doencaHistoriaClinica.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(doencaHistoriaClinica);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DoencaHistoriaClinicaExists(doencaHistoriaClinica.Id))
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
            ViewData["DoenIdDoenca"] = new SelectList(_context.Doencas, "Id", "DoenCodDoenca", doencaHistoriaClinica.DoenIdDoenca);
            ViewData["HiclIdHistoriaClinica"] = new SelectList(_context.HistoriaClinicas, "Id", "HiclStAtivo", doencaHistoriaClinica.HiclIdHistoriaClinica);
            return View(doencaHistoriaClinica);
        }

        // GET: DoencaHistoriaClinicas/Delete/5
        [Authorize(Roles = "Administradores, Profissionais")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var doencaHistoriaClinica = await _context.DoencaHistoriaClinicas.Include(d => d.DoenIdDoencaNavigation)
                                                                             .Include(d => d.HiclIdHistoriaClinicaNavigation)
                                                                                .ThenInclude(h => h.PaciIdPacienteNavigation)
                                                                                    .ThenInclude(p => p.IdPessoaNavigation)
                                                                                        .ThenInclude(p => p.AspuIdAspnetuserNavigation)
                                                                             .FirstOrDefaultAsync(m => m.Id == id);
            if (doencaHistoriaClinica == null)
            {
                return NotFound();
            }

            return View(doencaHistoriaClinica);
        }

        // POST: DoencaHistoriaClinicas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administradores, Profissionais")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var doencaHistoriaClinica = await _context.DoencaHistoriaClinicas.FindAsync(id);
            _context.DoencaHistoriaClinicas.Remove(doencaHistoriaClinica);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DoencaHistoriaClinicaExists(int id)
        {
            return _context.DoencaHistoriaClinicas.Any(e => e.Id == id);
        }
    }
}
