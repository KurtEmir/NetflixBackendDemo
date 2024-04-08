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
    public class MediasController : ControllerBase
    {
        private readonly CodeFlixContext _context;

        public MediasController(CodeFlixContext context)
        {
            _context = context;
        }

        // GET: api/Medias
        [HttpGet]
        [Authorize]
        public ActionResult<List<Media>> GetMedias()
        {
            return _context.Medias.ToList();
        }
        
        // GET: api/Medias/5
        [HttpGet("{id}")]
        [Authorize]
        public ActionResult<Media> GetMedia(int id)
        {
            var media = _context.Medias.Where(m => m.Id == id).FirstOrDefault();

            if (media == null)
            {
                return NotFound();
            }
            return media;
        }

        [HttpGet("{categoryId}")]
        [Authorize]
        public ActionResult<List<MediaCategory>> GetMediaCategory(short categoryId)
        {
            var media = _context.MediaCategories.Where(mc => mc.CategoryId == categoryId).Include(mc => mc.Media).ToList();

            return media;
        }

        // PUT: api/Medias/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Administrator, ContentAdmin")]
        public ActionResult PutMedia(int id, Media media)
        {
            Media? mediaToUpdate=null;

            if (mediaToUpdate==null)
            {
                return NotFound();
            }
            if (id != media.Id)
            {
                return BadRequest();
            }

            mediaToUpdate.Name = media.Name;
            mediaToUpdate.Description = media.Description;

            _context.Medias.Update(media);
            _context.SaveChanges();
            
            return Ok("Media updated!");
        }

        // POST: api/Medias
        [HttpPost]
        [Authorize(Roles = "Administrator, ContentAdmin")]
        public ActionResult<Media> PostMedia(Media media)
        {
            _context.Medias.Add(media);
            _context.SaveChanges();

            return Ok();
        }

        // DELETE: api/Medias/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrator, ContentAdmin")]
        public ActionResult DeleteMedia(int id)
        {
            var media =  _context.Medias.Find(id);
            if (media == null)
            {
                return NotFound();
            }
            media.Passive = true;
            _context.Medias.Update(media);
            _context.SaveChanges();

            return Ok("Media deleted.");
        }
    }
}
