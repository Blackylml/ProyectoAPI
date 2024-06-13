using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
namespace ProyectoAPI
{
    public class Cliente
    {
        [Key] // Esto indica que idCliente es la clave primaria
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int idCliente { get; set; }
        public string nombre { get; set; } = string.Empty;
        public string direccion { get; set; }
        public string correo_electronico { get; set; }
        public string telefono { get; set; }

        public int status { get; set; }
    }
}
