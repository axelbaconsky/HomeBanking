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
                        new Account {ClientId = clientRafael.Id, CreationDate = DateTime.Now, Number = String.Empty, Balance = 5000}
                    };
                    foreach (Account account in accounts)
                    {
                        context.Accounts.Add(account);
                    }
                }
                context.SaveChanges();
            }

            if (!context.Transactions.Any())
            {
                var account1 = context.Accounts.FirstOrDefault(c => c.Number == "VIN001");
                if (account1 != null)
                {
                    var transactions = new Transaction[]
                    {
                        new Transaction { AccountId= account1.Id, Amount = 10000, Date= DateTime.Now.AddHours(-5), Description = "Transferencia recibida", Type = TransactionType.CREDIT.ToString() },
                        new Transaction { AccountId= account1.Id, Amount = -2000, Date= DateTime.Now.AddHours(-6), Description = "Compra en tienda mercado libre", Type = TransactionType.DEBIT.ToString() },
                        new Transaction { AccountId= account1.Id, Amount = -3000, Date= DateTime.Now.AddHours(-7), Description = "Compra en tienda adidas", Type = TransactionType.DEBIT.ToString() },
                    };
                    foreach (Transaction transaction in transactions)
                    {
                        context.Transactions.Add(transaction);
                    }
                    context.SaveChanges();
                }
            }
        }
    }
}
