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
    public class ProveedorController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly ILogger<ProveedorController> _logger;

        public ProveedorController(DataContext context, ILogger<ProveedorController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpPost("agregar")]
        public async Task<ActionResult<Proveedor>> AgregarProveedor([FromBody] Proveedor nuevoProveedor)
        {
            if (await _context.Proveedor.AnyAsync(p => p.correo_electronico == nuevoProveedor.correo_electronico))
            {
                return BadRequest("Proveedor con ese correo electrónico ya existe.");
            }

            nuevoProveedor.status = 1; // Asume que el status por defecto es 1 para proveedores activos

            _context.Proveedor.Add(nuevoProveedor);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Proveedor agregado: {Nombre}", nuevoProveedor.nombre);
            return CreatedAtAction(nameof(AgregarProveedor), new { id = nuevoProveedor.idProveedor }, nuevoProveedor);
        }

        [HttpGet("getAll")]
        public async Task<ActionResult<IEnumerable<Proveedor>>> GetAllProveedores()
        {
            _logger.LogInformation("Obteniendo todos los proveedores activos");
            var proveedores = await _context.Proveedor.Where(p => p.status == 1).ToListAsync();
            return Ok(proveedores);
        }

        [HttpGet("{idProveedor}")]
        public async Task<ActionResult<Proveedor>> GetProveedor(int idProveedor)
        {
            var proveedor = await _context.Proveedor.FindAsync(idProveedor);
            if (proveedor == null)
            {
                return NotFound("Proveedor no encontrado");
            }
            return Ok(proveedor);
        }

        [HttpPut("{idProveedor}/eliminar")]
        public async Task<IActionResult> EliminarProveedor(int idProveedor)
        {
            var proveedor = await _context.Proveedor.FindAsync(idProveedor);
            if (proveedor == null)
            {
                return NotFound("Proveedor no encontrado");
            }

            proveedor.status = 0;
            await _context.SaveChangesAsync();

            _logger.LogInformation("Proveedor eliminado (estado cambiado a 0): {idProveedor}", idProveedor);
            return NoContent();
        }

        [HttpPut("{idProveedor}")]
        public async Task<IActionResult> ModificarProveedor(int idProveedor, [FromBody] Proveedor proveedorActualizado)
        {
            var proveedorExistente = await _context.Proveedor.FindAsync(idProveedor);
            if (proveedorExistente == null)
            {
                return NotFound("Proveedor no encontrado.");
            }

            proveedorExistente.nombre = proveedorActualizado.nombre;
            proveedorExistente.direccion = proveedorActualizado.direccion;
            proveedorExistente.correo_electronico = proveedorActualizado.correo_electronico;
            proveedorExistente.telefono = proveedorActualizado.telefono;
            proveedorExistente.status = proveedorActualizado.status;

            await _context.SaveChangesAsync();

            _logger.LogInformation("Proveedor modificado: {Nombre}", proveedorExistente.nombre);
            return NoContent();
        }
    }
}
