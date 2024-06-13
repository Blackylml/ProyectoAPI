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
    public class VentaController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly ILogger<VentaController> _logger;

        public VentaController(DataContext context, ILogger<VentaController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpPost("agregar")]
        public async Task<ActionResult<Venta>> AgregarVenta([FromBody] Venta nuevaVenta)
        {
            nuevaVenta.status = 1; // Asume que el status por defecto es 1 para ventas activas

            _context.Venta.Add(nuevaVenta);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Venta agregada: {idVenta}", nuevaVenta.idVenta);
            return CreatedAtAction(nameof(AgregarVenta), new { id = nuevaVenta.idVenta }, nuevaVenta);
        }

        [HttpGet("getAll")]
        public async Task<ActionResult<IEnumerable<Venta>>> GetAllVentas()
        {
            _logger.LogInformation("Obteniendo todas las ventas activas");
            var ventas = await _context.Venta.Where(v => v.status == 1).ToListAsync();
            return Ok(ventas);
        }

        [HttpGet("{idVenta}")]
        public async Task<ActionResult<Venta>> GetVenta(int idVenta)
        {
            var venta = await _context.Venta.FindAsync(idVenta);
            if (venta == null)
            {
                return NotFound("Venta no encontrada");
            }
            return Ok(venta);
        }

        [HttpPut("{idVenta}/eliminar")]
        public async Task<IActionResult> EliminarVenta(int idVenta)
        {
            var venta = await _context.Venta.FindAsync(idVenta);
            if (venta == null)
            {
                return NotFound("Venta no encontrada");
            }

            venta.status = 0;
            await _context.SaveChangesAsync();

            _logger.LogInformation("Venta eliminada (estado cambiado a 0): {idVenta}", idVenta);
            return NoContent();
        }

        [HttpPut("{idVenta}")]
        public async Task<IActionResult> ModificarVenta(int idVenta, [FromBody] Venta ventaActualizada)
        {
            var ventaExistente = await _context.Venta.FindAsync(idVenta);
            if (ventaExistente == null)
            {
                return NotFound("Venta no encontrada.");
            }

            ventaExistente.cantidad_vendida = ventaActualizada.cantidad_vendida;
            ventaExistente.fecha = ventaActualizada.fecha;
            ventaExistente.nombre = ventaActualizada.nombre;
            ventaExistente.precio_unitario = ventaActualizada.precio_unitario;
            ventaExistente.status = ventaActualizada.status;

            await _context.SaveChangesAsync();

            _logger.LogInformation("Venta modificada: {idVenta}", ventaExistente.idVenta);
            return NoContent();
        }
    }
}
