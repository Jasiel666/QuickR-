
using AplicacionPoe.Models;
using AplicacionPoe.Sessions;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity.Infrastructure;
using System.Web.UI.WebControls;
using AplicacionPoe.Models.ViewModels;
using System.IO;
using Microsoft.Ajax.Utilities;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;

namespace AplicacionPoe.Controllers
{

   [ValidateSession]
    public class PrincipalController : Controller
    {
        // GET: Principal
        CompanyEntities6 dbo = new CompanyEntities6();
        static string db_connection1 = "Data Source=DESKTOP-6VQJGHU\\SQLEXPRESS666; Initial Catalog=Company; Integrated Security=true; user id=sa; pwd=guicho2017";

        ////Seccion Empleados////
        public ActionResult Employee_view()//Función que devuelve los elementos del tipo empleado y los refleja en la tabla de empleados
        {
            if (ModelState.IsValid)
            {
                List<employeeO> select_object;
                using (CompanyEntities6 data_employee = new CompanyEntities6())
                {

                    select_object = (from employee1 in data_employee.employee
                                     select new employeeO
                                     {
                                         employee_id = employee1.employee_id,
                                         e_name = employee1.e_name,
                                         e_admission_date = employee1.e_admission_date,
                                         e_supervisor = employee1.e_supervisor,
                                         e_observation = employee1.e_observation,
                                         e_certificate = employee1.e_certificate,
                                         e_active = employee1.e_active

                                     }).ToList();

                }

                return View(select_object);
            }

            return View();
        }

        [HttpPost]
        public ActionResult Employee_register(employee employee1, HttpPostedFileBase image1)// Esta función permite registrar un empleado
        {
            bool registered1;
          
                using (SqlConnection conn = new SqlConnection(db_connection1))//Creamos una nueva conexión a la base de datos
                {
                    byte[] imageData = null;

                  
                    if (image1 != null && image1.ContentLength > 0)//si existe una imagen, entonces: 
                    {
                      
                        using (var binaryReader = new BinaryReader(image1.InputStream))//se crea un nuevo objeto de tipo BinaryReader
                        {
                            imageData = binaryReader.ReadBytes(image1.ContentLength);// se pasa a imageData el contenido de la imagen en binario
                        }
                    }
                      
                    try
                    {

                        SqlCommand scomm = new SqlCommand("sp_employee_register", conn);// se crea el comando el cual utiliza un procedimiento almacenado 
                        scomm.Parameters.AddWithValue("employee_id", employee1.employee_id);//se agregan los valores ingresados en la vista y se pasa al objeto employee1|
                        scomm.Parameters.AddWithValue("e_name", employee1.e_name);
                        scomm.Parameters.AddWithValue("e_admission_date", employee1.e_admission_date);
                        scomm.Parameters.AddWithValue("e_active", employee1.e_active);
                        scomm.Parameters.AddWithValue("e_supervisor", employee1.e_supervisor);
                        scomm.Parameters.AddWithValue("e_observation", employee1.e_observation);
                        scomm.Parameters.AddWithValue("e_certificate", employee1.e_certificate);
                        scomm.Parameters.AddWithValue("e_image", imageData);
                        scomm.Parameters.Add("registered1", SqlDbType.Bit).Direction = ParameterDirection.Output;
                        scomm.CommandType = CommandType.StoredProcedure;

                        conn.Open();//se abre la conexión
                        scomm.ExecuteNonQuery();//se ejecuta
                        registered1 = Convert.ToBoolean(scomm.Parameters["registered1"].Value);//en registered1 se guarda un valor booleano


                    }
                    catch (SqlException)
                    {
                        ViewData["alertMessageRegister"] = "No se pueden insertar supervisores inexistentes";
                        return View();

                    }
                    catch (DbUpdateException)
                    {
                        ViewData["alertMessageRegister"] = "No se pueden insertar supervisores inexistentes";
                        return View();

                    }

                }

                if (registered1)// Si el empleado es registrado con exito se regresa a la vista principal donde se encuentra la tabla con empleados
                {
                    return RedirectToAction("Employee_view", "Principal");
                }
                else
                {
                    ViewData["alertMessageRegister"] = "Este empleado ya existe";
                  
                }
                return View();
            
        }

       

