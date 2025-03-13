using ITB2203Application.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ITB2203Application.Controllers;
[ApiController]
[Route("api/[controller]")]
public class SessionsController : Controller
{
    private readonly DataContext _context;

    public SessionsController(DataContext context)
    {
        _context = context;
    }

    [HttpGet]
    public ActionResult<IEnumerable<Session>> GetSession(Session? session, string? auditoriumname = null, DateTime? periodStart = null, DateTime? periodEnd = null, string? movieTitle = null)
    {
        var query = _context.Sessions!.AsQueryable();
        if (auditoriumname != null)
            query = query.Where(x => x.AuditoriumName != null && x.AuditoriumName.ToUpper().Contains(auditoriumname.ToUpper()));
        if (periodStart != null && periodEnd != null)
            query = query.Where(i => periodStart < i.StartTime && i.StartTime < periodEnd);
        if (periodStart != null)
            query = query.Where(i => periodStart <= i.StartTime);
        if (movieTitle != null)
        query = query.Where(s => _context.Movies!.Any(m => m.Id == s.MovieId && m.Title == movieTitle));
        return query.ToList();
    }

    [HttpGet("{id}")]
    public ActionResult<TextReader> GetSession(int id)
    {
        var session = _context.Sessions!.Find(id);

        if (session == null)
        {
            return NotFound();
        }

        return Ok(session);
    }

    [HttpPut("{id}")]
    public IActionResult PutSession(int id, Session session)
    {
        var dbSession = _context.Tests!.AsNoTracking().FirstOrDefault(x => x.Id == session.Id);
        if (id != session.Id || dbSession == null)
        {
            return NotFound();
        }

        _context.Update(session);
        _context.SaveChanges();

        return NoContent();
    }

    [HttpPost]
    public ActionResult<Session> PostSession(Session session, string? SeatNo = null)
    {
        var query = _context.Tickets.AsQueryable();
        var dbMovies = _context.Movies!.Find(session.MovieId);
        if (dbMovies == null)
        {
            return BadRequest();
        }
        if (SeatNo != null)
        {
            query = query.Where(x => x.SeatNo != null && x.SeatNo.ToUpper().Contains(SeatNo.ToUpper()));

            return BadRequest();
        }

        var timeNow = DateTime.Now;
        var startTime = session.StartTime;
        if (timeNow > startTime)
        {
            return BadRequest();
        }
        var dbSession = _context.Sessions!.Find(session.Id);
        if (dbSession == null)
        {
            _context.Add(session);
            _context.SaveChanges();

            return CreatedAtAction(nameof(GetSession), new { Id = session.Id }, session);
        }
        else
        {
            return Conflict();
        }
    }

    [HttpDelete("{id}")]
    public IActionResult DeleteSession(int id)
    {
        var session = _context.Sessions!.Find(id);
        if (session == null)
        {
            return NotFound();
        }

        _context.Remove(session);
        _context.SaveChanges();

        return Ok();
    }
}
