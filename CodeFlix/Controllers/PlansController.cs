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
    public class PlansController : ControllerBase
    {
        private readonly CodeFlixContext _context;

        public PlansController(CodeFlixContext context)
        {
            _context = context;
        }

        // GET: api/Plans
        [HttpGet]
        public ActionResult<List<Plan>> GetPlans()
        {
            return _context.Plans.ToList();
        }

        // GET: api/Plans/5
        [HttpGet("{id}")]
        public ActionResult<Plan> GetPlan(short id)
        {
            var plan = _context.Plans.Find(id);

            if (plan == null)
            {
                return NotFound();
            }
            return plan;
        }

        // PUT: api/Plans/5
        [HttpPut("{id}")]
        public ActionResult PutPlan(short id, Plan plan)
        {
            if (id != plan.Id)
            {
                return BadRequest();
            }
            _context.Plans.Update(plan);
            _context.SaveChanges();
            return NoContent();
        }

        // POST: api/Plans
        [HttpPost]
        public ActionResult<Plan> PostPlan(Plan plan)
        {
            _context.Plans.Add(plan);
            _context.SaveChanges();
            return Ok();
        }

        // DELETE: api/Plans/5
        [HttpDelete("{id}")]
        public ActionResult DeletePlan(short id)
        {
            var plan = _context.Plans.Find(id);
            if (plan == null)
            {
                return NotFound();
            }
            _context.Plans.Remove(plan);
            _context.SaveChanges();
            return NoContent();
        }
    }
}
