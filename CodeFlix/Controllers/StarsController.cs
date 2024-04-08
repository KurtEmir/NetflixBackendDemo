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
    public class StarsController : ControllerBase
    {
        private readonly CodeFlixContext _context;

        public StarsController(CodeFlixContext context)
        {
            _context = context;
        }

        // GET: api/Stars
        [HttpGet]
        [Authorize]
        public ActionResult<List<Star>> GetStars()
        {
            return _context.Stars.ToList();
        }

        // GET: api/Stars/5
        [HttpGet("{id}")]
        [Authorize]
        public ActionResult<Star> GetStar(int id)
        {
            var star = _context.Stars.Where(s => s.Id == id).Include(s => s.MediaStars).AsNoTracking().FirstOrDefault();
            if (star==null)
            {
                return NotFound();
            }
            return star;
        }

        // PUT: api/Stars/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Administrator, ContentAdmin")]
        public ActionResult PutStar(int id, Star star)
        {
            if (id != star.Id)
            {
                return BadRequest();
            }

            _context.Entry(star).State = EntityState.Modified;
            _context.SaveChangesAsync();
            
            return Ok();
        }

        // POST: api/Stars
        [HttpPost]
        [Authorize(Roles = "Administrator, ContentAdmin")]
        public ActionResult<Star> PostStar(Star star)
        {         
            _context.Stars.Add(star);
            _context.SaveChanges();
            
            return Ok("Star added!");
        }

        // DELETE: api/Stars/5
        [HttpDelete("{id}")]
        public ActionResult DeleteStar(int id)
        {
            Star? star = _context.Stars.FindAsync(id).Result;
            if (star == null)
            {
                return NotFound("Star not found.");
            }
            
            var mediaStars = _context.MediaStars.Where(ms => ms.StarId == id);
            foreach (var mediaStar in mediaStars)
            {
                var media = _context.Medias.FindAsync(mediaStar.MediaId).Result;
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
