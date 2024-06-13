using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ProyectoAPI
{
    public class Recordatorio
    {
        [Key] // Esto indica que idCliente es la clave primaria
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int idRecordatorio { get; set; }
        public string descripcion { get; set; }
        public string fecha { get; set; }

    }
}
