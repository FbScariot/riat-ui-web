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
    public class DoencasController1 : Controller
    {
        private readonly RIATContext _context;

        public DoencasController1(RIATContext context)
        {
            _context = context;
        }

        // GET: Doencas
        public async Task<IActionResult> Index()
        {
            return View(await _context.Doencas.ToListAsync());
        }

        // GET: Doencas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var doenca = await _context.Doencas
                .FirstOrDefaultAsync(m => m.Id == id);
            if (doenca == null)
            {
                return NotFound();
            }

            return View(doenca);
        }

        // GET: Doencas/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Doencas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administradores, Profissionais")]
        public async Task<IActionResult> Create([Bind("DoenCodDoenca,Id,DoenNmHipotese")] Doenca doenca)
        {
            if (ModelState.IsValid)
            {
                _context.Add(doenca);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(doenca);
        }

        // GET: Doencas/Edit/5
        [Authorize(Roles = "Administradores, Profissionais")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var doenca = await _context.Doencas.FindAsync(id);
            if (doenca == null)
            {
                return NotFound();
            }
            return View(doenca);
        }

        // POST: Doencas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administradores, Profissionais")]
        public async Task<IActionResult> Edit(int id, [Bind("DoenCodDoenca,Id,DoenNmHipotese")] Doenca doenca)
        {
            if (id != doenca.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(doenca);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DoencaExists(doenca.Id))
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
            return View(doenca);
        }

        // GET: Doencas/Delete/5
        [Authorize(Roles = "Administradores, Profissionais")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var doenca = await _context.Doencas
                .FirstOrDefaultAsync(m => m.Id == id);
            if (doenca == null)
            {
                return NotFound();
            }

            return View(doenca);
        }

        // POST: Doencas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administradores, Profissionais")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var doenca = await _context.Doencas.FindAsync(id);
            _context.Doencas.Remove(doenca);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DoencaExists(int id)
        {
            return _context.Doencas.Any(e => e.Id == id);
        }
    }
}
