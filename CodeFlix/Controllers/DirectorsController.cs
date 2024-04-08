using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CodeFlix.Data;
using CodeFlix.Models;
using Microsoft.AspNetCore.Authorization;

namespace CodeFlix.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DirectorsController : ControllerBase
    {
        private readonly CodeFlixContext _context;

        public DirectorsController(CodeFlixContext context)
        {
            _context = context;
        }

        // GET: api/Directors
        [HttpGet]
        [Authorize]
        public ActionResult<List<Director>> GetDirectors()
        {        
            return _context.Directors.ToList();
        }

        // GET: api/Directors/5
        [HttpGet("{id}")]
        [Authorize]
        public ActionResult<Director> GetDirector(int id)
        {
            var director=_context.Directors.Where(d=>d.Id==id).Include(d=>d.MediaDirectors).AsNoTracking().FirstOrDefault();
            //var director = _context.MediaDirectors.Where(md=>md.DirectorId==id).Include(md=>md.DirectorId).Include(md=>md.Media).AsNoTracking().ToList();
            if (director == null)
            {
                return NotFound();
            }
            return director;
        }

        // PUT: api/Directors/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Administrator, ContentAdmin")]
        public async Task<IActionResult> PutDirector(int id, Director director)
        {
            if (id != director.Id)
            {
                return BadRequest();
            }

            _context.Entry(director).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return Ok("Director updated!");
        }

        // POST: api/Directors
        [HttpPost]
        [Authorize(Roles = "Administrator, ContentAdmin")]
        public ActionResult PostDirector(Director director)
        {

            _context.Directors.Add(director);
            _context.SaveChanges();

            return Ok("Director added!");
        }

        // DELETE: api/Directors/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrator, ContentAdmin")]
        public ActionResult DeleteDirector(int id)
        {
            
            Director? director =  _context.Directors.FindAsync(id).Result;
            if (director==null)
            {
                return BadRequest("Director not found");
            }

            var mediaDirectors = _context.MediaDirectors.Where(m => m.DirectorId == id);
            foreach (var mediaDirector in mediaDirectors)
            {
                var media = _context.Medias.FindAsync(mediaDirector.MediaId).Result;
                if (media != null)
                {
                    media.Passive = true;
                    _context.Medias.Update(media);
                }
            }
            _context.SaveChanges();
            return Ok();
        }
    }
}
