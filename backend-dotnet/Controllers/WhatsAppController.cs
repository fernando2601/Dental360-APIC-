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
        public async Task<ActionResult> GetConversations(
            [FromQuery] int page = 1,
            [FromQuery] int limit = 25,
            [FromQuery] string? status = null)
        {
            return Ok(new
            {
                message = "Conversations retrieved",
                page,
                limit,
                status
            });
        }

        [HttpGet("conversations/{id}")]
        public async Task<ActionResult> GetConversation(int id)
        {
            return Ok(new
            {
                message = "Conversation retrieved",
                id
            });
        }

        [HttpGet("conversations/{id}/messages")]
        public async Task<ActionResult> GetMessages(int id)
        {
            return Ok(new
            {
                message = "Messages retrieved",
                id
            });
        }

        [HttpPost("conversations/{id}/messages")]
        public async Task<ActionResult> SendMessage(int id, [FromBody] object message)
        {
            return Ok(new
            {
                message = "Message sent",
                id,
                data = message
            });
        }

        [HttpGet("templates")]
        public async Task<ActionResult> GetTemplates()
        {
            return Ok(new
            {
                message = "Templates retrieved"
            });
        }

        [HttpPost("templates")]
        public async Task<ActionResult> CreateTemplate([FromBody] object template)
        {
            return Ok(new
            {
                message = "Template created",
                data = template
            });
        }

        [HttpGet("settings")]
        public async Task<ActionResult> GetSettings()
        {
            return Ok(new
            {
                message = "Settings retrieved"
            });
        }
    }
} 