using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using ConCord.Models;
using ConCord.Hubs;
using ConCord.Services;

namespace ConCord.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ChannelsController : ControllerBase
{
    private readonly DatabaseContext _context;
    private readonly IHubContext<ChatHub> _hub;
    private readonly IToxicLanguageDetector _toxicDetector;

    public ChannelsController(DatabaseContext context, IHubContext<ChatHub> hub, IToxicLanguageDetector toxicDetector)
    {
        _context = context;
        _hub = hub;
        _toxicDetector = toxicDetector;
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
    [ProducesResponseType(typeof(Message), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Message>> PostChannelMessage(int channelId, Message? message)
    {
        if (message == null || string.IsNullOrWhiteSpace(message.Text))
        {
            return BadRequest(new { message = "Message text cannot be empty." });
        }

        var trimmedText = message.Text.Trim();
        if (trimmedText.Length > 500)
        {
            return BadRequest(new { message = "Message text cannot exceed 500 characters." });
        }

        if (!await _context.Channels.AnyAsync(c => c.Id == channelId))
        {
            return NotFound(new { message = $"Channel with ID {channelId} does not exist." });
        }

        message.Text = trimmedText;
        if (!string.IsNullOrEmpty(message.UserName) && message.UserName.Length > 10)
        {
            message.UserName = message.UserName.Substring(0, 10);
        }

        var toxicity = _toxicDetector.CheckToxicity(message.Text);
        if (toxicity.IsToxic)
        {
            return BadRequest(new
            {
                isToxic = true,
                title = "Toxic Language Detected",
                message = "Your message was flagged as containing toxic or inappropriate language. ConCord promotes a friendly and respectful chat community. Please rephrase your message and refrain from posting toxic language.",
                score = toxicity.ToxicityScore
            });
        }

        message.ChannelId = channelId;
        _context.Messages.Add(message);
        await _context.SaveChangesAsync();
        await _hub.Clients.Group(channelId.ToString()).SendAsync("ReceiveMessage", message);
        return Ok(message);
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