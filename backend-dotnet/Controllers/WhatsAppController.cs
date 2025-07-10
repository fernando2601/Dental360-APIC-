using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace DentalSpa.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class WhatsAppController : ControllerBase
    {
        [HttpGet("conversations")]
        public Task<ActionResult> GetConversations(
            [FromQuery] int page = 1,
            [FromQuery] int limit = 25,
            [FromQuery] string? status = null)
        {
            return Task.FromResult<ActionResult>(Ok(new
            {
                message = "Conversations retrieved",
                page,
                limit,
                status
            }));
        }

        [HttpGet("conversations/{id}")]
        public Task<ActionResult> GetConversation(int id)
        {
            return Task.FromResult<ActionResult>(Ok(new
            {
                message = "Conversation retrieved",
                id
            }));
        }

        [HttpGet("conversations/{id}/messages")]
        public Task<ActionResult> GetMessages(int id)
        {
            return Task.FromResult<ActionResult>(Ok(new
            {
                message = "Messages retrieved",
                id
            }));
        }

        [HttpPost("conversations/{id}/messages")]
        public Task<ActionResult> SendMessage(int id, [FromBody] object message)
        {
            return Task.FromResult<ActionResult>(Ok(new
            {
                message = "Message sent",
                id,
                data = message
            }));
        }

        [HttpGet("templates")]
        public Task<ActionResult> GetTemplates()
        {
            return Task.FromResult<ActionResult>(Ok(new
            {
                message = "Templates retrieved"
            }));
        }

        [HttpPost("templates")]
        public Task<ActionResult> CreateTemplate([FromBody] object template)
        {
            return Task.FromResult<ActionResult>(Ok(new
            {
                message = "Template created",
                data = template
            }));
        }

        [HttpGet("settings")]
        public Task<ActionResult> GetSettings()
        {
            return Task.FromResult<ActionResult>(Ok(new
            {
                message = "Settings retrieved"
            }));
        }
    }
} 