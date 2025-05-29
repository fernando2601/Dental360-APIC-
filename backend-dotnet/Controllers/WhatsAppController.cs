using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ClinicApi.Models;
using ClinicApi.Services;

namespace ClinicApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class WhatsAppController : ControllerBase
    {
        private readonly IWhatsAppService _whatsAppService;

        public WhatsAppController(IWhatsAppService whatsAppService)
        {
            _whatsAppService = whatsAppService;
        }

        // Conversas
        [HttpGet("conversations")]
        public async Task<ActionResult> GetConversations(
            [FromQuery] int page = 1,
            [FromQuery] int limit = 25,
            [FromQuery] string? status = null)
        {
            var conversations = await _whatsAppService.GetConversationsAsync(page, limit, status);
            return Ok(conversations);
        }

        [HttpGet("conversations/{id}")]
        public async Task<ActionResult> GetConversation(int id)
        {
            var conversation = await _whatsAppService.GetConversationByIdAsync(id);
            if (conversation == null)
                return NotFound();

            return Ok(conversation);
        }

        // Mensagens
        [HttpGet("conversations/{id}/messages")]
        public async Task<ActionResult> GetMessages(int id)
        {
            var messages = await _whatsAppService.GetMessagesAsync(id);
            return Ok(messages);
        }

        [HttpPost("conversations/{id}/messages")]
        public async Task<ActionResult> SendMessage(int id, [FromBody] SendMessageDto messageDto)
        {
            var result = await _whatsAppService.SendMessageAsync(id, messageDto);
            return Ok(result);
        }

        // Templates
        [HttpGet("templates")]
        public async Task<ActionResult> GetTemplates()
        {
            var templates = await _whatsAppService.GetTemplatesAsync();
            return Ok(templates);
        }

        [HttpPost("templates")]
        public async Task<ActionResult> CreateTemplate([FromBody] CreateTemplateDto templateDto)
        {
            var template = await _whatsAppService.CreateTemplateAsync(templateDto);
            return Ok(template);
        }

        [HttpPost("templates/{id}/send")]
        public async Task<ActionResult> SendTemplate(int id, [FromBody] SendTemplateDto sendDto)
        {
            var result = await _whatsAppService.SendTemplateAsync(id, sendDto);
            return Ok(result);
        }

        // Automação
        [HttpGet("automation/rules")]
        public async Task<ActionResult> GetAutomationRules()
        {
            var rules = await _whatsAppService.GetAutomationRulesAsync();
            return Ok(rules);
        }

        [HttpPost("automation/rules")]
        public async Task<ActionResult> CreateAutomationRule([FromBody] CreateAutomationRuleDto ruleDto)
        {
            var rule = await _whatsAppService.CreateAutomationRuleAsync(ruleDto);
            return Ok(rule);
        }

        // Relatórios
        [HttpGet("reports/messages")]
        public async Task<ActionResult> GetMessageReports(
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null)
        {
            var reports = await _whatsAppService.GetMessageReportsAsync(startDate, endDate);
            return Ok(reports);
        }

        // Configurações
        [HttpGet("settings")]
        public async Task<ActionResult> GetWhatsAppSettings()
        {
            var settings = await _whatsAppService.GetSettingsAsync();
            return Ok(settings);
        }

        [HttpPut("settings")]
        public async Task<ActionResult> UpdateWhatsAppSettings([FromBody] WhatsAppSettingsDto settingsDto)
        {
            var result = await _whatsAppService.UpdateSettingsAsync(settingsDto);
            return Ok(result);
        }
    }
}