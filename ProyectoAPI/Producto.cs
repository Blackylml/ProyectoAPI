using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ProyectoAPI
{
    public class Producto
    {
        [Key] // Esto indica que idCliente es la clave primaria
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int idProducto{ get; set; }
        public string nombre { get; set; } = string.Empty;
        public int precio { get; set; } 
        public int cantidad { get; set; } 

        public int status { get; set; }
    }
    
}
