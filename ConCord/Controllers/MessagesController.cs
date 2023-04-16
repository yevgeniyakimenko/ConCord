using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ConCord.Models;

namespace ConCord.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MessagesController : ControllerBase
{
  private readonly DatabaseContext _context;
  public MessagesController(DatabaseContext context)
  {
    _context = context;
  }

  // GET: api/Messages
  [HttpGet]
  public async Task<ActionResult<IEnumerable<Message>>> GetMessages()
  {
    return await _context.Messages.ToListAsync();
  }
}