        public ActionResult Employee_register()
        {
            return View();
        }



        [HttpGet]
        public ActionResult Employee_edit(int Id, HttpPostedFileBase image1)//Esta función permite editar la información de un empleado en especifico tomando como parametro la llave primara (id)
        {                                        

            employeeO employee1 = new employeeO();
            byte[] imageData = null;


            if (image1 != null && image1.ContentLength > 0)
            {

                using (var binaryReader = new BinaryReader(image1.InputStream))
                {
                    imageData = binaryReader.ReadBytes(image1.ContentLength);
                }
            }
            using (CompanyEntities6 data_employee = new CompanyEntities6())
            {
                var table = data_employee.employee.Find(Id);
                employee1.e_image = table.e_image;
                employee1.employee_id = table.employee_id;
                employee1.e_name = table.e_name;
                employee1.e_admission_date = table.e_admission_date;
                employee1.e_supervisor = table.e_supervisor;
                employee1.e_observation = table.e_observation;
                employee1.e_certificate = table.e_certificate;
                employee1.e_active = table.e_active;
                
             

            }

            return View(employee1);// la vista nos devuelve los campos rellenados con la información anterior

        }

        [HttpPost]
        public ActionResult Employee_edit(employeeO data_employee, HttpPostedFileBase image1)// Esta función toma los datos ingresados y los actualiza en caso de no haber errores
        {
        
           
            try
            {
                if (ModelState.IsValid)
                {
                    byte[] imageData = null;


                    if (image1 != null && image1.ContentLength > 0)
                    {

                        using (var binaryReader = new BinaryReader(image1.InputStream))
                        {
                            imageData = binaryReader.ReadBytes(image1.ContentLength);
                        }
                    }

                    using (CompanyEntities6 database= new CompanyEntities6())//creamos una entidad de la base de datos de la compañia
                    {
                        var employee1 = database.employee.Find(data_employee.employee_id);//se busca el id del empleado especificado
                        if (image1 != null)
                        {
                            employee1.e_image = imageData;
                        }
                        //A continuación los nuevos datps ingresados en la vista se pasan a los entes de la base de datos
                        employee1.employee_id = data_employee.employee_id;
                        employee1.e_name = data_employee.e_name;
                        employee1.e_admission_date = (DateTime)data_employee.e_admission_date;
                        employee1.e_supervisor = data_employee.e_supervisor;
                        employee1.e_observation = data_employee.e_observation;
                        employee1.e_certificate = data_employee.e_certificate;
                        employee1.e_active = data_employee.e_active;
                        
                       
                        database.Entry(employee1).State = System.Data.Entity.EntityState.Modified;// se modifica la base de datos
                        database.SaveChanges();//se guardan los datos

                    }
                    return Redirect("~/Principal/Employee_view");
             
                }
                

            }
            catch (Exception)
            {


            }

            return Redirect("~/Principal/Employee_view");
        }

        public ActionResult Employee_delete_view(int id) //esta función nos muestra los datos del empleados a eliminar
        {
            employeeO employee1 = new employeeO();

            using (CompanyEntities6 data_employee = new CompanyEntities6())//se crea un modelo del empleado según el id
            {
                var table = data_employee.employee.Find(id);
                employee1.employee_id = table.employee_id;
                employee1.e_name = table.e_name;
                employee1.e_admission_date = table.e_admission_date;
                employee1.e_supervisor = table.e_supervisor;
                employee1.e_observation = table.e_observation;
                employee1.e_certificate = table.e_certificate;
                employee1.e_active = table.e_active;
            }

            return View(employee1);// nos regresa la vista con el empleado especificadoa a eliminar

           
        }

        [HttpGet]
        public ActionResult Employee_delete(int id)//esta función nos permite eliminar un empleado
        {
            using (CompanyEntities6 delete_employee = new CompanyEntities6())//se crea una entidad de la compañia
            {
                employee actionDelete = delete_employee.employee.Find(id);//y se pasa el id del empleado a borrar
                delete_employee.employee.Remove(actionDelete);//se realiza la eliminación del empleado
                delete_employee.SaveChanges();//se guardan los datos en la base de datos

            }


            return Redirect("~/Principal/Employee_view");//no regresa a la vista principal de empleados

        }

