using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using APIMangas.Models;

namespace APIMangas.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MangasController : ControllerBase
    {
        private readonly MangaDBContext _context;

        private readonly ILogger _logger;//Le "Ilogger" est implementé et injecté pour pouvoir journaliser par la suite.
                                         //Ca va nous servir de traçabilité sur les plateformes serveurs et ça nous permet de gérer les exceptions.
                                         //Comme par exemple pour gérer un panier utilisateur qui ne fonctionne pas par exemple.
                                         //Et donc pour savoir où se trouve l'exception.

        public MangasController(MangaDBContext context, ILogger<MangasController> logger)
        {
            _logger= logger;
            _context = context;
            
        }

        // GET: api/Mangas
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Manga>>> GetMangas(int debut, int nbrResult)
        {
            _logger.LogInformation("Action Method GetMangas called......v2");
            return await _context.Mangas.ToListAsync();
        }

        // GET: api/Mangas/GetMangasPaged (Route)
        [HttpGet("GetMangasPaged")]
        public async Task<ActionResult<IEnumerable<Manga>>> GetMangasPaged(int debut, int nbrResult) //Pour éviter de faire bugger le serveur.
                                                                                                     //Il est très important de faire de la pagination pour éviter d'afficher en une seule fois.
        {
            _logger.LogInformation("Action Method GetMangas called......v2");
            return await _context.Mangas.Skip(debut).Take(nbrResult).ToListAsync();
        }


        // GET: api/Mangas/5
        /// <summary>
        /// Renvoie un manga à partir de son identifiant
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Objet Manga</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<Manga>> GetManga(int id)
        {
            //Ajout d'un bloc try...catch
            try
            {
                var manga = await _context.Mangas.FindAsync(id);

                if (manga == null)
                {
                    return NotFound();
                }

                return manga;
            }
            catch (Exception ex)
            {
                var url = HttpContext.Request.Path;
                _logger.LogError(ex, $"URL : {url}");//On peut utiliser le LogError, LogCritical,... en fonction du niveau de gravité du journal.
                                                     //Si tu définis ton niveau à "LogInformation", ton journal va regarder le "LogInformation" et ce qu'il y a au dessus (niveau supérieur).
                                                     //Le Log level permet d'affiner pour ne pas gravir tous les niveaux du journal.
                return NotFound();
                
            }
        }

        // PUT: api/Mangas/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        /// <summary>
        /// Met à jour un manga à partir de son identifiant.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="manga"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> PutManga(int id, Manga manga)
        {
            if (id != manga.Id)
            {
                return BadRequest();
            }

            _context.Entry(manga).State = EntityState.Modified; //Mode deconnecté

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex) //Exception générique pour pouvoir tracer et recupérer.
                                 //Le DbUpdateConcurrency quant-à-lui permet de gérer les exceptions et tous les exceptions vont pouvoir entrer.
            {
                _logger.LogError(ex, ""); //"LogError" trace du fail alors que "LogInformation" trace de l'info dans la journalisation.
                                          //Lors de la gestion des exceptions notamment.
                if (!MangaExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Mangas
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Manga>> PostManga(Manga manga)
        {
            _context.Mangas.Add(manga);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetManga", new { id = manga.Id }, manga);
        }

        // DELETE: api/Mangas/5
        /// <summary>
        /// Supprime un manga à partir de son identifiant.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteManga(int id)
        {
            var manga = await _context.Mangas.FindAsync(id);
            if (manga == null)
            {
                return NotFound();
            }

            _context.Mangas.Remove(manga);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool MangaExists(int id)
        {
            return _context.Mangas.Any(e => e.Id == id);
        }
    }
}
