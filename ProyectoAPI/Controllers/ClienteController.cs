using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace ProyectoAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClienteController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly ILogger<ClienteController> _logger;

        public ClienteController(DataContext context, ILogger<ClienteController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpPost("agregar")]
        public async Task<ActionResult<Cliente>> AgregarCliente([FromBody] Cliente nuevoCliente)
        {
            if (await _context.Cliente.AnyAsync(c => c.correo_electronico == nuevoCliente.correo_electronico))
            {
                return BadRequest("Cliente con ese correo electrónico ya existe.");
            }

            nuevoCliente.status = 1; // Asume que el status por defecto es 1 para clientes activos

            _context.Cliente.Add(nuevoCliente);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Cliente agregado: {Nombre}", nuevoCliente.nombre);
            return CreatedAtAction(nameof(AgregarCliente), new { id = nuevoCliente.idCliente }, nuevoCliente);
        }

        [HttpGet("getAll")]
        public async Task<ActionResult<IEnumerable<Cliente>>> GetAllClientes()
        {
            _logger.LogInformation("Obteniendo todos los clientes activos");
            var clientes = await _context.Cliente.Where(c => c.status == 1).ToListAsync();
            return Ok(clientes);
        }

        [HttpGet("{idCliente}")]
        public async Task<ActionResult<Cliente>> GetCliente(int idCliente)
        {
            var cliente = await _context.Cliente.FindAsync(idCliente);
            if (cliente == null)
            {
                return NotFound("Cliente no encontrado");
            }
            return Ok(cliente);
        }

        [HttpPut("{idCliente}/eliminar")]
        public async Task<IActionResult> EliminarCliente(int idCliente)
        {
            var cliente = await _context.Cliente.FindAsync(idCliente);
            if (cliente == null)
            {
                return NotFound("Cliente no encontrado");
            }

            cliente.status = 0;
            await _context.SaveChangesAsync();

            _logger.LogInformation("Cliente eliminado (estado cambiado a 0): {idCliente}", idCliente);
            return NoContent();
        }

        [HttpPut("{idCliente}")]
        public async Task<IActionResult> ModificarCliente(int idCliente, [FromBody] Cliente clienteActualizado)
        {
            var clienteExistente = await _context.Cliente.FindAsync(idCliente);
            if (clienteExistente == null)
            {
                return NotFound("Cliente no encontrado.");
            }

            clienteExistente.nombre = clienteActualizado.nombre;
            clienteExistente.direccion = clienteActualizado.direccion;
            clienteExistente.correo_electronico = clienteActualizado.correo_electronico;
            clienteExistente.telefono = clienteActualizado.telefono;
            clienteExistente.status = clienteActualizado.status;

            await _context.SaveChangesAsync();

            _logger.LogInformation("Cliente modificado: {Nombre}", clienteExistente.nombre);
            return NoContent();
        }
    }
}
