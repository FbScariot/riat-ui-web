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
    public class SintomaDoencaHistoriaclinicasController : Controller
    {
        private readonly RIATContext _context;

        public SintomaDoencaHistoriaclinicasController(RIATContext context)
        {
            _context = context;
        }

        // GET: SintomaDoencaHistoriaclinicas
        public async Task<IActionResult> Index()
        {
            var rIATContext = _context.SintomaDoencaHistoriaclinicas.Include(s => s.DohcIdDoencaHistoriaClinicaNavigation)
                                                                        .ThenInclude(d => d.DoenIdDoencaNavigation)
                                                                    .Include(s => s.IdAtendimentoNavigation)
                                                                        .ThenInclude(a => a.PaciIdPacienteNavigation)
                                                                            .ThenInclude(p => p.IdPessoaNavigation)
                                                                                .ThenInclude(p => p.AspuIdAspnetuserNavigation)
                                                                    .Include(s => s.IdAtendimentoNavigation)
                                                                        .ThenInclude(a => a.ProfIdProfissionalNavigation)
                                                                            .ThenInclude(p => p.IdPessoaNavigation)
                                                                                .ThenInclude(p => p.AspuIdAspnetuserNavigation)
                                                                    .Include(s => s.SintIdSintomaNavigation);
            return View(await rIATContext.ToListAsync());
        }

        // GET: SintomaDoencaHistoriaclinicas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sintomaDoencaHistoriaclinica = await _context.SintomaDoencaHistoriaclinicas.Include(s => s.DohcIdDoencaHistoriaClinicaNavigation)
                                                                                                .ThenInclude(d => d.DoenIdDoencaNavigation)
                                                                                            .Include(s => s.IdAtendimentoNavigation)
                                                                                                .ThenInclude(a => a.PaciIdPacienteNavigation)
                                                                                                    .ThenInclude(p => p.IdPessoaNavigation)
                                                                                                        .ThenInclude(p => p.AspuIdAspnetuserNavigation)
                                                                                            .Include(s => s.IdAtendimentoNavigation)
                                                                                                .ThenInclude(a => a.ProfIdProfissionalNavigation)
                                                                                                    .ThenInclude(p => p.IdPessoaNavigation)
                                                                                                        .ThenInclude(p => p.AspuIdAspnetuserNavigation)
                                                                                            .Include(s => s.SintIdSintomaNavigation)
                                                                                            .FirstOrDefaultAsync(m => m.Id == id);
            if (sintomaDoencaHistoriaclinica == null)
            {
                return NotFound();
            }

            return View(sintomaDoencaHistoriaclinica);
        }

        // GET: SintomaDoencaHistoriaclinicas/Create
        public IActionResult Create()
        {
            ViewData["DohcIdDoencaHistoriaClinica"] = new SelectList(_context.DoencaHistoriaClinicas, "Id", "Id");
            ViewData["IdAtendimento"] = new SelectList(_context.Atendimentos, "Id", "AtenStAgendamento");
            ViewData["SintIdSintoma"] = new SelectList(_context.Sintomas, "Id", "SintCodSintoma");
            return View();
        }

        // POST: SintomaDoencaHistoriaclinicas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administradores, Profissionais")]
        public async Task<IActionResult> Create([Bind("Id,DohcIdDoencaHistoriaClinica,SintIdSintoma,SidhCdPeso,IdAtendimento")] SintomaDoencaHistoriaclinica sintomaDoencaHistoriaclinica)
        {
            if (ModelState.IsValid)
            {
                _context.Add(sintomaDoencaHistoriaclinica);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["DohcIdDoencaHistoriaClinica"] = new SelectList(_context.DoencaHistoriaClinicas, "Id", "Id", sintomaDoencaHistoriaclinica.DohcIdDoencaHistoriaClinica);
            ViewData["IdAtendimento"] = new SelectList(_context.Atendimentos, "Id", "AtenStAgendamento", sintomaDoencaHistoriaclinica.IdAtendimento);
            ViewData["SintIdSintoma"] = new SelectList(_context.Sintomas, "Id", "SintCodSintoma", sintomaDoencaHistoriaclinica.SintIdSintoma);
            return View(sintomaDoencaHistoriaclinica);
        }

        // GET: SintomaDoencaHistoriaclinicas/Edit/5
        [Authorize(Roles = "Administradores, Profissionais")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sintomaDoencaHistoriaclinica = await _context.SintomaDoencaHistoriaclinicas.FindAsync(id);
            if (sintomaDoencaHistoriaclinica == null)
            {
                return NotFound();
            }
            ViewData["DohcIdDoencaHistoriaClinica"] = new SelectList(_context.DoencaHistoriaClinicas, "Id", "Id", sintomaDoencaHistoriaclinica.DohcIdDoencaHistoriaClinica);
            ViewData["IdAtendimento"] = new SelectList(_context.Atendimentos, "Id", "AtenStAgendamento", sintomaDoencaHistoriaclinica.IdAtendimento);
            ViewData["SintIdSintoma"] = new SelectList(_context.Sintomas, "Id", "SintCodSintoma", sintomaDoencaHistoriaclinica.SintIdSintoma);
            return View(sintomaDoencaHistoriaclinica);
        }

        // POST: SintomaDoencaHistoriaclinicas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administradores, Profissionais")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,DohcIdDoencaHistoriaClinica,SintIdSintoma,SidhCdPeso,IdAtendimento")] SintomaDoencaHistoriaclinica sintomaDoencaHistoriaclinica)
        {
            if (id != sintomaDoencaHistoriaclinica.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(sintomaDoencaHistoriaclinica);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SintomaDoencaHistoriaclinicaExists(sintomaDoencaHistoriaclinica.Id))
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
            ViewData["DohcIdDoencaHistoriaClinica"] = new SelectList(_context.DoencaHistoriaClinicas, "Id", "Id", sintomaDoencaHistoriaclinica.DohcIdDoencaHistoriaClinica);
            ViewData["IdAtendimento"] = new SelectList(_context.Atendimentos, "Id", "AtenStAgendamento", sintomaDoencaHistoriaclinica.IdAtendimento);
            ViewData["SintIdSintoma"] = new SelectList(_context.Sintomas, "Id", "SintCodSintoma", sintomaDoencaHistoriaclinica.SintIdSintoma);
            return View(sintomaDoencaHistoriaclinica);
        }

        // GET: SintomaDoencaHistoriaclinicas/Delete/5
        [Authorize(Roles = "Administradores, Profissionais")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sintomaDoencaHistoriaclinica = await _context.SintomaDoencaHistoriaclinicas.Include(s => s.DohcIdDoencaHistoriaClinicaNavigation)
                                                                                                .ThenInclude(d => d.DoenIdDoencaNavigation)
                                                                                            .Include(s => s.IdAtendimentoNavigation)
                                                                                                .ThenInclude(a => a.PaciIdPacienteNavigation)
                                                                                                    .ThenInclude(p => p.IdPessoaNavigation)
                                                                                                        .ThenInclude(p => p.AspuIdAspnetuserNavigation)
                                                                                            .Include(s => s.IdAtendimentoNavigation)
                                                                                                .ThenInclude(a => a.ProfIdProfissionalNavigation)
                                                                                                    .ThenInclude(p => p.IdPessoaNavigation)
                                                                                                        .ThenInclude(p => p.AspuIdAspnetuserNavigation)
                                                                                            .Include(s => s.SintIdSintomaNavigation)
                                                                                            .FirstOrDefaultAsync(m => m.Id == id);
            if (sintomaDoencaHistoriaclinica == null)
            {
                return NotFound();
            }

            return View(sintomaDoencaHistoriaclinica);
        }

        // POST: SintomaDoencaHistoriaclinicas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administradores, Profissionais")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var sintomaDoencaHistoriaclinica = await _context.SintomaDoencaHistoriaclinicas.FindAsync(id);
            _context.SintomaDoencaHistoriaclinicas.Remove(sintomaDoencaHistoriaclinica);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SintomaDoencaHistoriaclinicaExists(int id)
        {
            return _context.SintomaDoencaHistoriaclinicas.Any(e => e.Id == id);
        }
    }
}
