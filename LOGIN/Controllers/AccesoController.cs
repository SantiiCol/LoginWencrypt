using LOGIN.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Security.Cryptography;
using System.Text;

public class AccesoController : Controller
{
    private readonly string _cadena;

    public AccesoController(IConfiguration configuration)
    {
        _cadena = configuration.GetConnectionString("Conexion");
    }

    public IActionResult Login()
    {
        return View();
    }

    public IActionResult Registrar()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Registrar(Usuario usuario)
    {
        if (usuario.Clave != usuario.ConfirmarClave)
        {
            ViewData["Mensaje"] = "Las contraseñas no coinciden";
            return View();
        }

        usuario.Clave = ConvertirSha256(usuario.Clave);

        bool registrado;
        string mensaje;

        using (SqlConnection cn = new SqlConnection(_cadena))
        {
            SqlCommand cmd = new SqlCommand("sp_RegistrarUsuario", cn);
            cmd.Parameters.AddWithValue("NombreUsuario", usuario.NombreUsuario);
            cmd.Parameters.AddWithValue("Correo", usuario.Correo);
            cmd.Parameters.AddWithValue("Clave", usuario.Clave);
            cmd.Parameters.Add("Registrado", SqlDbType.Bit).Direction = ParameterDirection.Output;
            cmd.Parameters.Add("Mensaje", SqlDbType.NVarChar, 100).Direction = ParameterDirection.Output;
            cmd.CommandType = CommandType.StoredProcedure;

            cn.Open();
            cmd.ExecuteNonQuery();

            registrado = Convert.ToBoolean(cmd.Parameters["Registrado"].Value);
            mensaje = cmd.Parameters["Mensaje"].Value.ToString();
        }

        ViewData["Mensaje"] = mensaje;

        if (registrado)
        {
            return RedirectToAction("Login", "Acceso");
        }
        else
        {
            return View();
        }
    }

    [HttpPost]
    public IActionResult Login(Usuario usuario)
    {
        usuario.Clave = ConvertirSha256(usuario.Clave);

        using (SqlConnection cn = new SqlConnection(_cadena))
        {
            SqlCommand cmd = new SqlCommand("sp_ValidarUsuario", cn);
            cmd.Parameters.AddWithValue("Correo", usuario.Correo);
            cmd.Parameters.AddWithValue("Clave", usuario.Clave);
            cmd.CommandType = CommandType.StoredProcedure;

            cn.Open();

            object result = cmd.ExecuteScalar();

            if (result != null)
            {
                usuario.IdUsuario = Convert.ToInt32(result.ToString());
            }
            else
            {
                usuario.IdUsuario = 0;
            }
        }

        if (usuario.IdUsuario != 0)
        {
            HttpContext.Session.SetInt32("IdUsuario", usuario.IdUsuario);
            return RedirectToAction("Index", "Home");
        }
        else
        {
            ViewData["Mensaje"] = "Usuario no encontrado";
            return View();
        }
    }

    public static string ConvertirSha256(string texto)
    {
        StringBuilder sb = new StringBuilder();
        using (SHA256 hash = SHA256Managed.Create())
        {
            Encoding enc = Encoding.UTF8;
            byte[] result = hash.ComputeHash(enc.GetBytes(texto));

            foreach (byte b in result)
                sb.Append(b.ToString("x2"));
        }

        return sb.ToString();
    }
}