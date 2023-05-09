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
    public class PacientesController : Controller
    {
        private readonly RIATContext _context;

        public PacientesController(RIATContext context)
        {
            _context = context;
        }

        // GET: Pacientes
        public async Task<IActionResult> Index()
        {
            IQueryable<Paciente> listaPacientes = _context.Pacientes
                                      .Include(p => p.IdPessoaNavigation).ThenInclude(p => p.AspuIdAspnetuserNavigation)
                                      .Include(p => p.ProfIdProfissionalResponsavelNavigation).ThenInclude(p => p.IdPessoaNavigation).ThenInclude(p => p.AspuIdAspnetuserNavigation)
                                      .Include(p => p.PaciTpContratacaoNavigation)
                                      .Include(p => p.GrinIdGrauInstrucaoNavigation);

            if (User.IsInRole("Administradores"))
            {

            }
            else if (User.IsInRole("Pacientes"))
            {
                listaPacientes = listaPacientes.Where(a => a.IdPessoaNavigation.AspuIdAspnetuserNavigation.Email == User.Identity.Name);
            }
            else if (User.IsInRole("Profissionais"))
            {
                listaPacientes = listaPacientes.Where(a => a.ProfIdProfissionalResponsavelNavigation.IdPessoaNavigation.AspuIdAspnetuserNavigation.Email == User.Identity.Name);
            }

            return View(await listaPacientes.OrderBy(p => p.IdPessoaNavigation.AspuIdAspnetuserNavigation.FirstName).ThenBy(p => p.IdPessoaNavigation.AspuIdAspnetuserNavigation.LastName).ToListAsync());
        }

        // GET: Pacientes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var paciente = await _context.Pacientes
                                         .Include(p => p.IdPessoaNavigation).ThenInclude(p => p.AspuIdAspnetuserNavigation)
                                         .Include(p => p.ProfIdProfissionalResponsavelNavigation).ThenInclude(p => p.IdPessoaNavigation).ThenInclude(p => p.AspuIdAspnetuserNavigation)
                                         .Include(p => p.PaciTpContratacaoNavigation)
                                         .Include(p => p.GrinIdGrauInstrucaoNavigation)
                                         .FirstOrDefaultAsync(m => m.IdPessoa == id);

            if (paciente == null)
            {
                return NotFound();
            }

            return View(paciente);
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

            return listaPacientes.OrderBy(p => p.IdPessoaNavigation.AspuIdAspnetuserNavigation.FirstName).ThenBy(p => p.IdPessoaNavigation.AspuIdAspnetuserNavigation.LastName).ToList();
        }

        private List<Profissional> ListarProfissionais()
        {
            IQueryable<Profissional> listaProfissionais = _context.Profissionals.Include(p => p.IdPessoaNavigation).ThenInclude(p => p.AspuIdAspnetuserNavigation);


            if (User.IsInRole("Administradores"))
            {

            }
            else if (User.IsInRole("Pacientes"))
            {
                listaProfissionais = listaProfissionais.Where(p => p.Pacientes.Any(p => p.IdPessoaNavigation.AspuIdAspnetuserNavigation.Email == User.Identity.Name));
            }
            else if (User.IsInRole("Profissionais"))
            {
                listaProfissionais = listaProfissionais.Where(p => p.IdPessoaNavigation.AspuIdAspnetuserNavigation.Email == User.Identity.Name);
            }

            var profissionais = listaProfissionais.OrderBy(p => p.IdPessoaNavigation.AspuIdAspnetuserNavigation.FirstName).ThenBy(p => p.IdPessoaNavigation.AspuIdAspnetuserNavigation.LastName).ToList();
            
            return profissionais;
        }

        // GET: Pacientes/Create
        [Authorize(Roles = "Administradores")]
        public IActionResult Create()
        {
            ViewData["IdPessoa"] = new SelectList(ListarPacientes(), "IdPessoa", "NomePaciente");
            ViewData["ProfIdProfissionalResponsavel"] = new SelectList(ListarProfissionais(), "IdPessoa", "NomeProfissional");
            ViewData["GrinIdGrauInstrucao"] = new SelectList(_context.GrauInstrucaos, "Id", "GrinNmGrauInstrucao");
            ViewData["PaciTpContratacao"] = new SelectList(_context.TipoContratacaos, "Id", "TiprNmTipoContratacao");

            //([PACI_ST_ESTADO_CIVIL] = 'Outro' OR[PACI_ST_ESTADO_CIVIL] = 'União Estável' OR[PACI_ST_ESTADO_CIVIL] = 'Divorciado' OR[PACI_ST_ESTADO_CIVIL] = 'Separado' OR[PACI_ST_ESTADO_CIVIL] = 'Viúvo' OR[PACI_ST_ESTADO_CIVIL] = 'Casado' OR[PACI_ST_ESTADO_CIVIL] = 'Solteiro')

            return View();
        }

        // POST: Pacientes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administradores")]
        public async Task<IActionResult> Create([Bind("IdPessoa,PaciCdPassaporte,PaciCdRegistroGeral,PaciStEstadoCivil,PaciNmMae,PaciNmPai,PaciNmProfissao,PaciTxEncaminhadoPor,PaciStAtivo,PaciDtAtivacao,PaciDtDesativacao,GrinIdGrauInstrucao,PaciTpContratacao,ProfIdProfissional")] Paciente paciente)
        {
            if (string.IsNullOrEmpty(paciente.PaciStAtivo))
            {
                paciente.PaciStAtivo = "A";
            }

            if (paciente.PaciDtAtivacao == DateTime.MinValue)
            {
                paciente.PaciDtAtivacao = DateTime.Now;
            }

            if (ModelState.IsValid)
            {
                _context.Add(paciente);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            ViewData["IdPessoa"] = new SelectList(ListarPacientes(), "IdPessoa", "NomePaciente", paciente.IdPessoa);
            ViewData["ProfIdProfissionalResponsavel"] = new SelectList(ListarProfissionais(), "IdPessoa", "NomeProfissional", paciente.ProfIdProfissionalResponsavelNavigation);

            ViewData["PaciTpContratacao"] = new SelectList(_context.TipoContratacaos, "Id", "TiprNmTipoContratacao", paciente.PaciTpContratacao);
            ViewData["GrinIdGrauInstrucao"] = new SelectList(_context.GrauInstrucaos, "Id", "GrinNmGrauInstrucao", paciente.GrinIdGrauInstrucao);
            return View(paciente);
        }

        // GET: Pacientes/Edit/5
        [Authorize(Roles = "Administradores, Pacientes")]
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

            paciente = _context.Pacientes.Include(p => p.IdPessoaNavigation).ThenInclude(p => p.AspuIdAspnetuserNavigation).FirstOrDefaultAsync(m => m.IdPessoa == id).Result;

            ViewData["ProfIdProfissionalResponsavel"] = new SelectList(ListarProfissionais(), "IdPessoa", "NomeProfissional", paciente.ProfIdProfissionalResponsavel);
            ViewData["GrinIdGrauInstrucao"] = new SelectList(_context.GrauInstrucaos, "Id", "GrinNmGrauInstrucao", paciente.GrinIdGrauInstrucao);
            ViewData["PaciTpContratacao"] = new SelectList(_context.TipoContratacaos, "Id", "TiprNmTipoContratacao", paciente.PaciTpContratacao);

            return View(paciente);
        }

        // POST: Pacientes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administradores, Pacientes")]
        public async Task<IActionResult> Edit(int idPessoa, [Bind("IdPessoa,PaciCdPassaporte,PaciCdRegistroGeral,PaciStEstadoCivil,PaciNmMae,PaciNmPai,PaciNmProfissao,PaciTxEncaminhadoPor,PaciStAtivo,PaciDtAtivacao,PaciDtDesativacao,GrinIdGrauInstrucao,PaciTpContratacao,ProfIdProfissional")] Paciente paciente)
        {
            if (idPessoa != paciente.IdPessoa)
            {
                return NotFound();
            }

            if (string.IsNullOrEmpty(paciente.PaciStAtivo))
            {
                paciente.PaciStAtivo = "A";
            }

            if (paciente.PaciDtAtivacao == DateTime.MinValue)
            {
                paciente.PaciDtAtivacao = DateTime.Now;
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

            paciente = _context.Pacientes.Include(p => p.IdPessoaNavigation).ThenInclude(p => p.AspuIdAspnetuserNavigation).FirstOrDefaultAsync(m => m.IdPessoa == idPessoa).Result;

            ViewData["ProfIdProfissionalResponsavel"] = new SelectList(ListarProfissionais(), "IdPessoa", "NomeProfissional", paciente.ProfIdProfissionalResponsavel);
            ViewData["GrinIdGrauInstrucao"] = new SelectList(_context.GrauInstrucaos, "Id", "GrinNmGrauInstrucao", paciente.GrinIdGrauInstrucao);
            ViewData["PaciTpContratacao"] = new SelectList(_context.TipoContratacaos, "Id", "TiprNmTipoContratacao", paciente.PaciTpContratacao);

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
                                         .Include(p => p.IdPessoaNavigation).ThenInclude(p => p.AspuIdAspnetuserNavigation)
                                         .Include(p => p.ProfIdProfissionalResponsavelNavigation).ThenInclude(p => p.IdPessoaNavigation).ThenInclude(p => p.AspuIdAspnetuserNavigation)
                                         .Include(p => p.PaciTpContratacaoNavigation)
                                         .Include(p => p.GrinIdGrauInstrucaoNavigation)
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
