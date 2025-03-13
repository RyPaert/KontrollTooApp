using ITB2203Application.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ITB2203Application.Controllers;
[ApiController]
[Route("api/[controller]")]
public class TicketsController : Controller
{
    private readonly DataContext _context;

    public TicketsController(DataContext context)
    {
        _context = context;
    }

    [HttpGet]
    public ActionResult<IEnumerable<Ticket>> GetTicket(string? seatno = null)
    {
        var query = _context.Tickets!.AsQueryable();

        if (seatno != null)
            query = query.Where(x => x.SeatNo != null && x.SeatNo.ToUpper().Contains(seatno.ToUpper()));

        return query.ToList();
    }

    [HttpGet("{id}")]
    public ActionResult<TextReader> GetTicket(int id)
    {
        var ticket = _context.Tickets!.Find(id);

        if (ticket == null)
        {
            return NotFound();
        }

        return Ok(ticket);
    }

    [HttpPut("{id}")]
    public IActionResult PutTicket(int id, Ticket ticket)
    {
        var dbTicket = _context.Tickets!.AsNoTracking().FirstOrDefault(x => x.Id == ticket.Id);
        if (id != ticket.Id || dbTicket == null)
        {
            return NotFound();
        }

        _context.Update(ticket);
        _context.SaveChanges();

        return NoContent();
    }

    [HttpPost]
    public ActionResult<Ticket> PostTicket(Ticket ticket)
    {
        var query = _context.Tickets.AsQueryable();
        var dbTicket = _context.Tickets!.Find(ticket.Id);
        var dbSession = _context.Sessions.Find(ticket.SessionId);
            query = query.Where(x => x.SessionId == ticket.SessionId && x.SeatNo.Equals(ticket.SeatNo));
            if (query.Count() > 0)
            {
                return BadRequest();
            }
            if (dbSession == null)
            {
            return BadRequest();
            }

        if (dbTicket == null)
        {
            if (ticket.Price <= 0)
            {
                return BadRequest();              
            }
         _context.Add(ticket);
        _context.SaveChanges();

        return CreatedAtAction(nameof(GetTicket), new { Id = ticket.Id }, ticket);
        }
        else
        {
            return Conflict();
        }
    }

    [HttpDelete("{id}")]
    public IActionResult DeleteTicket(int id)
    {
        var ticket = _context.Tickets!.Find(id);
        if (ticket == null)
        {
            return NotFound();
        }

        _context.Remove(ticket);
        _context.SaveChanges();

        return NoContent();
    }
}
