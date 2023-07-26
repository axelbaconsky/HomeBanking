using System;
using System.Linq;

namespace HomeBankingMindHub.Models
{
    public class DBInitializer
    {
        public static void Initialize(HomeBankingContext context)
        {
            if (!context.Clients.Any())
            {
                //Creamos los datos de prueba
                var clients = new Client[]
                {
                    new Client {
                        FirstName="Eduardo",
                        LastName="Mendoza",
                        Email = "eduardo@gmail.com",
                        Password="123456"
                },
                    new Client {
                    FirstName = "Rafael",
                    LastName = "Rodriguez",
                    Email = "rafael@gmail.com",
                    Password = "123456"
                },
             };

            foreach (Client client in clients)
                {
                    context.Clients.Add(client);
                }

                //guardamos
                context.SaveChanges();
            }

            if (!context.Accounts.Any())
            {
                var clientRafael = context.Clients.FirstOrDefault(c => c.Email == "rafael@gmail.com");
                if (clientRafael != null)
                {
                    var accounts = new Account[]
                    {
                        new Account {ClientId = clientRafael.Id, CreationDate = DateTime.Now, Number = string.Empty, Balance = 1000}
                    };
                    foreach (Account account in accounts)
                    {
                        context.Accounts.Add(account);
                    }
                }
                context.SaveChanges();
            }

        }
    }
}
