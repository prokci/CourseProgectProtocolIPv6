using CourseProgectProtocolIPv6.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

using System;
using System.Configuration;

using CourseProgectProtocolIPv6.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using System.Configuration;

using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;

using System;
using Microsoft.Data.SqlClient;
using System.Data;

namespace CourseProgectProtocolIPv6.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        static string connectionString = WebApplication.CreateBuilder().Configuration.GetConnectionString("UserConnection");
        private List<string[]> Oll;



        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
            Oll = Get("News");//Загрузка данных с базы "News" и добавление в список
            Oll.AddRange(Get("Histore")); //Загрузка данных с базы "Histore" и добавление в список
            Oll.AddRange(Get("Theory")); //Загрузка данных с базы "Theory" и добавление в список
            Oll.AddRange(Get("Examplys"));//Загрузка данных с базы "Examplys" и добавление в список
        }

        public IActionResult Index()
        {
            ViewBag.Oll = Oll;//передача списка для показа его в предстовлении
            return View();//запуск представления
        }

        public IActionResult News()
        {
            ViewBag.News = Get("News");
            return View();
        }
        public IActionResult Histore()
        {
            ViewBag.Table = Get("Histore");
            return View();
        }
        
        public IActionResult Theory()
        {
            ViewBag.Table = Get("Theory");
            return View();
        }

        public IActionResult Examplys()
        {
            ViewBag.Table = Get("Examplys");
            return View();
        }

        public IActionResult Sources()
        {
            ViewBag.Table = Get("Sources");
            return View();
        }

        
        [Authorize]//проверка на авторизацию при отсуствии таковой преброс на страницу входа
        public IActionResult Entrance()
        {
            ViewBag.Table = GetUsers();//загрузка пользователей
            ViewBag.News = GetNews();//загрузка новостей

            ViewBag.RoleId = GetRoleId(ViewBag.Table, User.Identity.Name);
            //определение роли пользователя
            ViewBag.IdAuthor = GetId(ViewBag.Table, User.Identity.Name);
            //определение ID пользователя

            List<int> index;

            if (ViewBag.RoleId == 2)//только для роли "users"
            {
                index = new List<int>();//иницализация списка 

                for (int i = 0; i < ViewBag.News.Count; i++)
                    //создание списка последовательных чисел 
                {
                    index.Add(i);
                }

                
                int IaO = 0;
                for (int i = 0; i < index.Count; i++)
                    //удоление из списка не соответсвуюших определёному пользователю
                    //индексов

                {
                    if (ViewBag.News[i + IaO][3] != ViewBag.IdAuthor)
                    {
                        index.RemoveAt(i);
                        i--;
                        IaO++;
                    }
                }
                ViewBag.Index = index;//передача списка в предстовление
            }

            return View("Entrance");//запуск представления


        }

        private int GetId(List<List<object>> Base, string name)
        {
            for (int i = 0; i < Base.Count; i++)
            {
                if (Base[i][1].ToString() == name)
                {
                    return int.Parse(Base[i][0].ToString());
                }
            }
            return -1;
        }

        [HttpGet]//тип предстовления "get"
        public IActionResult NewsOpen(int Id)
        {
            ViewBag.Id = Id;//передача ID
            ViewBag.Oll = Oll;//перредача списка 
            return View();//запуск представления
        }
        [HttpPost]//тип предстовления "post"
        public IActionResult NewsEdit(int id, bool isEditOrNew,string idAuthor)
        {
            ViewBag.Id = id;//предача ID
            ViewBag.IdAuthor = int.Parse(idAuthor);//рпередача ID автора
            ViewBag.News = Get("News");//загрузка новостей
            if (!isEditOrNew)//только при создании новости
            {
                AddNews(" ", " ", int.Parse(idAuthor));//добовление в базу данных
                ViewBag.News.Add(new string[] { " ", " " });//добовление в список нововстей
                Oll.Insert(id, new string[] { " ", " " });//добовление в обший список
            }
            ViewBag.IdBase = GetNews()[id][0];//предедача ID новости
            return View();//запуск представления
        }
        [HttpGet]//тип предстовления "get"
        public IActionResult NewsSave(int id,string name,string value)
        {
            SetNews(id, name, value);//сохранение нвости в базе
            return Entrance();//перход к представлению личного кабинета
        }

        public IActionResult NewsDelete(int id)
        {
            DeleteNews(id);//удаление новости
            return Entrance();//перход к представлению личного кабинета
        }

        public IActionResult SetRoleId(int id,string roleId)
        {
            SetRole(id,int.Parse(roleId));//смена роли пользователя
            return Entrance();//перход к представлению личного кабинета
        }

        [HttpPost]//тип предстовления "post"
        public IActionResult Find(string Find)
        {
            ViewBag.Oll = Oll;//пердача предстовления
            List<int> index = new List<int>();
            //обявление и иницализация списка индексов
            for (int i = 0; i < Oll.Count; i++)
            //создание списка последовательных чисел 
            {
                index.Add(i);
            }
            List<int> index2 = new List<int>(index);
            int IaO = 0;
            for (int i = 0; i < index.Count; i++)
            //удаление номеров записей в которых нет искомой стороки
            //в названии
            {
                if (Oll[i + IaO][0].IndexOf(Find, StringComparison.OrdinalIgnoreCase) == -1)
                {
                    index.RemoveAt(i);
                    i--;
                    IaO++;
                }
            }
            IaO = 0;
            for (int i = 0; i < index2.Count; i++)
            //удаление номеров записей в которых нет искомой стороки
            //в содержании
            {
                if (Oll[i + IaO][1].IndexOf(Find, StringComparison.OrdinalIgnoreCase) == -1)
                {
                    index2.RemoveAt(i);
                    i--;
                    IaO++;
                }
            }
            ViewBag.Index = index;//передач списка идексов с найдеными названиями
            ViewBag.IndexV = index2;//передач списка идексов с найдеными содержаниями
            ViewBag.Find = Find;//передача искомой строки
            return View();//запуск представления
        }


        private static object SetNews(int id, string name, string value)
        {
            // название процедуры
            string sqlExpression = "sp_SetNews4";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(sqlExpression, connection);
                // указываем, что команда представляет хранимую процедуру
                command.CommandType = System.Data.CommandType.StoredProcedure;
                // параметр для ввода имени
                SqlParameter idParam = new SqlParameter
                {
                    ParameterName = "@id",
                    Value = id
                };
                // добавляем параметр
                command.Parameters.Add(idParam);
                // параметр для ввода
                SqlParameter nameParam = new SqlParameter
                {
                    ParameterName = "@name",
                    Value = name
                };
                command.Parameters.Add(nameParam);

                SqlParameter valueParam = new SqlParameter
                {
                    ParameterName = "@value",
                    Value = value
                };
                command.Parameters.Add(valueParam);

                var result = command.ExecuteScalar();
                // если нам не надо возвращать id
                //var result = command.ExecuteNonQuery();

                return result;
            }
        }
        private static object DeleteNews(int id)
        {
            // название процедуры
            string sqlExpression = "sp_DeleteNews2";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(sqlExpression, connection);
                // указываем, что команда представляет хранимую процедуру
                command.CommandType = System.Data.CommandType.StoredProcedure;
                // параметр для ввода имени
                SqlParameter idParam = new SqlParameter
                {
                    ParameterName = "@id",
                    Value = id
                };
                // добавляем параметр
                command.Parameters.Add(idParam);
               

                var result = command.ExecuteScalar();
                // если нам не надо возвращать id
                //var result = command.ExecuteNonQuery();

                return result;
            }
        }

        

        private static int GetRoleId(List<List<object>> Base,string lt)
        {
            for(int i = 0; i < Base.Count; i++)
            {
                if(Base[i][1].ToString() == lt)
                //поиск подходяшего юзера из списка
                {
                    return int.Parse(Base[i][3].ToString());
                }
            }
            return -1;
            
        }


        private static object AddNews(string name, string value,int idAuthor)
        {
            // название процедуры
            string sqlExpression = "sp_AddNews";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(sqlExpression, connection);
                // указываем, что команда представляет хранимую процедуру
                command.CommandType = System.Data.CommandType.StoredProcedure;
                // параметр для ввода имени
                SqlParameter nameParam = new SqlParameter
                {
                    ParameterName = "@name",
                    Value = name
                };
                // добавляем параметр
                command.Parameters.Add(nameParam);
                // параметр для ввода
                SqlParameter valueParam = new SqlParameter
                {
                    ParameterName = "@value",
                    Value = value
                };
                command.Parameters.Add(valueParam);

                SqlParameter idAuthorParam = new SqlParameter
                {
                    ParameterName = "@idAuthor",
                    Value = idAuthor
                };
                command.Parameters.Add(idAuthorParam);

                var result = command.ExecuteScalar();
                // если нам не надо возвращать id
                //var result = command.ExecuteNonQuery();

                return result;
            }
        }


        private static object SetRole(int id, int roleId)
        {// название процедуры
            string sqlExpression = "sp_SetRoleId";

            using (SqlConnection connection = new SqlConnection(WebApplication.CreateBuilder().Configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(sqlExpression, connection);
                // указываем, что команда представляет хранимую процедуру
                command.CommandType = System.Data.CommandType.StoredProcedure;
                // параметр для ввода имени
                SqlParameter idParam = new SqlParameter
                {
                    ParameterName = "@id",
                    Value = id
                };
                // добавляем параметр
                command.Parameters.Add(idParam);
                // параметр для ввода
                SqlParameter roleIdParam = new SqlParameter
                {
                    ParameterName = "@roleId",
                    Value = roleId
                };
                command.Parameters.Add(roleIdParam);

                var result = command.ExecuteScalar();
                // если нам не надо возвращать id
                //var result = command.ExecuteNonQuery();

                return result;
            }
        }
        private static List<string[]> Get(string str)
        {
            // название процедуры
            string sqlExpression = "sp_Get"+str;
            var Base = new List<string[]>();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(sqlExpression, connection);
                // указываем, что команда представляет хранимую процедуру
                command.CommandType = System.Data.CommandType.StoredProcedure;
                var reader = command.ExecuteReader();
                

                if (reader.HasRows)
                {
                    
                    while (reader.Read())
                    {
                        int id = reader.GetInt32(0);
                        string name = reader.GetString(1);
                        string value = reader.GetString(2);
                        Base.Add(new string[] {name,value});
                    }
                }
                reader.Close();
            }
            return Base;
        }
        private static List<object[]> GetNews()
        {
            // название процедуры
            string sqlExpression = "sp_GetNews";
            var Base = new List<object[]>();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(sqlExpression, connection);
                // указываем, что команда представляет хранимую процедуру
                command.CommandType = System.Data.CommandType.StoredProcedure;
                var reader = command.ExecuteReader();


                if (reader.HasRows)
                {

                    while (reader.Read())
                    {
                        int id = reader.GetInt32(0);
                        string name = reader.GetString(1);
                        string value = reader.GetString(2);
                        int UserId = reader.GetInt32(3);
                        Base.Add(new object[] { id,name, value , UserId });
                    }
                }
                reader.Close();
            }
            return Base;
        }
        private static List<List<object>> GetUsers()
        {
            // название процедуры
            string sqlExpression = "sp_GetUsers";
            var Base = new List<List<object>>();
            using (SqlConnection connection = new SqlConnection(
                WebApplication.CreateBuilder().Configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(sqlExpression, connection);
                // указываем, что команда представляет хранимую процедуру
                command.CommandType = System.Data.CommandType.StoredProcedure;
                var reader = command.ExecuteReader();


                if (reader.HasRows)
                {

                    while (reader.Read())
                    {
                        int Id = reader.GetInt32(0);
                        string Email = reader.GetString(1);
                        string Pasword = reader.GetString(2);
                        int RoleId = reader.GetInt32(3);
                        Base.Add(new List<object>() { Id, Email, Pasword, RoleId });
                    }
                }
                reader.Close();
            }
            return Base;
        }




        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}