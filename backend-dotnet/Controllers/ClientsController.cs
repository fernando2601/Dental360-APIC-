using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using DentalSpa.Domain.Entities;
using DentalSpa.Application.Interfaces;
using DentalSpa.Application.DTOs;

namespace DentalSpa.API.Controllers
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
        public async Task<ActionResult<IEnumerable<ClientResponse>>> GetAll()
        {
            var clients = await _clientService.GetAllAsync();
            return Ok(clients);
        }

        /// <summary>
        /// Obtém cliente por ID
        /// </summary>
        /// <param name="id">ID do cliente</param>
        /// <returns>Dados do cliente</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<ClientResponse>> GetById(int id)
        {
            var client = await _clientService.GetByIdAsync(id);
            if (client == null)
                return NotFound();
            return Ok(client);
        }

        /// <summary>
        /// Cria um novo cliente
        /// </summary>
        /// <param name="request">Dados do cliente</param>
        /// <returns>Cliente criado</returns>
        [HttpPost]
        public async Task<ActionResult<ClientResponse>> Create([FromBody] ClientCreateRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var response = await _clientService.CreateAsync(request);
            return CreatedAtAction(nameof(GetById), null, response);
        }

        /// <summary>
        /// Atualiza cliente existente
        /// </summary>
        /// <param name="id">ID do cliente</param>
        /// <param name="request">Novos dados do cliente</param>
        /// <returns>Cliente atualizado</returns>
        [HttpPut("{id}")]
        public async Task<ActionResult<ClientResponse>> Update(int id, [FromBody] ClientCreateRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var response = await _clientService.UpdateAsync(id, request);
            if (response == null)
                return NotFound();
            return Ok(response);
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
        public async Task<ActionResult<IEnumerable<Client>>> SearchClients([FromQuery] string searchTerm)
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