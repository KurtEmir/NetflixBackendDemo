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
    public class CategoriesController : ControllerBase
    {
        private readonly CodeFlixContext _context;

        public CategoriesController(CodeFlixContext context)
        {
            _context = context;
        }

        // GET: api/Categories
        [HttpGet]
        public ActionResult<List<Category>> GetCategory()
        {
            return _context.Categories.AsNoTracking().ToList();
        }

        // GET: api/Categories/5
        [HttpGet("{id}")]
        [Authorize]
        public ActionResult<Category> GetCategory(short id)
        {
          
            Category? category =  _context.Categories.Find(id);

            if (category == null)
            {
                return NotFound();
            }

            return category;
        }

        // PUT: api/Categories/5
        [HttpPut]
        [Authorize(Roles ="ContentAdmin")]
        public void PutCategory(Category category) //return NoContent vardı. o zaman hiç birşey döndürmeyecektir. bu yüzden void yaptık.
        {
            _context.Categories.Update(category);

            try
            {
                _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
            }

        }

        // POST: api/Categories
        [HttpPost]
        [Authorize(Roles = "ContentAdmin")]
        public ActionResult PostCategory(Category category)
        {
          
            _context.Categories.Add(category);
            _context.SaveChanges();

            return Ok();
        }
    }
}
