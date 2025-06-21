using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using DentalSpa.Application.Interfaces;
using DentalSpa.Domain.Entities;

namespace DentalSpa.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class SubscriptionsController : ControllerBase
    {
        private readonly ISubscriptionService _subscriptionService;
        private readonly ILogger<SubscriptionsController> _logger;

        public SubscriptionsController(ISubscriptionService subscriptionService, ILogger<SubscriptionsController> logger)
        {
            _subscriptionService = subscriptionService;
            _logger = logger;
        }

        // Subscription endpoints
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Subscription>>> GetAllSubscriptions()
        {
            try
            {
                var subscriptions = await _subscriptionService.GetAllSubscriptionsAsync();
                return Ok(subscriptions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar assinaturas");
                return StatusCode(500, new { message = "Erro interno do servidor" });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Subscription>> GetSubscription(int id)
        {
            try
            {
                var subscription = await _subscriptionService.GetSubscriptionByIdAsync(id);
                if (subscription == null)
                {
                    return NotFound(new { message = "Assinatura não encontrada" });
                }
                return Ok(subscription);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar assinatura: {Id}", id);
                return StatusCode(500, new { message = "Erro interno do servidor" });
            }
        }

        [HttpPost]
        public async Task<ActionResult<Subscription>> CreateSubscription([FromBody] Subscription subscription)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var entity = await _subscriptionService.CreateSubscriptionAsync(subscription);
                return CreatedAtAction(nameof(GetSubscription), new { id = entity.Id }, entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar assinatura");
                return StatusCode(500, new { message = "Erro interno do servidor" });
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Subscription>> UpdateSubscription(int id, [FromBody] Subscription subscription)
        {
            try
            {
                if (id != subscription.Id)
                {
                    return BadRequest(new { message = "ID da assinatura não confere" });
                }
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var entity = await _subscriptionService.UpdateSubscriptionAsync(subscription);
                return Ok(entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar assinatura: {Id}", id);
                return StatusCode(500, new { message = "Erro interno do servidor" });
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteSubscription(int id)
        {
            try
            {
                var success = await _subscriptionService.DeleteSubscriptionAsync(id);
                if (!success)
                {
                    return NotFound(new { message = "Assinatura não encontrada" });
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao deletar assinatura: {Id}", id);
                return StatusCode(500, new { message = "Erro interno do servidor" });
            }
        }

        // Client subscription endpoints
        [HttpGet("clients")]
        public async Task<ActionResult<IEnumerable<ClientSubscription>>> GetAllClientSubscriptions()
        {
            try
            {
                var clientSubscriptions = await _subscriptionService.GetAllClientSubscriptionsAsync();
                return Ok(clientSubscriptions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar assinaturas de clientes");
                return StatusCode(500, new { message = "Erro interno do servidor" });
            }
        }

        [HttpGet("clients/{id}")]
        public async Task<ActionResult<ClientSubscription>> GetClientSubscription(int id)
        {
            try
            {
                var clientSubscription = await _subscriptionService.GetClientSubscriptionByIdAsync(id);
                if (clientSubscription == null)
                {
                    return NotFound(new { message = "Assinatura de cliente não encontrada" });
                }
                return Ok(clientSubscription);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar assinatura de cliente: {Id}", id);
                return StatusCode(500, new { message = "Erro interno do servidor" });
            }
        }

        [HttpPost("clients")]
        public async Task<ActionResult<ClientSubscription>> CreateClientSubscription([FromBody] ClientSubscription clientSubscription)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var entity = await _subscriptionService.CreateClientSubscriptionAsync(clientSubscription);
                return CreatedAtAction(nameof(GetClientSubscription), new { id = entity.Id }, entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar assinatura de cliente");
                return StatusCode(500, new { message = "Erro interno do servidor" });
            }
        }

        [HttpPut("clients/{id}")]
        public async Task<ActionResult<ClientSubscription>> UpdateClientSubscription(int id, [FromBody] ClientSubscription clientSubscription)
        {
            try
            {
                if (id != clientSubscription.Id)
                {
                    return BadRequest(new { message = "ID da assinatura não confere" });
                }
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var entity = await _subscriptionService.UpdateClientSubscriptionAsync(clientSubscription);
                return Ok(entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar assinatura de cliente: {Id}", id);
                return StatusCode(500, new { message = "Erro interno do servidor" });
            }
        }

        [HttpDelete("clients/{id}")]
        public async Task<ActionResult> DeleteClientSubscription(int id)
        {
            try
            {
                var success = await _subscriptionService.DeleteClientSubscriptionAsync(id);
                if (!success)
                {
                    return NotFound(new { message = "Assinatura de cliente não encontrada" });
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao deletar assinatura de cliente: {Id}", id);
                return StatusCode(500, new { message = "Erro interno do servidor" });
            }
        }

        [HttpPost("clients/{id}/activate")]
        public async Task<ActionResult> ActivateSubscription(int id)
        {
            try
            {
                var success = await _subscriptionService.ActivateClientSubscriptionAsync(id);
                if (!success)
                {
                    return NotFound(new { message = "Assinatura de cliente não encontrada" });
                }

                return Ok(new { message = "Assinatura ativada com sucesso" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao ativar assinatura: {Id}", id);
                return StatusCode(500, new { message = "Erro interno do servidor" });
            }
        }

        [HttpPost("clients/{id}/suspend")]
        public async Task<ActionResult> SuspendSubscription(int id)
        {
            try
            {
                var success = await _subscriptionService.SuspendClientSubscriptionAsync(id);
                if (!success)
                {
                    return NotFound(new { message = "Assinatura de cliente não encontrada" });
                }

                return Ok(new { message = "Assinatura suspensa com sucesso" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao suspender assinatura: {Id}", id);
                return StatusCode(500, new { message = "Erro interno do servidor" });
            }
        }

        [HttpPost("clients/{id}/cancel")]
        public async Task<ActionResult> CancelSubscription(int id)
        {
            try
            {
                var success = await _subscriptionService.CancelClientSubscriptionAsync(id);
                if (!success)
                {
                    return NotFound(new { message = "Assinatura de cliente não encontrada" });
                }

                return Ok(new { message = "Assinatura cancelada com sucesso" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao cancelar assinatura: {Id}", id);
                return StatusCode(500, new { message = "Erro interno do servidor" });
            }
        }

        [HttpPost("clients/{id}/renew")]
        public async Task<ActionResult> RenewSubscription(int id)
        {
            try
            {
                var success = await _subscriptionService.RenewClientSubscriptionAsync(id);
                if (!success)
                {
                    return NotFound(new { message = "Assinatura de cliente não encontrada" });
                }

                return Ok(new { message = "Assinatura renovada com sucesso" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao renovar assinatura: {Id}", id);
                return StatusCode(500, new { message = "Erro interno do servidor" });
            }
        }

        [HttpGet("clients/by-client/{clientId}")]
        public async Task<ActionResult<IEnumerable<ClientSubscription>>> GetClientSubscriptionsByClientId(int clientId)
        {
            try
            {
                var subscriptions = await _subscriptionService.GetClientSubscriptionsByClientIdAsync(clientId);
                return Ok(subscriptions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar assinaturas do cliente: {ClientId}", clientId);
                return StatusCode(500, new { message = "Erro interno do servidor" });
            }
        }

        [HttpGet("clients/active/{clientId}")]
        public async Task<ActionResult<ClientSubscription>> GetActiveClientSubscription(int clientId)
        {
            try
            {
                var subscription = await _subscriptionService.GetActiveClientSubscriptionAsync(clientId);
                if (subscription == null)
                {
                    return NotFound(new { message = "Nenhuma assinatura ativa encontrada para este cliente" });
                }
                return Ok(subscription);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar assinatura ativa do cliente: {ClientId}", clientId);
                return StatusCode(500, new { message = "Erro interno do servidor" });
            }
        }

        [HttpGet("expired")]
        public async Task<ActionResult<IEnumerable<ClientSubscription>>> GetExpiredSubscriptions()
        {
            try
            {
                var subscriptions = await _subscriptionService.GetExpiredSubscriptionsAsync();
                return Ok(subscriptions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar assinaturas expiradas");
                return StatusCode(500, new { message = "Erro interno do servidor" });
            }
        }

        [HttpGet("renewal-due")]
        public async Task<ActionResult<IEnumerable<ClientSubscription>>> GetSubscriptionsDueForRenewal([FromQuery] int daysAhead = 7)
        {
            try
            {
                var subscriptions = await _subscriptionService.GetSubscriptionsDueForRenewalAsync(daysAhead);
                return Ok(subscriptions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar assinaturas com renovação pendente");
                return StatusCode(500, new { message = "Erro interno do servidor" });
            }
        }
    }
}