        public ActionResult Employee_profile_view(int id)//Esta función nos muestra el perfil del empleado seleccionado 
        {



            employeeO employee1 = new employeeO();

            using (CompanyEntities6 data_employee = new CompanyEntities6())//Se crea una entidad de la compañoia
            {
                var table = data_employee.employee.Find(id);//se especifica el id del empleado para su busqueda
                //se pasa la información del empleado especifico al modelo
                employee1.employee_id = table.employee_id;
                employee1.e_name = table.e_name;
                employee1.e_admission_date = table.e_admission_date;
                employee1.e_supervisor = table.e_supervisor;
                employee1.e_observation = table.e_observation;
                employee1.e_certificate = table.e_certificate;
                employee1.e_active = table.e_active;
            }

            return View(employee1);//nos devuelve la vista del empleado
        }

        public ActionResult GetImage(int id)//esta función nos devuelve la imagen ingresada y nos la muestra como foto de perfil
        {
            using (CompanyEntities6 data_employee = new CompanyEntities6())
            {
                var employee = data_employee.employee.Find(id);//se especifica la imágen del empleado definido por su id
                if (employee != null && employee.e_image != null)// si existe la imágen y el empleado:
                {
                    return new FileContentResult(employee.e_image, "image/jpeg");//Nos devuelve la imagen y la podemos mostrar en la vista
                }
            }

            string defaultImagePath = Server.MapPath("~/Content/Images/default-avatarF.jpg");//Imagen por default en caso de no haber imágen previa
            byte[] defaultImageBytes = System.IO.File.ReadAllBytes(defaultImagePath);
            return new FileContentResult(defaultImageBytes, "image/jpeg");
        }
        //DE AQUI EN ADELANTE LAS FUNCIONES SON EXACTAMENTE LAS MISMA SOLO ADAPTADAS A LOS DISTINTOS OBJETOS (SUPERVISOR, CLIENTE, SERVICIO)
        //////Seccion supervisores//////

        public ActionResult Supervisors_view()
        {
            if (ModelState.IsValid)
            {
                List<supervisorO> select_object1;
                using (CompanyEntities6 data_supervisor = new CompanyEntities6())
                {

                    select_object1 = (from supervisor1 in data_supervisor.supervisor
                                      select new supervisorO
                                      {
                                          supervisor_id = supervisor1.supervisor_id,
                                          s_name = supervisor1.s_name,
                                          s_area = supervisor1.s_area,
                                          s_active = supervisor1.s_active

                                      }).ToList();

                }

                return View(select_object1);
            }
            return View();
        }

        [HttpPost]
        public ActionResult Supervisors_Register(supervisor supervisor1)
        {
            bool registered2;

            using (SqlConnection conn = new SqlConnection(db_connection1))
            {
                try
                {


                    SqlCommand scomm = new SqlCommand("sp_supervisor_register", conn);
                    scomm.Parameters.AddWithValue("supervisor_id", supervisor1.supervisor_id);
                    scomm.Parameters.AddWithValue("s_name", supervisor1.s_name);
                    scomm.Parameters.AddWithValue("s_area", supervisor1.s_area);
                    scomm.Parameters.AddWithValue("s_active", supervisor1.s_active);
                    scomm.Parameters.Add("registered2", SqlDbType.Bit).Direction = ParameterDirection.Output;
                    scomm.CommandType = System.Data.CommandType.StoredProcedure;

                    conn.Open();
                    scomm.ExecuteNonQuery();
                    registered2 = Convert.ToBoolean(scomm.Parameters["registered2"].Value);





                }
                catch (SqlException)
                {
                    ViewData["alertMessageRegister"] = "No se pueden insertar supervisores existentes";
                    return View();

                }

             }
            if (registered2)
            {
                    return RedirectToAction("Supervisors_view", "Principal");

            }
            else
            {
                ViewData["alertMessageRegister"] = "No se pueden insertar supervisores existentes";
            }
               
            return View();

         }

        public ActionResult Supervisors_Register()
        {

            return View();
        }



