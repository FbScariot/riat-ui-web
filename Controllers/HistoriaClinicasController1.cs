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
    public class HistoriaClinicasController1 : Controller
    {
        private readonly RIATContext _context;

        public HistoriaClinicasController1(RIATContext context)
        {
            _context = context;
        }

        // GET: HistoriaClinicas
        public async Task<IActionResult> Index()
        {
            var rIATContext = _context.HistoriaClinicas.Include(h => h.IdAtendimentoNavigation).Include(h => h.PaciIdPacienteNavigation);
            return View(await rIATContext.ToListAsync());
        }

        // GET: HistoriaClinicas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var historiaClinica = await _context.HistoriaClinicas
                .Include(h => h.IdAtendimentoNavigation)
                .Include(h => h.PaciIdPacienteNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (historiaClinica == null)
            {
                return NotFound();
            }

            return View(historiaClinica);
        }

        // GET: HistoriaClinicas/Create
        public IActionResult Create()
        {
            ViewData["IdAtendimento"] = new SelectList(_context.Atendimentos, "Id", "AtenStAgendamento");
            ViewData["PaciIdPaciente"] = new SelectList(_context.Pacientes, "IdPessoa", "PaciStAtivo");
            return View();
        }

        // POST: HistoriaClinicas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administradores, Profissionais")]
        public async Task<IActionResult> Create([Bind("Id,PaciIdPaciente,HiclContatoReferencia,HiclNmPlanoSaude,HiclMotivoConsulta,HiclDtAtivacao,HiclDtDesativacao,HiclStAtivo,IdAtendimento")] HistoriaClinica historiaClinica)
        {
            if (ModelState.IsValid)
            {
                _context.Add(historiaClinica);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdAtendimento"] = new SelectList(_context.Atendimentos, "Id", "AtenStAgendamento", historiaClinica.IdAtendimento);
            ViewData["PaciIdPaciente"] = new SelectList(_context.Pacientes, "IdPessoa", "PaciStAtivo", historiaClinica.PaciIdPaciente);
            return View(historiaClinica);
        }

        // GET: HistoriaClinicas/Edit/5
        [Authorize(Roles = "Administradores, Profissionais")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var historiaClinica = await _context.HistoriaClinicas.FindAsync(id);
            if (historiaClinica == null)
            {
                return NotFound();
            }
            ViewData["IdAtendimento"] = new SelectList(_context.Atendimentos, "Id", "AtenStAgendamento", historiaClinica.IdAtendimento);
            ViewData["PaciIdPaciente"] = new SelectList(_context.Pacientes, "IdPessoa", "PaciStAtivo", historiaClinica.PaciIdPaciente);
            return View(historiaClinica);
        }

        // POST: HistoriaClinicas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administradores, Profissionais")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,PaciIdPaciente,HiclContatoReferencia,HiclNmPlanoSaude,HiclMotivoConsulta,HiclDtAtivacao,HiclDtDesativacao,HiclStAtivo,IdAtendimento")] HistoriaClinica historiaClinica)
        {
            if (id != historiaClinica.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(historiaClinica);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!HistoriaClinicaExists(historiaClinica.Id))
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
            ViewData["IdAtendimento"] = new SelectList(_context.Atendimentos, "Id", "AtenStAgendamento", historiaClinica.IdAtendimento);
            ViewData["PaciIdPaciente"] = new SelectList(_context.Pacientes, "IdPessoa", "PaciStAtivo", historiaClinica.PaciIdPaciente);
            return View(historiaClinica);
        }

        // GET: HistoriaClinicas/Delete/5
        [Authorize(Roles = "Administradores, Profissionais")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var historiaClinica = await _context.HistoriaClinicas
                .Include(h => h.IdAtendimentoNavigation)
                .Include(h => h.PaciIdPacienteNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (historiaClinica == null)
            {
                return NotFound();
            }

            return View(historiaClinica);
        }

        // POST: HistoriaClinicas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administradores, Profissionais")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var historiaClinica = await _context.HistoriaClinicas.FindAsync(id);
            _context.HistoriaClinicas.Remove(historiaClinica);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool HistoriaClinicaExists(int id)
        {
            return _context.HistoriaClinicas.Any(e => e.Id == id);
        }
    }
}
