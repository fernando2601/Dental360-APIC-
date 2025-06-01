using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using DentalSpa.Application.DTOs;
using DentalSpa.Application.Services;

namespace DentalSpa.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ClientsController : ControllerBase
    {
        private readonly IClientService _clientService;

        public ClientsController(IClientService clientService)
        {
            _clientService = clientService;
        }

        /// <summary>
        /// Obtém lista de todos os clientes
        /// </summary>
        /// <returns>Lista de clientes</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ClientDTO>>> GetAllClients()
        {
            try
            {
                var clients = await _clientService.GetAllClientsAsync();
                return Ok(clients);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Obtém cliente por ID
        /// </summary>
        /// <param name="id">ID do cliente</param>
        /// <returns>Dados do cliente</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<ClientDTO>> GetClientById(int id)
        {
            try
            {
                var client = await _clientService.GetClientByIdAsync(id);
                return Ok(client);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Cria um novo cliente
        /// </summary>
        /// <param name="request">Dados do cliente</param>
        /// <returns>Cliente criado</returns>
        [HttpPost]
        public async Task<ActionResult<ClientDTO>> CreateClient([FromBody] ClientCreateRequest request)
        {
            try
            {
                var client = await _clientService.CreateClientAsync(request);
                return CreatedAtAction(nameof(GetClientById), new { id = client.Id }, client);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Atualiza cliente existente
        /// </summary>
        /// <param name="id">ID do cliente</param>
        /// <param name="request">Novos dados do cliente</param>
        /// <returns>Cliente atualizado</returns>
        [HttpPut("{id}")]
        public async Task<ActionResult<ClientDTO>> UpdateClient(int id, [FromBody] ClientUpdateRequest request)
        {
            try
            {
                var client = await _clientService.UpdateClientAsync(id, request);
                return Ok(client);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Remove cliente
        /// </summary>
        /// <param name="id">ID do cliente</param>
        /// <returns>Confirmação de remoção</returns>
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteClient(int id)
        {
            try
            {
                await _clientService.DeleteClientAsync(id);
                return Ok(new { message = "Cliente removido com sucesso" });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Busca clientes por termo
        /// </summary>
        /// <param name="searchTerm">Termo de busca</param>
        /// <returns>Lista de clientes encontrados</returns>
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<ClientDTO>>> SearchClients([FromQuery] string searchTerm)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(searchTerm))
                {
                    return BadRequest(new { message = "Termo de busca é obrigatório" });
                }

                var clients = await _clientService.SearchClientsAsync(searchTerm);
                return Ok(clients);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}