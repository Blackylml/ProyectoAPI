using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace ProyectoAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompraController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly ILogger<CompraController> _logger;

        public CompraController(DataContext context, ILogger<CompraController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpPost("agregar")]
        public async Task<ActionResult<Compra>> AgregarCompra([FromBody] Compra nuevaCompra)
        {
            nuevaCompra.status = 1; // Asume que el status por defecto es 1 para compras activas

            _context.Compra.Add(nuevaCompra);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Compra agregada: {IdCompra}", nuevaCompra.idCompra);
            return CreatedAtAction(nameof(AgregarCompra), new { id = nuevaCompra.idCompra }, nuevaCompra);
        }

        [HttpGet("getAll")]
        public async Task<ActionResult<IEnumerable<Compra>>> GetAllCompras()
        {
            _logger.LogInformation("Obteniendo todas las compras activas");
            var compras = await _context.Compra.Where(c => c.status == 1).ToListAsync();
            return Ok(compras);
        }

        [HttpGet("{idCompra}")]
        public async Task<ActionResult<Compra>> GetCompra(int idCompra)
        {
            var compra = await _context.Compra.FindAsync(idCompra);
            if (compra == null)
            {
                return NotFound("Compra no encontrada");
            }
            return Ok(compra);
        }

        [HttpPut("{idCompra}/eliminar")]
        public async Task<IActionResult> EliminarCompra(int idCompra)
        {
            var compra = await _context.Compra.FindAsync(idCompra);
            if (compra == null)
            {
                return NotFound("Compra no encontrada");
            }

            compra.status = 0;
            await _context.SaveChangesAsync();

            _logger.LogInformation("Compra eliminada (estado cambiado a 0): {IdCompra}", idCompra);
            return NoContent();
        }

        [HttpPut("{idCompra}")]
        public async Task<IActionResult> ModificarCompra(int idCompra, [FromBody] Compra compraActualizada)
        {
            var compraExistente = await _context.Compra.FindAsync(idCompra);
            if (compraExistente == null)
            {
                return NotFound("Compra no encontrada.");
            }

            compraExistente.cantidad_comprada = compraActualizada.cantidad_comprada;
            compraExistente.fecha = compraActualizada.fecha;
            compraExistente.precio_unitario = compraActualizada.precio_unitario;
            compraExistente.nombre = compraActualizada.nombre;
            compraExistente.status = compraActualizada.status;

            await _context.SaveChangesAsync();

            _logger.LogInformation("Compra modificada: {IdCompra}", compraExistente.idCompra);
            return NoContent();
        }
    }
}
