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
    public class ProfissionalsController : Controller
    {
        private readonly RIATContext _context;

        public ProfissionalsController(RIATContext context)
        {
            _context = context;
        }

        // GET: Profissionals
        public async Task<IActionResult> Index()
        {
            IQueryable<Profissional> listaProfissionals = _context.Profissionals.Include(p => p.IdPessoaNavigation).ThenInclude(p => p.AspuIdAspnetuserNavigation)                                                    
                                                    .Include(p => p.TiprIdTipoProfissionalNavigation);

            if (User.IsInRole("Administradores"))
            {

            }
            else if (User.IsInRole("Pacientes"))
            {
                listaProfissionals = listaProfissionals.Where(a => a.IdPessoaNavigation.AspuIdAspnetuserNavigation.Email == User.Identity.Name);
            }
            else if (User.IsInRole("Profissionais"))
            {
                listaProfissionals = listaProfissionals.Where(a => a.IdPessoaNavigation.AspuIdAspnetuserNavigation.Email == User.Identity.Name);
            }
            return View(await listaProfissionals.OrderBy(p =>p.IdPessoaNavigation.AspuIdAspnetuserNavigation.FirstName).ThenBy(p => p.IdPessoaNavigation.AspuIdAspnetuserNavigation.LastName).ToListAsync());
        }

        // GET: Profissionals/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var profissional = await _context.Profissionals.Include(p => p.IdPessoaNavigation).ThenInclude(p => p.AspuIdAspnetuserNavigation)
                                                           .Include(p => p.TiprIdTipoProfissionalNavigation)
                                                           .FirstOrDefaultAsync(m => m.IdPessoa == id);
            if (profissional == null)
            {
                return NotFound();
            }

            return View(profissional);
        }

        // GET: Profissionals/Create
        [Authorize(Roles = "Administradores")]

        public IActionResult Create()
        {
            var listaPacientes = _context.Pessoas
                                       .Include(p => p.AspuIdAspnetuserNavigation)
                                       .OrderBy(p => p.AspuIdAspnetuserNavigation.FirstName).ThenBy(p => p.AspuIdAspnetuserNavigation.LastName)
                                       .Select(p => new {
                                                           IdPessoa = p.Id,
                                                           p.AspuIdAspnetuserNavigation.CompleteName
                                                       }
                                              );

            ViewData["IdPessoa"] = new SelectList(listaPacientes.ToList(), "IdPessoa", "CompleteName");
            ViewData["TiprIdTipoProfissional"] = new SelectList(_context.TipoProfissionals, "Id", "TiprNmTipoProfissional");

            return View();
        }

        // POST: Profissionals/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administradores")]
        public async Task<IActionResult> Create([Bind("IdPessoa,TiprIdTipoProfissional,ProfNrCrp,ProfCdEPsi,ProfTxMiniCurriculoInterno,ProfTxMiniCurriculoPublico,ProfStAtivo,ProfDtAtivacao,ProfDtDesativacao")] Profissional profissional)
        {
            if(string.IsNullOrEmpty(profissional.ProfStAtivo))
            {
                profissional.ProfStAtivo = "A";
            }

            if (profissional.ProfDtAtivacao == DateTime.MinValue)
            {
                profissional.ProfDtAtivacao = DateTime.Now;
            }

            if (ModelState.IsValid)
            {
                _context.Add(profissional);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            var listaPacientes = _context.Pacientes.Include(p => p.IdPessoaNavigation)
                                                 .ThenInclude(p => p.AspuIdAspnetuserNavigation)
                                                 .OrderBy(p => p.IdPessoaNavigation.AspuIdAspnetuserNavigation.FirstName).ThenBy(p => p.IdPessoaNavigation.AspuIdAspnetuserNavigation.LastName)
                                                 .Select(p => new {
                                                     p.IdPessoa,
                                                     p.IdPessoaNavigation.AspuIdAspnetuserNavigation.CompleteName
                                                 }
                                                         );

            ViewData["IdPessoa"] = new SelectList(listaPacientes.ToList(), "IdPessoa", "CompleteName");
            ViewData["TiprIdTipoProfissional"] = new SelectList(_context.TipoProfissionals, "Id", "TiprNmTipoProfissional", profissional.TiprIdTipoProfissional);

            return View(profissional);
        }

        // GET: Profissionals/Edit/5
        [Authorize(Roles = "Administradores, Profissionais")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var profissional = await _context.Profissionals.FindAsync(id);
            if (profissional == null)
            {
                return NotFound();
            }

            profissional = _context.Profissionals.Include(p => p.IdPessoaNavigation).ThenInclude(p => p.AspuIdAspnetuserNavigation).FirstOrDefaultAsync(m => m.IdPessoa == id).Result;

            ViewData["TiprIdTipoProfissional"] = new SelectList(_context.TipoProfissionals, "Id", "TiprNmTipoProfissional", profissional.TiprIdTipoProfissional);

            return View(profissional);
        }

        // POST: Profissionals/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administradores, Profissionais")]
        public async Task<IActionResult> Edit(int id, [Bind("IdPessoa,TiprIdTipoProfissional,ProfNrCrp,ProfCdEPsi,ProfTxMiniCurriculoInterno,ProfTxMiniCurriculoPublico,ProfStAtivo,ProfDtAtivacao,ProfDtDesativacao")] Profissional profissional)
        {
            if (id != profissional.IdPessoa)
            {
                return NotFound();
            }

            if (string.IsNullOrEmpty(profissional.ProfStAtivo))
            {
                profissional.ProfStAtivo = "A";
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(profissional);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProfissionalExists(profissional.IdPessoa))
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

            profissional = _context.Profissionals.Include(p => p.IdPessoaNavigation).ThenInclude(p => p.AspuIdAspnetuserNavigation).FirstOrDefaultAsync(m => m.IdPessoa == id).Result;

            ViewData["TiprIdTipoProfissional"] = new SelectList(_context.TipoProfissionals, "Id", "TiprNmTipoProfissional", profissional.TiprIdTipoProfissional);

            return View(profissional);
        }

        // GET: Profissionals/Delete/5
        [Authorize(Roles = "Administradores, Profissionais")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var profissional = await _context.Profissionals.Include(p => p.IdPessoaNavigation).ThenInclude(p => p.AspuIdAspnetuserNavigation)
                                                           .Include(p => p.TiprIdTipoProfissionalNavigation)
                                                           .FirstOrDefaultAsync(m => m.IdPessoa == id);
            if (profissional == null)
            {
                return NotFound();
            }

            return View(profissional);
        }

        // POST: Profissionals/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administradores, Profissionais")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var profissional = await _context.Profissionals.FindAsync(id);
            _context.Profissionals.Remove(profissional);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProfissionalExists(int id)
        {
            return _context.Profissionals.Any(e => e.IdPessoa == id);
        }
    }
}
