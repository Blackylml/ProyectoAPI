using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ProyectoAPI
{
    public class Compra
    {
        [Key] // Esto indica que idCliente es la clave primaria
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int idCompra { get; set; }
        public int cantidad_comprada { get; set; }
        public string nombre { get; set; } 
        public string fecha { get; set; }
        public int precio_unitario { get; set; }
        public int status { get; set; }
    }
}
