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

        }
    }
}
