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
    public class SintomaAtendimentoesController : Controller
    {
        private readonly RIATContext _context;

        public SintomaAtendimentoesController(RIATContext context)
        {
            _context = context;
        }

        // GET: SintomaAtendimentoes
        public async Task<IActionResult> Index(int idAtendimento)
        {
            IQueryable<SintomaAtendimento> sintomaAtendimento = _context.SintomaAtendimentos.Include(a => a.IdAtendimentoNavigation.PaciIdPacienteNavigation).ThenInclude(p => p.IdPessoaNavigation).ThenInclude(p => p.AspuIdAspnetuserNavigation)
                                                          .Include(a => a.IdAtendimentoNavigation.ProfIdProfissionalNavigation).ThenInclude(p => p.IdPessoaNavigation).ThenInclude(p => p.AspuIdAspnetuserNavigation)
                                                          .Include(s => s.DopaIdDoencaPacienteNavigation).ThenInclude(s => s.DoenIdDoencaNavigation)
                                                          .Include(s => s.IdAtendimentoNavigation)
                                                          .Include(s => s.SintIdSintomaNavigation);

            if(idAtendimento != 0) 
            {
                sintomaAtendimento = sintomaAtendimento.Where(s => s.IdAtendimento.GetValueOrDefault() == idAtendimento);
            }

            //sintomaAtendimento = sintomaAtendimento.OrderBy(s => s.IdAtendimentoNavigation.PaciIdPacienteNavigation.NomePaciente)
            //                                       .ThenBy(s => s.IdAtendimentoNavigation.ProfIdProfissionalNavigation.NomeProfissional)
            //                                       .ThenByDescending(s => s.IdAtendimentoNavigation.AtenDtAgendamento)
            //                                       .ThenBy(s => s.DopaIdDoencaPacienteNavigation.DoenIdDoencaNavigation.DoenNmHipotese)
            //                                       .ThenBy(s => s.SintIdSintomaNavigation.SintNomeSintoma);

            return View(await sintomaAtendimento.ToListAsync());
        }

        // GET: SintomaAtendimentoes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sintomaAtendimento = await _context.SintomaAtendimentos
                .Include(a => a.IdAtendimentoNavigation.PaciIdPacienteNavigation).ThenInclude(p => p.IdPessoaNavigation).ThenInclude(p => p.AspuIdAspnetuserNavigation)
                .Include(a => a.IdAtendimentoNavigation.ProfIdProfissionalNavigation).ThenInclude(p => p.IdPessoaNavigation).ThenInclude(p => p.AspuIdAspnetuserNavigation)
                .Include(s => s.DopaIdDoencaPacienteNavigation).ThenInclude(s => s.DoenIdDoencaNavigation)
                .Include(s => s.IdAtendimentoNavigation)
                .Include(s => s.SintIdSintomaNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (sintomaAtendimento == null)
            {
                return NotFound();
            }

            return View(sintomaAtendimento);
        }

        // GET: SintomaAtendimentoes/Create
        [Authorize(Roles = "Administradores, Profissionais")]

        public IActionResult Create()
        {
            var listaPacientes = _context.Pacientes.Include(p => p.IdPessoaNavigation).ThenInclude(p => p.AspuIdAspnetuserNavigation)
                                              .Select(p => new {
                                                  p.IdPessoa,
                                                  p.NomePaciente
                                              }
                                                     )
                                              .ToList();


            var selectCodigoDoProduto = new List<SelectListItem>();
            selectCodigoDoProduto.Insert(0, new SelectListItem { Text = "--Selecione um Paciente--", Value = "" });
            selectCodigoDoProduto.AddRange(new SelectList(listaPacientes, "IdPessoa", "NomePaciente"));
            ViewData["PaciIdPaciente"] = selectCodigoDoProduto;

            //var listaProfissionais = _context.Profissionals.Include(p => p.IdPessoaNavigation).ThenInclude(p => p.AspuIdAspnetuserNavigation)
            //                               .Select(p => new {
            //                                   p.IdPessoa,
            //                                   p.NomeProfissional
            //                               }
            //                                      );

            //ViewData["ProfIdProfissional"] = new SelectList(listaProfissionais.ToList(), "IdPessoa", "NomeProfissional");

            //ViewData["IdAtendimento"] = new SelectList(_context.Atendimentos, "Id", "AtenDtAgendamento");
            var selectAtendimentos = new List<SelectListItem>();
            selectAtendimentos.Insert(0, new SelectListItem { Text = "--Selecione um Atendimento--", Value = "" });
            ViewData["IdAtendimento"] = selectAtendimentos;

            //var listaDoencaPacientes = _context.DoencaPacientes.Include(p => p.DoenIdDoencaNavigation)
            //                              .Select(p => new {
            //                                                  p.DoenIdDoenca,
            //                                                  p.DoenIdDoencaNavigation.DoenNmHipotese
            //                                               }
            //                                     );

            //ViewData["DopaIdDoencaPaciente"] = new SelectList(listaDoencaPacientes, "DoenIdDoenca", "DoenNmHipotese");
            var selectDoencas = new List<SelectListItem>();
            selectDoencas.Insert(0, new SelectListItem { Text = "--Selecione uma Doença--", Value = "" });
            ViewData["DopaIdDoencaPaciente"] = selectDoencas;

            //var listaSintomas = _context.SintomaAtendimentos.Include(p => p.SintIdSintomaNavigation)
            //                             .Select(p => new {
            //                                                 p.SintIdSintoma,
            //                                                 p.SintIdSintomaNavigation.SintNomeSintoma
            //                                             }
            //                                    );

            //ViewData["SintIdSintoma"] = new SelectList(listaSintomas, "SintIdSintoma", "SintNomeSintoma");
            var selectSintomas = new List<SelectListItem>();
            selectSintomas.Insert(0, new SelectListItem { Text = "--Selecione um Sintoma--", Value = "" });
            ViewData["SintIdSintoma"] = selectSintomas;

            return View();
        }

        // POST: SintomaAtendimentoes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administradores, Profissionais")]
        public async Task<IActionResult> Create([Bind("Id,DopaIdDoencaPaciente,SintIdSintoma,SidhCdPeso,IdAtendimento")] SintomaAtendimento sintomaAtendimento)
        {
            if (ModelState.IsValid)
            {
                _context.Add(sintomaAtendimento);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            var listaPacientes = _context.Pacientes.Include(p => p.IdPessoaNavigation).ThenInclude(p => p.AspuIdAspnetuserNavigation)
                                               .Select(p => new {
                                                   p.IdPessoa,
                                                   p.NomePaciente
                                               }
                                                      );

            ViewData["PaciIdPaciente"] = new SelectList(listaPacientes.ToList(), "IdPessoa", "NomePaciente");

            //var listaProfissionais = _context.Profissionals.Include(p => p.IdPessoaNavigation).ThenInclude(p => p.AspuIdAspnetuserNavigation)
            //                               .Select(p => new {
            //                                   p.IdPessoa,
            //                                   p.NomeProfissional
            //                               }
            //                                      );

            //ViewData["ProfIdProfissional"] = new SelectList(listaProfissionais.ToList(), "IdPessoa", "NomeProfissional");

            //ViewData["IdAtendimento"] = new SelectList(_context.Atendimentos, "Id", "AtenDtAgendamento");
            ViewData["IdAtendimento"] = new SelectListItem { Text = "--Selecione um Atendimento--", Value = "" };

            //var listaDoencaPacientes = _context.DoencaPacientes.Include(p => p.DoenIdDoencaNavigation)
            //                              .Select(p => new {
            //                                  p.DoenIdDoenca,
            //                                  p.DoenIdDoencaNavigation.DoenNmHipotese
            //                              }
            //                                     );

            //ViewData["DopaIdDoencaPaciente"] = new SelectList(listaDoencaPacientes, "DoenIdDoenca", "DoenNmHipotese");
            ViewData["DopaIdDoencaPaciente"] = new SelectListItem { Text = "--Selecione uma Doença--", Value = "" };

            //var listaSintomas = _context.SintomaAtendimentos.Include(p => p.SintIdSintomaNavigation)
            //                             .Select(p => new {
            //                                 p.SintIdSintoma,
            //                                 p.SintIdSintomaNavigation.SintNomeSintoma
            //                             }
            //                                    );

            //ViewData["SintIdSintoma"] = new SelectList(listaSintomas, "SintIdSintoma", "SintNomeSintoma");
            ViewData["SintIdSintoma"] = new SelectListItem { Text = "--Selecione um Sintoma--", Value = "" };

            return View(sintomaAtendimento);
        }

        // GET: SintomaAtendimentoes/Edit/5
        [Authorize(Roles = "Administradores, Profissionais")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sintomaAtendimento = await _context.SintomaAtendimentos.Include(a => a.IdAtendimentoNavigation.PaciIdPacienteNavigation).ThenInclude(p => p.IdPessoaNavigation).ThenInclude(p => p.AspuIdAspnetuserNavigation)
                .Include(a => a.IdAtendimentoNavigation.ProfIdProfissionalNavigation).ThenInclude(p => p.IdPessoaNavigation).ThenInclude(p => p.AspuIdAspnetuserNavigation)
                .Include(s => s.DopaIdDoencaPacienteNavigation).ThenInclude(s => s.DoenIdDoencaNavigation)
                .Include(s => s.IdAtendimentoNavigation)
                .Include(s => s.SintIdSintomaNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (sintomaAtendimento == null)
            {
                return NotFound();
            }

            return View(sintomaAtendimento);
        }

        // GET: SintomaAtendimentoes/Edit/5
        [Authorize(Roles = "Administradores, Profissionais")]
        public async Task<IActionResult> EditInline(int idAtendimento)
        {
            IQueryable<SintomaAtendimento> sintomaAtendimento = _context.SintomaAtendimentos.Include(a => a.IdAtendimentoNavigation.PaciIdPacienteNavigation).ThenInclude(p => p.IdPessoaNavigation).ThenInclude(p => p.AspuIdAspnetuserNavigation)
                .Include(a => a.IdAtendimentoNavigation.ProfIdProfissionalNavigation).ThenInclude(p => p.IdPessoaNavigation).ThenInclude(p => p.AspuIdAspnetuserNavigation)
                .Include(s => s.DopaIdDoencaPacienteNavigation).ThenInclude(s => s.DoenIdDoencaNavigation)
                .Include(s => s.IdAtendimentoNavigation)
                .Include(s => s.SintIdSintomaNavigation);

            if (idAtendimento != 0)
            {
                sintomaAtendimento = sintomaAtendimento.Where(s => s.IdAtendimento.GetValueOrDefault() == idAtendimento);
            }

            if (sintomaAtendimento == null)
            {
                return NotFound();
            }

            sintomaAtendimento.OrderBy(s => s.IdAtendimentoNavigation.PaciIdPacienteNavigation.NomePaciente)
                              .ThenBy(s => s.IdAtendimentoNavigation.ProfIdProfissionalNavigation.NomeProfissional)
                              .ThenByDescending(s => s.IdAtendimentoNavigation.AtenDtAgendamento)
                              .ThenBy(s => s.DopaIdDoencaPacienteNavigation.DoenIdDoencaNavigation.DoenNmHipotese)
                              .ThenBy(s => s.SintIdSintomaNavigation.SintNomeSintoma);

            return View(await sintomaAtendimento.ToListAsync());
        }

        [HttpGet]
        public JsonResult GetAtendimentos(int idPaciente)
        {
            var selectAtendimentos = new List<SelectListItem>();

            var atendimentos = _context.Atendimentos.Where(s => s.PaciIdPaciente == idPaciente)
                                                    .OrderBy(c => c.AtenDtAgendamento)
                                                    .ToList();

            if (atendimentos != null)
            {
                selectAtendimentos.Insert(0, new SelectListItem { Text = "--Selecione um Atendimento--", Value = "" });
                selectAtendimentos.AddRange(new SelectList(atendimentos, "Id", "AtenDtAgendamento"));
            }

            return Json(new SelectList(selectAtendimentos, "Value", "Text"));

        }

        [HttpGet]
        public JsonResult GetDoencas(int idPaciente)
        {
            var selectDoencas = new List<SelectListItem>();

            var doencas = _context.DoencaPacientes.Include(d => d.DoenIdDoencaNavigation)
                                                   .Where(s => s.PaciIdPaciente == idPaciente)
                                                   .OrderBy(c => c.DoenIdDoencaNavigation.DoenNmHipotese)
                                                   .Select(p => new
                                                   {
                                                       p.DoenIdDoenca,
                                                       p.DoenIdDoencaNavigation.DoenNmHipotese
                                                   })
                                                   .ToList();

            if (doencas != null)
            {
                selectDoencas.Insert(0, new SelectListItem { Text = "--Selecione uma Doença--", Value = "" });
                selectDoencas.AddRange(new SelectList(doencas, "DoenIdDoenca", "DoenNmHipotese"));
            }

            return Json(new SelectList(selectDoencas, "Value", "Text"));

        }

        [HttpGet]
        public JsonResult GetSintomas(int idDoenca)
        {
            var selectSintomas = new List<SelectListItem>();

            var doencas = _context.Sintomas.Where(s => s.DoenIdDoenca == idDoenca)
                                                   .OrderBy(c => c.SintNomeSintoma)
                                                   .Select(p => new
                                                   {
                                                       p.Id,
                                                       p.SintNomeSintoma
                                                   })
                                                   .ToList();

            if (doencas != null)
            {
                selectSintomas.Insert(0, new SelectListItem { Text = "--Selecione um Sintoma--", Value = "" });
                selectSintomas.AddRange(new SelectList(doencas, "Id", "SintNomeSintoma"));
            }

            return Json(new SelectList(selectSintomas, "Value", "Text"));

        }

        // POST: SintomaAtendimentoes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administradores, Profissionais")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,DopaIdDoencaPaciente,SintIdSintoma,SidhCdPeso,IdAtendimento")] SintomaAtendimento sintomaAtendimento)
        {
            if (id != sintomaAtendimento.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(sintomaAtendimento);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SintomaAtendimentoExists(sintomaAtendimento.Id))
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
            ViewData["DopaIdDoencaPaciente"] = new SelectList(_context.DoencaPacientes, "Id", "Id", sintomaAtendimento.DopaIdDoencaPaciente);
            ViewData["IdAtendimento"] = new SelectList(_context.Atendimentos, "Id", "AtenStAgendamento", sintomaAtendimento.IdAtendimento);
            ViewData["SintIdSintoma"] = new SelectList(_context.Sintomas, "Id", "SintCodSintoma", sintomaAtendimento.SintIdSintoma);
            return View(sintomaAtendimento);
        }

        // GET: SintomaAtendimentoes/Delete/5
        [Authorize(Roles = "Administradores, Profissionais")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sintomaAtendimento = await _context.SintomaAtendimentos
                .Include(a => a.IdAtendimentoNavigation.PaciIdPacienteNavigation).ThenInclude(p => p.IdPessoaNavigation).ThenInclude(p => p.AspuIdAspnetuserNavigation)
                .Include(a => a.IdAtendimentoNavigation.ProfIdProfissionalNavigation).ThenInclude(p => p.IdPessoaNavigation).ThenInclude(p => p.AspuIdAspnetuserNavigation)
                .Include(s => s.DopaIdDoencaPacienteNavigation).ThenInclude(s => s.DoenIdDoencaNavigation)
                .Include(s => s.IdAtendimentoNavigation)
                .Include(s => s.SintIdSintomaNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (sintomaAtendimento == null)
            {
                return NotFound();
            }

            return View(sintomaAtendimento);
        }

        // POST: SintomaAtendimentoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administradores, Profissionais")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var sintomaAtendimento = await _context.SintomaAtendimentos.FindAsync(id);
            _context.SintomaAtendimentos.Remove(sintomaAtendimento);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SintomaAtendimentoExists(int id)
        {
            return _context.SintomaAtendimentos.Any(e => e.Id == id);
        }
    }
}
