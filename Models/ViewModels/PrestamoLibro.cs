namespace Models.ViewModels
{
    public class PrestamoLibro : Libro
    {

        public PrestamoLibro(Libro l)
        {
            IdLibro = l.IdLibro;
            IdStock = l.IdStock;
            ClasificacionEdad = l.ClasificacionEdad;
            FechaPublicacion = l.FechaPublicacion;
            ISBN = l.ISBN;
            Titulo = l.Titulo;
            Descripcion = l.Descripcion;
            ImagenUrl = l.ImagenUrl;
            Activo = l.Activo;
        }

        public bool Disponible { get; set; }
    }
}