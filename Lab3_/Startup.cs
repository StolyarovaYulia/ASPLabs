using System;
using System.Collections.Generic;
using System.Text;
using Lab3_.Data;
using Lab3_.Middleware;
using Lab3_.Models;
using Lab3_.Services;
using Lab3_.ViewModels;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Lab3_
{
    public class Startup
    {
        private static readonly string CacheKey = "Tracks 20";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // внедрение зависимости для доступа к БД с использованием EF
            var connection = Configuration.GetConnectionString("SqlServerConnection");
            services.AddDbContext<RadiostationContext>(options => options.UseSqlServer(connection));
            // внедрение зависимости OperationService
            services.AddTransient<ITracksService, TracksService>();
            // добавление кэширования
            services.AddMemoryCache();
            // добавление поддержки сессии
            services.AddDistributedMemoryCache();
            services.AddSession();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();
            else
                app.UseExceptionHandler("/Home/Error");

            // добавляем поддержку статических файлов
            app.UseStaticFiles();

            // добавляем поддержку сессий
            app.UseSession();

            app.UseMiddleware<InfoMiddleware>();

            // добавляем компонент middleware по инициализации базы данных и производим инициализацию базы
            app.UseDbInitializer();

            // добавляем компонент middleware для реализации кэширования и записывем данные в кэш
            app.UseOperatinCache(CacheKey);

            app.MapWhen(ctx => ctx.Request.Path == "/", Index);
            app.MapWhen(ctx => ctx.Request.Path == "/searchform1", CookieSearch);
            app.MapWhen(ctx => ctx.Request.Path == "/search1", CookieSearchHandler);
            app.MapWhen(ctx => ctx.Request.Path == "/searchform2", SessionSearch);
            app.MapWhen(ctx => ctx.Request.Path == "/search2", SessionSearchHandler);

            app.UseRouting();
        }

        private static string AppendTrackTable(string htmlString, IEnumerable<TrackViewModel> tracks)
        {
            var stringBuilder = new StringBuilder(htmlString);

            stringBuilder.Append("<div style=\"display: inline-block;\"><H1>Треки</H1>");
            stringBuilder.Append("<TABLE BORDER=1>");
            stringBuilder.Append("<TH>");
            stringBuilder.Append("<TD>#</TD>");
            stringBuilder.Append("<TD>Наименование</TD>");
            stringBuilder.Append("<TD>Исполнитель</TD>");
            stringBuilder.Append("<TD>Жанр</TD>");
            stringBuilder.Append("<TD>Рейтинг</TD>");
            stringBuilder.Append("</TH>");

            foreach (var track in tracks)
            {
                stringBuilder.Append("<TR>");
                stringBuilder.Append("<TD>-</TD>");
                stringBuilder.Append($"<TD>{track.Id}</TD>");
                stringBuilder.Append($"<TD>{track.Name}</TD>");
                stringBuilder.Append($"<TD>{track.Performer}</TD>");
                stringBuilder.Append($"<TD>{track.Genre}</TD>");
                stringBuilder.Append($"<TD>{track.Rating}</TD>");
                stringBuilder.Append("</TR>");
            }

            stringBuilder.Append("</table>");
            stringBuilder.Append("</div>");

            return stringBuilder.ToString();
        }

        private static string AppendGenreTable(string htmlString, IEnumerable<Genre> genres)
        {
            var stringBuilder = new StringBuilder(htmlString);

            stringBuilder.Append("<div style=\"display: inline-block; margin-left: 35px;\"><H1>Жанры</H1>");
            stringBuilder.Append("<TABLE BORDER=1>");
            stringBuilder.Append("<TH>");
            stringBuilder.Append("<TD>#</TD>");
            stringBuilder.Append("<TD>Наименование</TD>");
            stringBuilder.Append("</TH>");

            foreach (var genre in genres)
            {
                stringBuilder.Append("<TR>");
                stringBuilder.Append("<TD>-</TD>");
                stringBuilder.Append($"<TD>{genre.Id}</TD>");
                stringBuilder.Append($"<TD>{genre.Name}</TD>");
                stringBuilder.Append("</TR>");
            }

            stringBuilder.Append("</table>");
            stringBuilder.Append("</div>");

            return stringBuilder.ToString();
        }

        private static string AppendPerformerTable(string htmlString, IEnumerable<Performer> performers)
        {
            var stringBuilder = new StringBuilder(htmlString);

            stringBuilder.Append("<div style=\"display: inline-block; margin-left: 35px;\"><H1>Исполнители</H1>");
            stringBuilder.Append("<TABLE BORDER=1>");
            stringBuilder.Append("<TH>");
            stringBuilder.Append("<TD>#</TD>");
            stringBuilder.Append("<TD>Наименование</TD>");
            stringBuilder.Append("<TD>Группа?</TD>");
            stringBuilder.Append("</TH>");

            foreach (var performer in performers)
            {
                var groupFlag = performer.IsGroup
                    ? "ДА"
                    : "НЕТ";
                stringBuilder.Append("<TR>");
                stringBuilder.Append("<TD>-</TD>");
                stringBuilder.Append($"<TD>{performer.Id}</TD>");
                stringBuilder.Append($"<TD>{performer.Name}</TD>");
                stringBuilder.Append($"<TD>{groupFlag}</TD>");
                stringBuilder.Append("</TR>");
            }

            stringBuilder.Append("</table>");
            stringBuilder.Append("</div>");

            return stringBuilder.ToString();
        }

        private static string AppendMenu(string htmlString)
        {
            return htmlString +
                   "<div style=\"margin-top: 15px; margin-bottom: 15px;\">" +
                        "<div style=\"display: inline-block; margin-right: 10px;\"><a href=\"/info\">Информация</a></div>" +
                        "<div style=\"display: inline-block; margin-right: 10px;\"><a href=\"/\">Таблица с кешированием</a></div>" +
                        "<div style=\"display: inline-block; margin-right: 10px;\"><a href=\"/searchform1\">Поиск (куки)</a></div>" +
                        "<div style=\"display: inline-block; margin-right: 10px;\"><a href=\"/searchform2\">Поиск (сессия)</a></div>" +
                   "</div>";
        }

        private static void Index(IApplicationBuilder app)
        {
            app.Run(context =>
            {
                var service = context.RequestServices.GetService<ITracksService>();

                if (service == null)
                {
                    throw new InvalidOperationException($"Unable to retrieve {nameof(ITracksService)} service");
                }

                var models = service.GetHomeViewModel(CacheKey);
                var htmlString = "<HTML><HEAD>" +
                                 "<TITLE>С кешированием</TITLE></HEAD>" +
                                 "<META http-equiv='Content-Type' content='text/html; charset=utf-8 />'" +
                                 "<BODY>";
                htmlString = AppendMenu(htmlString);
                htmlString = AppendTrackTable(htmlString, models.Tracks);
                htmlString = AppendGenreTable(htmlString, models.Genres);
                htmlString = AppendPerformerTable(htmlString, models.Performers);

                htmlString += "</BODY></HTML>";

                return context.Response.WriteAsync(htmlString);
            });
        }

        private static void SessionSearchHandler(IApplicationBuilder app)
        {
            app.Run(context =>
            {
                var genre = context.Request.Query["genre"].ToString();
                var performer = context.Request.Query["performer"].ToString();

                context.Session.Set("genre", Encoding.Default.GetBytes(genre));
                context.Session.Set("performer", Encoding.Default.GetBytes(performer));

                var service = context.RequestServices.GetService<ITracksService>();

                if (service == null)
                {
                    throw new InvalidOperationException($"Unable to retrieve {nameof(ITracksService)} service");
                }

                var htmlString = "<HTML><HEAD>" +
                                 "<TITLE>Поиск (сессия)</TITLE></HEAD>" +
                                 "<META http-equiv='Content-Type' content='text/html; charset=utf-8 />'" +
                                 "<BODY>";

                var tracks = service.SearchTracks(performer, genre);
                htmlString = AppendMenu(htmlString);
                htmlString = AppendTrackTable(htmlString, tracks);

                return context.Response.WriteAsync(htmlString);
            });
        }

        private static void SessionSearch(IApplicationBuilder app)
        {
            app.Run(context =>
            {
                var htmlString = "<HTML><HEAD>" +
                                 "<TITLE>Поиск (сессия)</TITLE></HEAD>" +
                                 "<META http-equiv='Content-Type' content='text/html; charset=utf-8 />'" +
                                 "<BODY>";
                htmlString = AppendMenu(htmlString);

                if (!context.Session.TryGetValue("genre", out var genreArray))
                {
                    genreArray = null;
                }

                var genreValue = genreArray == null
                    ? string.Empty
                    : Encoding.Default.GetString(genreArray);

                if (!context.Session.TryGetValue("performer", out var performerArray))
                {
                    performerArray = null;
                }

                var performer = performerArray == null
                    ? string.Empty
                    : Encoding.Default.GetString(performerArray);

                var service = context.RequestServices.GetService<ITracksService>();

                if (service == null)
                {
                    throw new InvalidOperationException($"Unable to retrieve {nameof(ITracksService)} service");
                }

                var genres = service.GetGenres();

                var selectHtml = "<select name='genre'>";
                foreach (var genre in genres)
                {
                    if (genre != string.Empty && genre == genreValue)
                    {
                        selectHtml += $"<option selected=\"selected\" value='{genre}'>" + genre + "</option>";
                    }

                    selectHtml += $"<option value='{genre}'>" + genre + "</option>";
                }

                selectHtml += "</select>";

                htmlString += "<form action = /search2 >" +
                "<br>Жанр: " + selectHtml +
                "<br>Исполнитель: " + $"<input type = 'text' name = 'performer' value='{performer}'>" +
                    "<br><input type = 'submit' value = 'Найти' ></form>";


                htmlString += "</BODY></HTML>";

                return context.Response.WriteAsync(htmlString);
            });
        }

        private static void CookieSearchHandler(IApplicationBuilder app)
        {
            app.Run(context =>
            {
                var genreParam = context.Request.Query["genre"].ToString();
                var performerParam = context.Request.Query["performer"].ToString();

                context.Response.Cookies.Append("genre", genreParam);
                context.Response.Cookies.Append("performer", performerParam);

                var service = context.RequestServices.GetService<ITracksService>();

                if (service == null)
                {
                    throw new InvalidOperationException($"Unable to retrieve {nameof(ITracksService)} service");
                }

                var htmlString = "<HTML><HEAD>" +
                                 "<TITLE>Поиск (куки)</TITLE></HEAD>" +
                                 "<META http-equiv='Content-Type' content='text/html; charset=utf-8 />'" +
                                 "<BODY>";

                var tracks = service.SearchTracks(performerParam, genreParam);
                htmlString = AppendMenu(htmlString);
                htmlString = AppendTrackTable(htmlString, tracks);

                return context.Response.WriteAsync(htmlString);
            });
        }

        private static void CookieSearch(IApplicationBuilder app)
        {
            app.Run(context =>
            {
                var htmlString = "<HTML><HEAD>" +
                                 "<TITLE>Поиск (куки)</TITLE></HEAD>" +
                                 "<META http-equiv='Content-Type' content='text/html; charset=utf-8 />'" +
                                 "<BODY>";
                htmlString = AppendMenu(htmlString);

                if (!context.Request.Cookies.TryGetValue("genre", out var genre))
                {
                    genre = string.Empty;
                }

                if (!context.Request.Cookies.TryGetValue("performer", out var performer))
                {
                    performer = string.Empty;
                }

                var service = context.RequestServices.GetService<ITracksService>();

                if (service == null)
                {
                    throw new InvalidOperationException($"Unable to retrieve {nameof(ITracksService)} service");
                }

                var genres = service.GetGenres();

                var selectHtml = "<select name='genre'>";
                foreach (var genreItem in genres)
                {
                    if (genre != string.Empty && genre == genreItem)
                    {
                        selectHtml += $"<option selected=\"selected\" value='{genreItem}'>" + genreItem + "</option>";
                    }

                    selectHtml += $"<option value='{genreItem}'>" + genreItem + "</option>";
                }

                selectHtml += "</select>";

                htmlString += "<form action = /search1 >" +
                "<br>Жанр: " + selectHtml +
                "<br>Исполнитель: " + $"<input type = 'text' name = 'performer' value='{performer}'>" +
                    "<br><input type = 'submit' value = 'Найти' ></form>";


                htmlString += "</BODY></HTML>";

                return context.Response.WriteAsync(htmlString);
            });
        }
    }
}