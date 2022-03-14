using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NewsReader.Models;
using System.Data.Linq;
using System.Xml;
using System.ServiceModel.Syndication;
using System.Text.RegularExpressions;

namespace NewsReader
{
    class Program
    {


        // код на коленке поэтому просьба строго не судить

       


        //////////////////////////////// PROGRAMM ////////////////////////////////

        // Создаем БД                                                           DB_NEWS                       SQLQuery_CreateDB.sql
        // Создаем таблицу                                                      News                          SQLQuery_CreateTableNews.sql
        // Скрипты будут вложены в отдельной папке SQL_Scripts

        // Инициализируем  модель таблицы News в папке Models                   News


        // Инициализируем строку подключения к БД
        // при необхзодимости пароль можно затребовать от пользователя и передать через переменную 



        static string connectionString = "";



        static void Main(string[] args)
        {

            //реализуем функцию авторизации для доступа к БД
            Console.WriteLine($"Введите имя сервера:");
            string serverName = Console.ReadLine();


            Console.WriteLine($"Введите Логин SQL-пользователя:");
            string User_ID = Console.ReadLine();



            EnterPassword:
            Console.WriteLine($"Введите пароль доступа к БД:");    // выводим сообщение на консоль
            string password = "";

            while (true)
            {
                var key = Console.ReadKey(true);                   // скрываем вводимые символы

                if (key.Key == ConsoleKey.Enter) break;            // при нажатии enter - выходим из цикла

                if (key.Key != ConsoleKey.Backspace)
                {
                    Console.Write("*");                                // вместо вводимых символов выводим *
                    password += key.KeyChar;
                }
                else
                {
                    Console.WriteLine("");
                    password = "";
                    goto EnterPassword;
                }
            }

            Console.WriteLine("");


            //строка подключения к бд
            connectionString = $"Data Source={serverName};Initial Catalog=DB_NEWS;user id={User_ID};password={password};Trusted_Connection=False;MultipleActiveResultSets=true;";

            DataContext db = new DataContext(connectionString);

            Table<News> news = db.GetTable<News>();



            string[] SoursesUrl = { "", "" };

            SoursesUrl[0] = "https://habr.com/ru/rss/all/all/";                     //Хабрхабр RSS Url
            SoursesUrl[1] = "https://www.ixbt.com/export/news.rss";                 //Интерфакс RSS Url

            int[] cntrsNewsReaded = { 0, 0 };                                       //количество прочитанных новостей
            int[] cntrsNewsSaved = { 0, 0 };                                        //количество сохраненных новостей

            // тут и так все понятно 
            for (var n = 0; n < 2; n++)
            {


                // проверочка на отсутствие неожиданных поворотов сюжета     
                // инициализируем новую строку URL через класс Uri 

                Uri sourseUrl = null;

                try
                {

                    // проверяем соответствие введенной строки Url-адресу
                    sourseUrl = new Uri(SoursesUrl[n]);

                    // специальные параметры: игрорирование неважных пробелов, комментариев, ограничение в количество символов в документе - 0 (не ограничено)
                    XmlReaderSettings settings = new XmlReaderSettings();
                    settings.IgnoreComments = true;
                    settings.IgnoreProcessingInstructions = true;
                    settings.IgnoreWhitespace = true;
                    settings.MaxCharactersInDocument = 0;

                    // создам экзепляр для XmlReader чтения  данных                   
                    XmlReader readFromURL = XmlReader.Create(SoursesUrl[n], settings);

                    // выгружаем новости
                    SyndicationFeed newsReaded = SyndicationFeed.Load(readFromURL);


                    // проверяем удалось ли что-то выгрузить   
                    if (newsReaded != null)
                    {
                        cntrsNewsReaded[n] = newsReaded.Items.Count();
                        // перебор каждого элемента в веб-канале
                        foreach (SyndicationItem item in newsReaded.Items)
                        {
                            // увеличиваем счетчик прочитанных новостей

                            // проверяем нет ли в БД новости с таким же GUID
                            var newsFromDB = news.Where(w => w.Guid == item.Id);
                      

                            if (newsFromDB.Count() == 0)
                            {
                                try
                                {
                                    // создаем элемент News-таблицы аналогичной таблице в БД
                                    News newsDB = new News();

                                    newsDB.ID = +1;
                                    newsDB.Guid = item.Id;                                                                 // содержет GUID-идентификатор 
                                    if (newsReaded.Generator == "habr.com")
                                    {
                                        newsDB.SourceName = newsReaded.Generator + ": " + newsReaded.Title.Text;           // источник
                                    }
                                    else
                                    {
                                        newsDB.SourceName = newsReaded.Title.Text;
                                    }
                                    newsDB.SourseUrl = item.Links.Select(s => s.Uri).FirstOrDefault().AbsoluteUri;         // URL адрес источника новости
                                    newsDB.Publication_Date = item.PublishDate.DateTime;                                   // дата публикации
                                    newsDB.News_title = item.Title.Text;                                                   // название новости
                                    var newsDescription = item.Summary.Text;
                                    newsDB.News_description = Regex.Replace(newsDescription, "<[^>]+>", string.Empty);     // описание новости

                                    //записываем данные в БД                             
                                    news.InsertOnSubmit(newsDB);
                                    db.SubmitChanges();
                                    //увеличивdb.SubmitChanges();аем счетчик сохраненных новостей
                                    cntrsNewsSaved[n] = cntrsNewsSaved[n] + 1;
                                }
                                catch
                                {
                                    Console.WriteLine($"Ошибка: Не удалось сохранить данные в БД!");    // выводим сообщение на консоль
                                }
                            }



                        }

                    }


                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка: {ex}");    // выводим сообщение на консоль
                }


            }
            Console.WriteLine($"Хабрхабр");    // выводим сообщение на консоль
            Console.WriteLine($"Прочитано новостей: {cntrsNewsReaded[0]}");    // выводим сообщение на консоль
            Console.WriteLine($"Сохранено новостей: {cntrsNewsSaved[0]}");    // выводим сообщение на консоль
            Console.WriteLine("");
            Console.WriteLine($"Интерфакс");    // выводим сообщение на консоль
            Console.WriteLine($"Прочитано новостей: {cntrsNewsReaded[1]}");    // выводим сообщение на консоль
            Console.WriteLine($"Сохранено новостей: {cntrsNewsSaved[1]}");    // выводим сообщение на консоль



            Console.ReadKey();


        }
    }
}
