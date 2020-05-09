using Red;
using System;
using System.Net;
using System.Threading.Tasks;
using ConsoleApp1;
using DreamPlace.Lib.Rx;
using Serilog.Core;

namespace Project53.New_arhtech.Http.RedServer
{
    public class Server
    {
        private Logger _logger;
        public Server()
        {
            _logger = Registry.GetValue<Logger>();
        }
        
        private RedHttpServer server;
        //async Task<HandlerType> Auth(Request req, Response res)
        //{
        //    if (req.Headers["Auth"] == "123")
        //        return HandlerType.Continue;

        //    await res.SendStatus(HttpStatusCode.Unauthorized);
        //    return HandlerType.Final;
        //}
        
        public void Start()
        {
            var server = new RedHttpServer(5000);
            server.RespondWithExceptionDetails = true;
            ConfigureActions(server);
            server.OnHandlerException += (e, sender)
                => _logger.Error("Ошибка внутреннего сервера", e);
            server.Start();

            _logger.Information("Server started.");
            
            // server = new RedHttpServer(5000);
            // server.Get("/index", Auth, (req, res) => res.SendFile("./index.html"));
            // server.Get("/auth", (req, res) => res.SendJson(""));
            //
            // // server.Post("/logout", Auth, async (req, res) =>
            // // {
            // //     return await res.SendStatus(HttpStatusCode.OK);
            // // });
            //
            // await server.StartAsync();
        }

        private static void ConfigureActions(RedHttpServer server)
        {
            server.Post("/logout", async (req, res) =>
            {
                Registry.OnNext("", RegistryAddresses.Logout);
                return await res.SendStatus(HttpStatusCode.OK);
            });
        }
    }
}