        [HttpGet]
        public ActionResult Supervisor_edit(int Id)
        {
            supervisorO supervisor1 = new supervisorO();

            using (CompanyEntities6 data_supervisor = new CompanyEntities6())
            {
                var table = data_supervisor.supervisor.Find(Id);
                supervisor1.supervisor_id = table.supervisor_id;
                supervisor1.s_name = table.s_name;
                supervisor1.s_area = table.s_area;
                supervisor1.s_active = table.s_active;


            }

            return View(supervisor1);

        }

        [HttpPost]
        public ActionResult Supervisor_edit(supervisorO data_supervisor)
        {


            try
            {
                if (ModelState.IsValid)
                {
                    using (CompanyEntities6 database = new CompanyEntities6())
                    {
                        var supervisor1 = database.supervisor.Find(data_supervisor.supervisor_id);
                        supervisor1.supervisor_id = data_supervisor.supervisor_id;
                        supervisor1.s_name = data_supervisor.s_name;
                        supervisor1.s_area = data_supervisor.s_area;
                        supervisor1.s_active = data_supervisor.s_active;
                        database.Entry(supervisor1).State = System.Data.Entity.EntityState.Modified;
                        database.SaveChanges();

                    }
                    return Redirect("~/Principal/Supervisors_view");

                }


            }
            catch (Exception)
            {


            }

            return Redirect("~/Principal/Supervisors_view");
        }
      
        public ActionResult Supervisor_delete_view(int id)
        {
            supervisorO supervisor1 = new supervisorO();

            using (CompanyEntities6 data_supervisor = new CompanyEntities6())
            {
                var table = data_supervisor.supervisor.Find(id);
                supervisor1.supervisor_id = table.supervisor_id;
                supervisor1.s_name = table.s_name;
                supervisor1.s_area = table.s_area;
                supervisor1.s_active = table.s_active;


            }

            return View(supervisor1);
         
        }

        [HttpGet]
        public ActionResult Supervisor_delete(int id)
        {
            using (CompanyEntities6 delete_supervisor = new CompanyEntities6())
            {
                supervisor actionDelete = delete_supervisor.supervisor.Find(id);
                delete_supervisor.supervisor.Remove(actionDelete);
                delete_supervisor.SaveChanges();

            }


            return Redirect("~/Principal/Supervisors_view");

        }

        ////Seccion Servicios////
        public ActionResult Service_view()
        {

            
            List<service12> select_service;
            using (CompanyEntities6 data_service = new CompanyEntities6())
            {

                select_service = (from service1 in data_service.service1
                                  select new service12
                                  {
                                     service_id=service1.service_id,
                                     service_date=service1.service_date,
                                     employee_attend= service1.employee_attend,
                                     client_attended= service1.client_attended,
                                     activity=service1.activity


                                  }).ToList();

            }

            return View(select_service);
           
        }

        [HttpPost]
        public ActionResult Service_register(service1 service1)
        {
            
          
            bool registered3;


            using (SqlConnection conn = new SqlConnection(db_connection1))
            {
                try
                {
                    SqlCommand scomm = new SqlCommand("sp_service_register", conn);
                    scomm.Parameters.AddWithValue("service_id", service1.service_id);
                    scomm.Parameters.AddWithValue("service_date", service1.service_date);
                    scomm.Parameters.AddWithValue("employee_attend", service1.employee_attend);
                    scomm.Parameters.AddWithValue("client_attended", service1.client_attended);
                    scomm.Parameters.AddWithValue("activity", service1.activity);
                    scomm.Parameters.Add("registered3", SqlDbType.Bit).Direction = ParameterDirection.Output;
                    scomm.CommandType = System.Data.CommandType.StoredProcedure;


                    conn.Open();
                    scomm.ExecuteNonQuery();

                    registered3 = Convert.ToBoolean(scomm.Parameters["registered3"].Value);


                }
                catch (SqlException)
                {
                    ViewData["alertMessageRegister"] = "No se pueden insertar empleados ni clientes inexistentes";
                    return View();
                }
                catch (DbUpdateException)
                {
                    ViewData["alertMessageRegister"] = "No se pueden insertar empleados ni clientes inexistentes";
                    return View();

                }

              
            }

            if (registered3)
            {
                return RedirectToAction("Service_view", "Principal");

            }
            else
            {
                return View();
            }
        }

        public ActionResult Service_register()
        {
            List<employee> employees = new List<employee>();
            ViewData["Employee"] = employees;

            return View();
        }

