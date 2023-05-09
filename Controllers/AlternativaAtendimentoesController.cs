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
    public class AlternativaAtendimentoesController : Controller
    {
        private readonly RIATContext _context;

        public AlternativaAtendimentoesController(RIATContext context)
        {
            _context = context;
        }

        // GET: AlternativaAtendimentoes
        public async Task<IActionResult> Index()
        {
            var rIATContext = _context.AlternativaAtendimentos.Include(a => a.IdAlternativaAdlNavigation).Include(a => a.IdAtendimentoNavigation);
            return View(await rIATContext.ToListAsync());
        }

        // GET: AlternativaAtendimentoes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var alternativaAtendimento = await _context.AlternativaAtendimentos
                .Include(a => a.IdAlternativaAdlNavigation)
                .Include(a => a.IdAtendimentoNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (alternativaAtendimento == null)
            {
                return NotFound();
            }

            return View(alternativaAtendimento);
        }

        // GET: AlternativaAtendimentoes/Create
        public IActionResult Create()
        {
            ViewData["IdAlternativaAdl"] = new SelectList(_context.AlternativaAdls, "Id", "Codigo");
            ViewData["IdAtendimento"] = new SelectList(_context.Atendimentos, "Id", "AtenStAgendamento");
            return View();
        }

        // POST: AlternativaAtendimentoes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,IdAtendimento,IdAlternativaAdl,Estado")] AlternativaAtendimento alternativaAtendimento)
        {
            if (ModelState.IsValid)
            {
                _context.Add(alternativaAtendimento);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdAlternativaAdl"] = new SelectList(_context.AlternativaAdls, "Id", "Codigo", alternativaAtendimento.IdAlternativaAdl);
            ViewData["IdAtendimento"] = new SelectList(_context.Atendimentos, "Id", "AtenStAgendamento", alternativaAtendimento.IdAtendimento);
            return View(alternativaAtendimento);
        }

        // GET: AlternativaAtendimentoes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var alternativaAtendimento = await _context.AlternativaAtendimentos.FindAsync(id);
            if (alternativaAtendimento == null)
            {
                return NotFound();
            }
            ViewData["IdAlternativaAdl"] = new SelectList(_context.AlternativaAdls, "Id", "Codigo", alternativaAtendimento.IdAlternativaAdl);
            ViewData["IdAtendimento"] = new SelectList(_context.Atendimentos, "Id", "AtenStAgendamento", alternativaAtendimento.IdAtendimento);
            return View(alternativaAtendimento);
        }

        // POST: AlternativaAtendimentoes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,IdAtendimento,IdAlternativaAdl,Estado")] AlternativaAtendimento alternativaAtendimento)
        {
            if (id != alternativaAtendimento.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(alternativaAtendimento);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AlternativaAtendimentoExists(alternativaAtendimento.Id))
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
            ViewData["IdAlternativaAdl"] = new SelectList(_context.AlternativaAdls, "Id", "Codigo", alternativaAtendimento.IdAlternativaAdl);
            ViewData["IdAtendimento"] = new SelectList(_context.Atendimentos, "Id", "AtenStAgendamento", alternativaAtendimento.IdAtendimento);
            return View(alternativaAtendimento);
        }

        // GET: AlternativaAtendimentoes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var alternativaAtendimento = await _context.AlternativaAtendimentos
                .Include(a => a.IdAlternativaAdlNavigation)
                .Include(a => a.IdAtendimentoNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (alternativaAtendimento == null)
            {
                return NotFound();
            }

            return View(alternativaAtendimento);
        }

        // POST: AlternativaAtendimentoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var alternativaAtendimento = await _context.AlternativaAtendimentos.FindAsync(id);
            _context.AlternativaAtendimentos.Remove(alternativaAtendimento);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AlternativaAtendimentoExists(int id)
        {
            return _context.AlternativaAtendimentos.Any(e => e.Id == id);
        }
    }
}
