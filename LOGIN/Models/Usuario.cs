using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LOGIN.Models;

public partial class Usuario
{
    [Key]
    public int IdUsuario { get; set; }

    public string? NombreUsuario { get; set; }

    [EmailAddress]
    public string? Correo { get; set; }

    public string? Clave { get; set; }

    public string? ConfirmarClave { get; set; }
}
