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
    public class Pacientes1Controller : Controller
    {
        private readonly RIATContext _context;

        public Pacientes1Controller(RIATContext context)
        {
            _context = context;
        }

        // GET: Pacientes
        public async Task<IActionResult> Index()
        {
            var rIATContext = _context.Pacientes.Include(p => p.GrinIdGrauInstrucaoNavigation).Include(p => p.IdPessoaNavigation);
            return View(await rIATContext.ToListAsync());
        }

        // GET: Pacientes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var paciente = await _context.Pacientes
                .Include(p => p.GrinIdGrauInstrucaoNavigation)
                .Include(p => p.IdPessoaNavigation)
                .FirstOrDefaultAsync(m => m.IdPessoa == id);
            if (paciente == null)
            {
                return NotFound();
            }

            return View(paciente);
        }

        // GET: Pacientes/Create
        public IActionResult Create()
        {
            ViewData["GrinIdGrauInstrucao"] = new SelectList(_context.GrauInstrucaos, "Id", "GrinNmGrauInstrucao");
            ViewData["IdPessoa"] = new SelectList(_context.Pessoas, "Id", "AspuIdAspnetuser");
            return View();
        }

        // POST: Pacientes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administradores, Profissionais")]
        public async Task<IActionResult> Create([Bind("IdPessoa,PaciCdPassaporte,PaciCdRegistroGeral,PaciStEstadoCivil,PaciNmMae,PaciNmPai,PaciNmProfissao,PaciTxEncaminhadoPor,PaciStAtivo,PaciDtAtivacao,PaciDtDesativacao,GrinIdGrauInstrucao,PaciTpContratacao")] Paciente paciente)
        {
            if (ModelState.IsValid)
            {
                _context.Add(paciente);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["GrinIdGrauInstrucao"] = new SelectList(_context.GrauInstrucaos, "Id", "GrinNmGrauInstrucao", paciente.GrinIdGrauInstrucao);
            ViewData["IdPessoa"] = new SelectList(_context.Pessoas, "Id", "AspuIdAspnetuser", paciente.IdPessoa);
            return View(paciente);
        }

        // GET: Pacientes/Edit/5
        [Authorize(Roles = "Administradores, Profissionais")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var paciente = await _context.Pacientes.FindAsync(id);
            if (paciente == null)
            {
                return NotFound();
            }
            ViewData["GrinIdGrauInstrucao"] = new SelectList(_context.GrauInstrucaos, "Id", "GrinNmGrauInstrucao", paciente.GrinIdGrauInstrucao);
            ViewData["IdPessoa"] = new SelectList(_context.Pessoas, "Id", "AspuIdAspnetuser", paciente.IdPessoa);
            return View(paciente);
        }

        // POST: Pacientes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administradores, Profissionais")]
        public async Task<IActionResult> Edit(int id, [Bind("IdPessoa,PaciCdPassaporte,PaciCdRegistroGeral,PaciStEstadoCivil,PaciNmMae,PaciNmPai,PaciNmProfissao,PaciTxEncaminhadoPor,PaciStAtivo,PaciDtAtivacao,PaciDtDesativacao,GrinIdGrauInstrucao,PaciTpContratacao")] Paciente paciente)
        {
            if (id != paciente.IdPessoa)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(paciente);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PacienteExists(paciente.IdPessoa))
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
            ViewData["GrinIdGrauInstrucao"] = new SelectList(_context.GrauInstrucaos, "Id", "GrinNmGrauInstrucao", paciente.GrinIdGrauInstrucao);
            ViewData["IdPessoa"] = new SelectList(_context.Pessoas, "Id", "AspuIdAspnetuser", paciente.IdPessoa);
            return View(paciente);
        }

        // GET: Pacientes/Delete/5
        [Authorize(Roles = "Administradores, Profissionais")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var paciente = await _context.Pacientes
                .Include(p => p.GrinIdGrauInstrucaoNavigation)
                .Include(p => p.IdPessoaNavigation)
                .FirstOrDefaultAsync(m => m.IdPessoa == id);
            if (paciente == null)
            {
                return NotFound();
            }

            return View(paciente);
        }

        // POST: Pacientes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administradores, Profissionais")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var paciente = await _context.Pacientes.FindAsync(id);
            _context.Pacientes.Remove(paciente);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PacienteExists(int id)
        {
            return _context.Pacientes.Any(e => e.IdPessoa == id);
        }
    }
}
