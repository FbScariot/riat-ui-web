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
    public class DoencaPacientesController : Controller
    {
        private readonly RIATContext _context;

        public DoencaPacientesController(RIATContext context)
        {
            _context = context;
        }

        // GET: DoencaPacientes
        public async Task<IActionResult> Index(int paciIdPaciente)
        {
            IQueryable<DoencaPaciente> doencaPacientes = _context.DoencaPacientes.Include(d => d.DoenIdDoencaNavigation)
                                                                                 .Include(d => d.PaciIdPacienteNavigation).ThenInclude(p => p.IdPessoaNavigation).ThenInclude(p => p.AspuIdAspnetuserNavigation)
                                                                                 .Include(d => d.PaciIdPacienteNavigation).ThenInclude(a => a.ProfIdProfissionalResponsavelNavigation).ThenInclude(p => p.IdPessoaNavigation).ThenInclude(p => p.AspuIdAspnetuserNavigation);
            if (User.IsInRole("Administradores"))
            {

            }
            else if (User.IsInRole("Pacientes"))
            {
                doencaPacientes = doencaPacientes.Where(a => a.PaciIdPacienteNavigation.IdPessoaNavigation.AspuIdAspnetuserNavigation.Email == User.Identity.Name);

            }
            else if (User.IsInRole("Profissionais"))
            {
                doencaPacientes = doencaPacientes.Where(a => a.PaciIdPacienteNavigation.ProfIdProfissionalResponsavelNavigation.IdPessoaNavigation.AspuIdAspnetuserNavigation.Email == User.Identity.Name);
            }

            if (paciIdPaciente != 0)
            { 
                doencaPacientes = doencaPacientes.Where(s => s.PaciIdPaciente == paciIdPaciente);
            }

            return View(await doencaPacientes.ToListAsync());
        }

        // GET: DoencaPacientes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var doencaPaciente = await _context.DoencaPacientes.Include(d => d.DoenIdDoencaNavigation)
                                                               .Include(d => d.PaciIdPacienteNavigation)
                                                               .ThenInclude(p => p.IdPessoaNavigation)
                                                                    .ThenInclude(p => p.AspuIdAspnetuserNavigation)
                                                               .FirstOrDefaultAsync(m => m.Id == id);

            if (doencaPaciente == null)
            {
                return NotFound();
            }

            return View(doencaPaciente);
        }
        private List<Paciente> ListarPacientes()
        {
            IQueryable<Paciente> listaPacientes = _context.Pacientes.Include(p => p.IdPessoaNavigation).ThenInclude(p => p.AspuIdAspnetuserNavigation);

            if (User.IsInRole("Administradores"))
            {

            }
            else if (User.IsInRole("Pacientes"))
            {
                listaPacientes = listaPacientes.Where(p => p.IdPessoaNavigation.AspuIdAspnetuserNavigation.Email == User.Identity.Name);
            }
            else if (User.IsInRole("Profissionais"))
            {
                listaPacientes = listaPacientes.Where(p => p.ProfIdProfissionalResponsavelNavigation.IdPessoaNavigation.AspuIdAspnetuserNavigation.Email == User.Identity.Name);
            }

            return listaPacientes.ToList();
        }

        // GET: DoencaPacientes/Create
        public IActionResult Create()
        {
            ViewData["PaciIdPaciente"] = new SelectList(ListarPacientes(), "IdPessoa", "NomePaciente");

            ViewData["DoenIdDoenca"] = new SelectList(_context.Doencas, "Id", "DoenCodDoenca");

            //ViewData["PaciIdPaciente"] = new SelectList(_context.Pacientes, "IdPessoa", "PaciStAtivo");

            return View();
        }
       

        // POST: DoencaPacientes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,DoenIdDoenca,PaciIdPaciente")] DoencaPaciente doencaPaciente)
        {
            if (ModelState.IsValid)
            {
                _context.Add(doencaPaciente);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["PaciIdPaciente"] = new SelectList(ListarPacientes(), "IdPessoa", "NomePaciente", doencaPaciente.PaciIdPaciente);

            ViewData["DoenIdDoenca"] = new SelectList(_context.Doencas, "Id", "DoenCodDoenca", doencaPaciente.DoenIdDoenca);

            //ViewData["PaciIdPaciente"] = new SelectList(_context.Pacientes, "IdPessoa", "PaciStAtivo", doencaPaciente.PaciIdPaciente);

            return View(doencaPaciente);
        }

        // GET: DoencaPacientes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var doencaPaciente = await _context.DoencaPacientes.FindAsync(id);
            if (doencaPaciente == null)
            {
                return NotFound();
            }

            doencaPaciente = _context.DoencaPacientes.Include(d => d.DoenIdDoencaNavigation)
                                                     .Include(d =>d.PaciIdPacienteNavigation).ThenInclude(p => p.IdPessoaNavigation).ThenInclude(p => p.AspuIdAspnetuserNavigation).FirstOrDefaultAsync(m => m.Id == id).Result;
            //ViewData["PaciIdPaciente"] = new SelectList(ListarPacientes(), "IdPessoa", "NomePaciente", doencaPaciente.PaciIdPaciente);

            //ViewData["DoenIdDoenca"] = new SelectList(_context.Doencas, "Id", "DoenCodDoenca", doencaPaciente.DoenIdDoenca);
            //ViewData["PaciIdPaciente"] = new SelectList(_context.Pacientes, "IdPessoa", "PaciStAtivo", doencaPaciente.PaciIdPaciente);
            return View(doencaPaciente);
        }

        // POST: DoencaPacientes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,DoenIdDoenca,PaciIdPaciente")] DoencaPaciente doencaPaciente)
        {
            if (id != doencaPaciente.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(doencaPaciente);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DoencaPacienteExists(doencaPaciente.Id))
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

            //ViewData["PaciIdPaciente"] = new SelectList(ListarPacientes(), "IdPessoa", "NomePaciente", doencaPaciente.PaciIdPaciente);

            //ViewData["DoenIdDoenca"] = new SelectList(_context.Doencas, "Id", "DoenCodDoenca", doencaPaciente.DoenIdDoenca);
            //ViewData["PaciIdPaciente"] = new SelectList(_context.Pacientes, "IdPessoa", "PaciStAtivo", doencaPaciente.PaciIdPaciente);
            doencaPaciente = _context.DoencaPacientes.Include(d => d.PaciIdPacienteNavigation).ThenInclude(p => p.IdPessoaNavigation).ThenInclude(p => p.AspuIdAspnetuserNavigation).FirstOrDefaultAsync(m => m.Id == id).Result;

            return View(doencaPaciente);
        }

        // GET: DoencaPacientes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var doencaPaciente = await _context.DoencaPacientes.Include(d => d.DoenIdDoencaNavigation)
                                                               .Include(a => a.PaciIdPacienteNavigation).ThenInclude(p => p.IdPessoaNavigation).ThenInclude(p => p.AspuIdAspnetuserNavigation)
                                                               .FirstOrDefaultAsync(m => m.Id == id);
            if (doencaPaciente == null)
            {
                return NotFound();
            }

            return View(doencaPaciente);
        }

        // POST: DoencaPacientes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var doencaPaciente = await _context.DoencaPacientes.FindAsync(id);
            _context.DoencaPacientes.Remove(doencaPaciente);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private bool DoencaPacienteExists(int id)
        {
            return _context.DoencaPacientes.Any(e => e.Id == id);
        }
    }
}
