using Microsoft.AspNetCore.Mvc;
using ProyectoAPI;
using Microsoft.EntityFrameworkCore;

[Route("api/[controller]")]
[ApiController]
public class RecordatorioController : ControllerBase
{
    private readonly DataContext _context;

    public RecordatorioController(DataContext context)
    {
        _context = context;
    }

    [HttpGet("getAll")]
    public async Task<ActionResult<IEnumerable<Recordatorio>>> GetRecordatorios()
    {
        return await _context.Recordatorio.ToListAsync();
    }

    [HttpPost("add")]
    public async Task<ActionResult<Recordatorio>> AddRecordatorio(Recordatorio recordatorio)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        _context.Recordatorio.Add(recordatorio);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetRecordatorio), new { id = recordatorio.idRecordatorio }, recordatorio);
    }


    [HttpGet("{id}")]
    public async Task<ActionResult<Recordatorio>> GetRecordatorio(int id)
    {
        var recordatorio = await _context.Recordatorio.FindAsync(id);

        if (recordatorio == null)
        {
            return NotFound();
        }

        return recordatorio;
    }

    // Otros métodos para actualizar y eliminar recordatorios...
}
