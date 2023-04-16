using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using ConCord.Models;
using ConCord.Hubs;

namespace ConCord.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ChannelsController : ControllerBase
{
    private readonly DatabaseContext _context;
    private readonly IHubContext<ChatHub> _hub;

    public ChannelsController(DatabaseContext context, IHubContext<ChatHub> hub)
    {
        _context = context;
        _hub = hub;
    }

    // GET: api/Channels
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Channel>>> GetChannels()
    {
        return await _context.Channels.ToListAsync();
    }

    // GET: api/Channels/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Channel>> GetChannel(int id)
    {
        
        var channel = await _context.Channels.FindAsync(id);

        if (channel == null)
        {
            return NotFound();
        }

        return channel;
    }

    // PUT: api/Channels/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{id}")]
    public async Task<IActionResult> PutChannel(int id, Channel channel)
    {
        if (id != channel.Id)
        {
            return BadRequest();
        }

        _context.Entry(channel).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!ChannelExists(id))
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

    // POST: api/Channels
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    public async Task<ActionResult<Channel>> PostChannel(Channel channel)
    {
        _context.Channels.Add(channel);
        await _context.SaveChangesAsync();

        return CreatedAtAction("GetChannel", new { id = channel.Id }, channel);
    }

    // GET: api/Channels/1/Messages
    [HttpGet("{channelId}/Messages")]
    public async Task<ActionResult<IEnumerable<Message>>> GetChannelMessages(int channelId)
    {
        return await _context.Messages.Where(m => m.ChannelId == channelId).ToListAsync();
    }

    // POST: api/Channels/1/Messages
    [HttpPost("{channelId}/Messages")]
    public async Task<Message> PostChannelMessage(int channelId, Message message)
    {
        message.ChannelId = channelId;
        _context.Messages.Add(message);
        await _context.SaveChangesAsync();
        await _hub.Clients.Group(channelId.ToString()).SendAsync("ReceiveMessage", message);
        //return CreatedAtAction(nameof (GetChannel), new { id = message.Id }, message);
        return message;
    }

    // DELETE: api/Channels/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteChannel(int id)
    {
        var channel = await _context.Channels.FindAsync(id);
        if (channel == null)
        {
            return NotFound();
        }

        _context.Channels.Remove(channel);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool ChannelExists(int id)
    {
        return _context.Channels.Any(e => e.Id == id);
    }
}