        public ActionResult Service_edit()
        {

            return View();
        }



        [HttpGet]
        public ActionResult Service_edit(int Id)
        {
          
            service12 service1 = new service12();

            using (CompanyEntities6 data_service = new CompanyEntities6())
            {
                var table = data_service.service1.Find(Id);
                service1.service_id = table.service_id;
                service1.service_date = table.service_date;
                service1.client_attended = table.client_attended;
                service1.employee_attend = table.employee_attend;
                service1.activity = table.activity;
            }

            return View(service1);

        }

        [HttpPost]
        public ActionResult Service_edit(service12 data_service)
        {
            


            if (ModelState.IsValid)
            {
                
               using (CompanyEntities6 database = new CompanyEntities6())
               {
                    try
                    {

                        var service2 = database.service1.Find(data_service.service_id);
                        service2.service_id = data_service.service_id;
                        service2.service_date = data_service.service_date;
                        service2.employee_attend = data_service.employee_attend;
                        service2.client_attended = data_service.client_attended;
                        service2.activity = data_service.activity;
                        database.Entry(service2).State = System.Data.Entity.EntityState.Modified;
                        database.SaveChanges();

                        return Redirect("~/Principal/Service_view");
                    }
                    catch (System.NullReferenceException)
                    {
                        TempData["ErrorMessage"] = "Cliente o empleado inexistentes";
                        return RedirectToAction("Service_edit", "Principal");

                    }
                    catch (SqlException)
                    {
                        TempData["ErrorMessage"] = "Error al ingresar los datos";
                        return RedirectToAction("Service_edit", "Principal");
                    }
                    catch (DbUpdateException)
                    {
                        TempData["ErrorMessage"] = "Cliente o empleado inexistentes";
                        return RedirectToAction("Service_edit", "Principal");
                        
                    }
                   

                }

               

             }

             return View();

        }

        public ActionResult Service_delete_view(int id)
        {
            service12 service1 = new service12();

            using (CompanyEntities6 data_service = new CompanyEntities6())
            {
                var table = data_service.service1.Find(id);
                service1.service_id = table.service_id;
                service1.service_date = table.service_date;
                service1.client_attended = table.client_attended;
                service1.employee_attend = table.employee_attend;
                service1.activity = table.activity;


            }

            return View(service1);

        }

        [HttpGet]
        public ActionResult Service_delete(int id)
        {
            using (CompanyEntities6 delete_service = new CompanyEntities6())
            {
                
                    service1 actionDelete = delete_service.service1.Find(id);
                    delete_service.service1.Remove(actionDelete);
                    delete_service.SaveChanges();
            }
            return Redirect("~/Principal/Service_view");

        }

        ////Seccion Clientes////
        public ActionResult Client_view()
        {
            List<clientO> select_client;
            using (CompanyEntities6 data_client = new CompanyEntities6())
            {

                select_client = (from client1 in data_client.client
                                  select new clientO
                                  {
                                      client_id = client1.client_id,
                                     client_name = client1.client_name,
                                      client_phone=client1.client_phone,
         

                                  }).ToList();

            }

            return View(select_client);

            
        }

        [HttpPost]
        public ActionResult Client_register(client client1)
        {
            bool registered4;

           
                using (SqlConnection conn = new SqlConnection(db_connection1))
                {

                    SqlCommand scomm = new SqlCommand("sp_client_register", conn);
                    scomm.Parameters.AddWithValue("client_id", client1.client_id);
                    scomm.Parameters.AddWithValue("client_name", client1.client_name);
                    scomm.Parameters.AddWithValue("client_phone", client1.client_phone);
                    
                    scomm.Parameters.Add("registered4", SqlDbType.Bit).Direction = ParameterDirection.Output;
                    scomm.CommandType = System.Data.CommandType.StoredProcedure;

                    conn.Open();
                    scomm.ExecuteNonQuery();

                    registered4 = Convert.ToBoolean(scomm.Parameters["registered4"].Value);

                }



            if (registered4)
            {
                return RedirectToAction("Client_view", "Principal");

            }
            else
            {
                return View();
            }

            
        }

