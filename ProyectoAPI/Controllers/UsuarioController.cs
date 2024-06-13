using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;



namespace ProyectoAPI.Controllers

{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuarioController : ControllerBase
    {               
        private readonly DataContext _context;
        private readonly ILogger<UsuarioController> _logger;
    

        public UsuarioController(DataContext context, ILogger<UsuarioController> logger)
        {
            _context = context;
            _logger = logger;
        }
        [HttpPost("register")]
        public async Task<ActionResult<Usuario>> Register([FromBody] RegisterRequest registerRequest)
        {
            if (await _context.Usuario.AnyAsync(u => u.correoElectronico == registerRequest.CorreoElectronico))
            {
                return BadRequest("Correo electrónico ya está registrado.");
            }

            var nuevoUsuario = new Usuario
            {
                nombre = registerRequest.Nombre,
                correoElectronico = registerRequest.CorreoElectronico,
                password = registerRequest.Password, // Almacenando la contraseña sin encriptar (no recomendado para producción)
            };

            _context.Usuario.Add(nuevoUsuario);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Usuario registrado: {Nombre}", registerRequest.Nombre);
            return CreatedAtAction(nameof(Register), new { id = nuevoUsuario.idUsuario }, nuevoUsuario);
        }



        [HttpPost("login")]
        public async Task<ActionResult<Usuario>> Login([FromBody] LoginRequest loginRequest)
        {
            // Imprimir los datos recibidos en la consola para depuración
            Console.WriteLine($"Received login request: CorreoElectronico = {loginRequest.CorreoElectronico}, Password = {loginRequest.Password}");
            _logger.LogInformation($"Received login request: CorreoElectronico = {loginRequest.CorreoElectronico}, Password = {loginRequest.Password}");

            _logger.LogInformation("Intento de login para: {CorreoElectronico}", loginRequest.CorreoElectronico);

            var usuario = await _context.Usuario.FirstOrDefaultAsync(u => u.correoElectronico == loginRequest.CorreoElectronico);

            if (usuario == null)
            {
                _logger.LogWarning("Usuario no encontrado: {CorreoElectronico}", loginRequest.CorreoElectronico);
                return NotFound("Usuario no encontrado");
            }

            if (usuario.password != loginRequest.Password) // Considera usar un método de hash para verificar la contraseña
            {
                _logger.LogWarning("Contraseña incorrecta para el usuario: {CorreoElectronico}", loginRequest.CorreoElectronico);
                return BadRequest("Contraseña incorrecta");
            }

            _logger.LogInformation("Usuario logueado correctamente: {CorreoElectronico}", loginRequest.CorreoElectronico);
            return Ok(usuario);
        }

        // Método GET para verificar la conexión y obtener todos los usuarios
        [HttpGet("getAll")]
        public async Task<ActionResult<IEnumerable<Usuario>>> GetAll()
        {
            _logger.LogInformation("Obteniendo todos los usuarios");
            var usuarios = await _context.Usuario.ToListAsync();
            return Ok(usuarios);
        }
    }
}
