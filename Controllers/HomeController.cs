 using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using AplicacionPoe.Models;
using System.Data.SqlClient;
using System.Data;


namespace AplicacionPoe.Controllers
{
    public class HomeController : Controller
    {

        static string db_connection = "Data Source=DESKTOP-6VQJGHU\\SQLEXPRESS666; Initial Catalog=Usuarios; Integrated Security=true; user id=sa; pwd=guicho2017";

        public ActionResult Login()
        {
            return View();
        }

        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Register(LoginUsers user1)
        {
            bool registered;
            string alertMessage;

            if (user1.UserLogin == null || user1.UserPassword == null || user1.confirmPassword==null)//Si falta un espacio por llenar la aplicación avisará al usuario
            {
                ViewData["alertMessage"] = "Llene todos los campos";
                return View();

            }
            else
            {
                if (user1.UserPassword == user1.confirmPassword)//Si la contraseña confirmada es igual a la contraseña inicial se procederá a encriptar la contraseña
                {
                    user1.UserPassword = ConvertSha256(user1.UserPassword);//encriptación

                    using (SqlConnection conn = new SqlConnection(db_connection))//Conexion a base de datos con un string con la ruta de la base de datos
                    {
                        SqlCommand scomm = new SqlCommand("sp_login", conn);//creación de comando tomando el procedimiento almacenado
                        scomm.Parameters.AddWithValue("userL", user1.UserLogin);//parametro del usuario tomado desde el procedimietno almacenado
                        scomm.Parameters.AddWithValue("passwordL", user1.UserPassword);//parametro de lac ontraseña tomado desde el procedimietno almacenado
                        scomm.Parameters.Add("registered", SqlDbType.Bit).Direction = ParameterDirection.Output;
                        scomm.Parameters.Add("alertMessage", SqlDbType.NVarChar, 200).Direction = ParameterDirection.Output;
                        scomm.CommandType = CommandType.StoredProcedure;

                        conn.Open();

                        scomm.ExecuteNonQuery();

                        registered = Convert.ToBoolean(scomm.Parameters["registered"].Value);
                        alertMessage = scomm.Parameters["alertMessage"].Value.ToString();
                    }

                }
                else
                { 
                    ViewData["alertMessage"] = "Las contraseñas no coinciden ";//Alerta si las contraseñas no coinciden
                    return View();
                }

               

                ViewData["alertMessage"] = alertMessage;

                if (registered)//si el usuario fue registrado con exito nos devuelve al login
                {

                    return RedirectToAction("Login", "Home");
                }
                else
                {
                    return View();
                }
            }

          
        }

        [HttpPost]
        public ActionResult Login(LoginUsers user1)
        { 
           if(user1.UserLogin==null || user1.UserPassword == null)// si alguno de los campos no fue ingresado nos devuelve un mensaje
            {
                ViewData["alertMessage"] = "Llene todos los campos ";
                return View();

            }
            else//Si todos los datos fueron ingresados de forma correcta la contraseña se encripta y nos permite ingresar
            {
                user1.UserPassword = ConvertSha256(user1.UserPassword);

                using (SqlConnection conn = new SqlConnection(db_connection))
                {
                    SqlCommand scomm = new SqlCommand("sp_validateUser", conn);
                    scomm.Parameters.AddWithValue("userL", user1.UserLogin);
                    scomm.Parameters.AddWithValue("passwordL", user1.UserPassword);
                    scomm.CommandType = CommandType.StoredProcedure;

                    conn.Open();

                    user1.id = Convert.ToInt32(scomm.ExecuteScalar().ToString());

                }
                if (user1.id != 0)
                {
                    Session["LoginUsers"] = user1;
                    return RedirectToAction("Employee_view", "Principal");
                   
                }
                else
                {
                    ViewData["alertMessage"] = "Usuario o contraseña incorrectos";
                    return View();
                }
            }
           

        }

        public static string ConvertSha256(string s)//funci+on para encriptar la contraseña ingresada
        {

            StringBuilder Sb = new StringBuilder();
            using (SHA256 hash = SHA256Managed.Create())
            {
                Encoding enc = Encoding.UTF8;
                byte[] result = hash.ComputeHash(enc.GetBytes(s));

                foreach (byte b in result)
                    Sb.Append(b.ToString("x2"));

            }

            return Sb.ToString();
        }

    


    }


}
