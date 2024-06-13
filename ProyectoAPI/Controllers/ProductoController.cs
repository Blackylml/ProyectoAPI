using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProyectoAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductoController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly ILogger<ProductoController> _logger;

        public ProductoController(DataContext context, ILogger<ProductoController> logger)
        {
            _context = context;
            _logger = logger;
        }

        
        [HttpPost("agregar")]
        public async Task<ActionResult<Producto>> AgregarProducto([FromBody] Producto nuevoProducto)
        {
            if (await _context.Producto.AnyAsync(p => p.nombre == nuevoProducto.nombre))
            {
                return BadRequest("Producto con ese nombre ya existe.");
            }

            nuevoProducto.status = 1; // Asume que el status por defecto es 1 para productos activos

            _context.Producto.Add(nuevoProducto);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Producto agregado: {Nombre}", nuevoProducto.nombre);
            return CreatedAtAction(nameof(AgregarProducto), new { id = nuevoProducto.idProducto }, nuevoProducto);
        }

        [HttpGet("getAll")]
        public async Task<ActionResult<IEnumerable<Producto>>> GetAllProductos()
        {
            _logger.LogInformation("Obteniendo todos los productos activos");
            var productos = await _context.Producto.Where(p => p.status == 1).ToListAsync();
            return Ok(productos);
        }


        [HttpGet("{idProducto}")]
        public async Task<ActionResult<Producto>> GetProducto(int idProducto)
        {
            var producto = await _context.Producto.FindAsync(idProducto);
            if (producto == null)
            {
                return NotFound("Producto no encontrado");
            }
            return Ok(producto);
        }
        [HttpPut("{idProducto}/eliminar")]
        public async Task<IActionResult> EliminarProducto(int idProducto)
        {
            var producto = await _context.Producto.FindAsync(idProducto);
            if (producto == null)
            {
                return NotFound("Producto no encontrado");
            }

            producto.status = 0;
            await _context.SaveChangesAsync();

            _logger.LogInformation("Producto eliminado (estado cambiado a 0): {IdProducto}", idProducto);
            return NoContent();
        }
        [HttpPut("{idProducto}")]
        public async Task<IActionResult> ModificarProducto(int idProducto, [FromBody] Producto productoActualizado)
        {
            var productoExistente = await _context.Producto.FindAsync(idProducto);
            if (productoExistente == null)
            {
                return NotFound("Producto no encontrado.");
            }

            productoExistente.nombre = productoActualizado.nombre;
            productoExistente.precio = productoActualizado.precio;
            productoExistente.cantidad = productoActualizado.cantidad;
            productoExistente.status = productoActualizado.status;

            await _context.SaveChangesAsync();

            _logger.LogInformation("Producto modificado: {Nombre}", productoExistente.nombre);
            return NoContent();
        }

    }
}