        public ActionResult Client_register()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Client_edit(int Id)
        {

            clientO client1 = new clientO();

            using (CompanyEntities6 data_client = new CompanyEntities6())
            {
                var table = data_client.client.Find(Id);
                client1.client_id = table.client_id;
                client1.client_name = table.client_name;
                client1.client_phone = table.client_phone;
            }

            return View(client1);

        }

        [HttpPost]
        public ActionResult Client_edit(clientO data_client)
        {



            if (ModelState.IsValid)
            {
                using (CompanyEntities6 database = new CompanyEntities6())
                {
                    try
                    {
                        var client1 = database.client.Find(data_client.client_id);
                        client1.client_id = data_client.client_id;
                        client1.client_name = data_client.client_name;
                        client1.client_phone = data_client.client_phone;
                        database.Entry(client1).State = System.Data.Entity.EntityState.Modified;
                        database.SaveChanges();

                        return Redirect("~/Principal/Client_view");




                    }
                    catch (SqlException)
                    {
                        ViewData["alertMessageRegister"] = "Ocurrio un error al ingresar los datos";
                        return View();
                    }
                    catch (DbUpdateException)
                    {
                        ViewData["alertMessageRegister"] = "Ocurrio un error al ingresar los datos";
                        return View();
                    }

                }



            }

            return View();

        }

        public ActionResult Client_delete_view(int id)
        {
           clientO client1 = new clientO();

            using (CompanyEntities6 data_client = new CompanyEntities6())
            {
                var table = data_client.client.Find(id);
                client1.client_id = table.client_id;
                client1.client_name = table.client_name;
                client1.client_phone = table.client_phone;


            }

            return View(client1);

        }

        [HttpGet]
        public ActionResult Client_delete(int id)
        {
            using (CompanyEntities6 delete_client = new CompanyEntities6())
            {

                client actionDelete = delete_client.client.Find(id);
                delete_client.client.Remove(actionDelete);
                delete_client.SaveChanges();
            }


            return Redirect("~/Principal/Client_view");

        }
        ////Seccion Busquedas////
        [HttpGet]
        public ActionResult Employee_search(string searchQuery)//Función de buscador, nos devuelve la lista con los empleados que coinciden en nombre o apellido
        {
            // Realiza la busqueda conforme el query
            var searchResults = dbo.employee.Where(e => e.e_name.Contains(searchQuery)).ToList();

           //crea el modelo segun el query
            var searchResultsO = searchResults.Select(e => new employeeO
            {
                employee_id = e.employee_id,
                e_name = e.e_name,
                e_admission_date = e.e_admission_date,
                e_supervisor = e.e_supervisor,
                e_certificate = e.e_certificate,
                e_observation = e.e_observation,
                e_active = e.e_active
            }).ToList();

            // pasa el resultado de la busqueda a la vista
            return View("Employee_search", searchResultsO);
        }
        [HttpGet]
        public ActionResult Supervisor_search(string searchQuery)
        {
           
            var searchResults = dbo.supervisor.Where(e => e.s_name.Contains(searchQuery)).ToList();

           
            var searchResultsO = searchResults.Select(e => new supervisorO
            {
                supervisor_id = e.supervisor_id,
                s_name = e.s_name,
                s_active = e.s_active,
                s_area=e.s_area
            }).ToList();

           
            return View("Supervisor_search", searchResultsO);
        }

        [HttpGet]
        public ActionResult Activities_search(string searchQuery)
        {
           
            var searchResults = dbo.service1.Where(e => e.activity.Contains(searchQuery)).ToList();

            
            var searchResultsO = searchResults.Select(e => new service12
            {
                service_id=e.service_id,
                service_date=e.service_date,
                employee_attend=e.employee_attend,
                client_attended=e.client_attended,
                activity=e.activity,
            }).ToList();

           
            return View("Activities_search", searchResultsO);
        }

        [HttpGet]
        public ActionResult Client_search(string searchQuery)
        {
           
            var searchResults = dbo.client.Where(e => e.client_name.Contains(searchQuery)).ToList();

           
            var searchResultsO = searchResults.Select(e => new clientO
            {
               client_id=e.client_id,
               client_name=e.client_name,
               client_phone=e.client_phone,

            }).ToList();

            
            return View("Client_search", searchResultsO);
        }


    }



}



    


     