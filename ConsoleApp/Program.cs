using SamuraiApp.Domain;
using SamuraiApp.Data;
using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32.SafeHandles;
using System.Collections.Generic;
//using SamuraiDataSecond;

namespace ConsoleApp
{
    class Program
    {
        private static SamuraiContext _context = new SamuraiContext();
        static void Main(string[] args)
        {
            //context.Database.EnsureCreated();
            //GetSamurais("Before Add:");
            //AddSamurai();
            //GetSamurais("After Add:");
            //InsertMultipleSamurais();
            //InsertVariousTypes();
            //GetSamurais("what");
            //QueryFilters();
            //RetrieveAndUpdateSamurai();
            //RetrieveAndUpdateMultipleSamurai();
            //RetrieveAndDeleteSamurai(); //Crasher grundet ikke eksisterende Id
            //MultipleDatabaseOperations();
            //InsertBattle();
            //QueryAndUpdateBattle_Disconnected();
            //InsertNewSamuraiWithAQuote();
            //ProjectSamuraisWithQuote();
            ExplicitLoadQuotes();
            //GetSamurais("Hey");
            Console.ReadKey();



        }

        #region Del6
        private static void InsertNewSamuraiWithAQuote()
        {
            var samurai = new Samurai
            {
                Name = "Samurai Jack",
                Quotes = new List<Quote>
                {
                    new Quote { Text = "Aku!"}
                }
            };
            _context.Samurais.Add(samurai);
            _context.SaveChanges();
        }
        private static void InsertNewSamuraiWithManyQuotes()
        {
            var samurai = new Samurai
            {
                Name = "Samurai Jack",
                Quotes = new List<Quote>
                {
                    new Quote { Text = "Aku!"},
                    new Quote { Text = "Who else wants some?!"}
                }
            };
            _context.Samurais.Add(samurai);
            _context.SaveChanges();
        }
        private static void AddQuoteToExistingSamuraiWithTracking()
        {
            var samurai = _context.Samurais.FirstOrDefault();
            samurai.Quotes.Add(new Quote
            {
                Text = "Thanks are not necessary"
            });
            _context.SaveChanges();
        }
        private static void AddQuoteToExistingSamuraiNotTracked(int samuraiId)
        {
            var samurai = _context.Samurais.Find(samuraiId);
            samurai.Quotes.Add( new Quote
            {
                Text = "Yes, it is I, Samurai Jack."
            });
            using(var newContext = new SamuraiContext())
            {
                newContext.Samurais.Update(samurai);
                newContext.SaveChanges();
            }
        }
        private static void AddQuoteToExistingSamuraiNotTracked_easy(int samuraiId)
        {
            Quote quote = (new Quote
            {
                Text = "Yes, it is I, Samurai Jack.",
                SamuraiId = samuraiId
            }); ;
            using (var newContext = new SamuraiContext())
            {
                newContext.Quotes.Add(quote);
                newContext.SaveChanges();
            }
        }
        private static void EagerLoadingSamuraiWithQuotes()
        {
            var samuraiWithQuotes = _context.Samurais
                .Include(s => s.Quotes)
                .Include(s => s.Clan)
                .ToList();
        }
        private static  void ProjectSomeProperties()
        {
            var someProperties = _context.Samurais.Select(s => new { s.Id, s.Name }).ToList();
            var idAndNames = _context.Samurais.Select(s => new IdAndName( s.Id, s.Name)).ToList();
        }
        public struct IdAndName
        {
            public IdAndName(int id, string name)
            {
                Id = id;
                Name = name;
            }
            public int Id;
            public string Name;
        }

        public static void ProjectSamuraisWithQuote()
        {
            //var somePropertiesWithQuotes = _context.Samurais.Select(s => new { s.Id, s.Name, s.Quotes }).ToList();
            //var somePropertiesWithQuotes = _context.Samurais.Select(s => new { s.Id, s.Name, s.Quotes.Count }).ToList();
            var somePropertiesWithQuotes = _context.Samurais
                .Select(s => new { s.Id, s.Name, 
                    whereQuotes = s.Quotes.Where(q => q.Text.Contains("Aku"))
            })
                .ToList();


        }

        private static void ExplicitLoadQuotes()
        {
            var samurai = _context.Samurais.FirstOrDefault(s => s.Name.Contains("Jack"));
            _context.Entry(samurai).Collection(s => s.Quotes).Load();
            _context.Entry(samurai).Reference(s => s.Horse).Load();
        }
        private static void LazyLoadingQuotes()
        {
            var samurai = _context.Samurais.FirstOrDefault(s => s.Name.Contains("Jack"));
            var quoteCount = samurai.Quotes.Count();
        }
        private static void ModifyingRelatedDataTracked()
        {
            var samurai = _context.Samurais.Include(s => s.Quotes).FirstOrDefault(s => s.Id == 2);
            samurai.Quotes[0].Text = "Did you hear that?";
            _context.Quotes.Remove(samurai.Quotes[1]);
            _context.SaveChanges();
        }

        private static void QueryAndUpdateBattle_Disconnected() 
        {
            var battle = _context.Battles.AsNoTracking().FirstOrDefault();
            battle.EndDate = new DateTime(1560, 06, 30);
            using(var newContextInstance = new SamuraiContext())
            {
                newContextInstance.Battles.Update(battle);
                newContextInstance.SaveChanges();
            }
        }
        private static void InsertBattle()
        {
            _context.Add(new Battle
            {
                Name = "Battle of Okehazama",
                StartDate = new DateTime(1560, 06, 1),
                EndDate = new DateTime(1560, 06, 15)
            });
            _context.SaveChanges();
        }

        #region Samurai Stuff
        private static void RetrieveAndDeleteSamurai()
        {
            var samurai = _context.Samurais.Find(4);
            _context.Samurais.Remove(samurai);
            _context.SaveChanges();

        }

        private static void MultipleDatabaseOperations()
        {
            var samurai = _context.Samurais.FirstOrDefault();
            samurai.Name = "Samurai " + samurai.Name;
            _context.Samurais.Add(new Samurai { Name = "Jack" });
            _context.SaveChanges();
        }

        private static void RetrieveAndUpdateMultipleSamurai()
        {
            var samurais = _context.Samurais.Skip(1).Take(3).ToList();
            samurais.ForEach(s => s.Name += " San");
            _context.SaveChanges();

        }

        private static void RetrieveAndUpdateSamurai()
        {
            var samurai = _context.Samurais.FirstOrDefault();
            samurai.Name += " San";
            _context.SaveChanges();
        }

        private static void QueryFilters()
        {
            var name = "Tonny";
            //var samurais = _context.Samurais.Where(s => s.Name == name).ToList();
            //var samurais = _context.Samurais.Where(s => s.Name.Contains(name)).ToList();
            //var samurais = _context.Samurais.Where(s => EF.Functions.Like(s.Name, "%" + name + "%")).ToList();
            //var samurai = _context.Samurais.Where(s => EF.Functions.Like(s.Name, "%" + name + "%")).FirstOrDefault();
            //var samurai = _context.Samurais.Find(1);
            var samurai = _context.Samurais.FirstOrDefault(s => s.Name == name );
            Console.WriteLine(samurai.Name);
            //foreach (Samurai samurai in samurais)
            //{
            //    Console.WriteLine(samurai.Name);
            //}

        }

        

        private static void AddSamurai()
        {
            var samurai = new Samurai { Name = "Spock" };
            _context.Samurais.Add(samurai);
            _context.SaveChanges();
        }
        private static void InsertMultipleSamurais()
        {
            var samurai = new Samurai { Name = "Tonny" };
            var samurai2 = new Samurai { Name = "Benny" };
            var samurai3 = new Samurai { Name = "Tonny" };
            var samurai4 = new Samurai { Name = "Benny" };
            _context.Samurais.AddRange(samurai, samurai2, samurai3, samurai4);

            _context.SaveChanges();
        }
        private static void InsertVariousTypes()
        {
            var samurai = new Samurai { Name = "Troels" };
            var clan = new Clan { ClanName = "Imperial Clan" };
            _context.AddRange(samurai, clan);
            _context.SaveChanges();
        }
        private static void GetSamurais(string text)
        {
            var samurais = _context.Samurais.ToList();
            Console.WriteLine($"{text}: Samurai count is {samurais.Count}");
            foreach (var samurai in samurais)
            {
                Console.WriteLine(samurai.Name);
            }
        }
        private static void GetSamuraisSimpler()
        {
            //var samurais = context.Samurais.ToList();
            var query = _context.Samurais;
            //var samurais = query.ToList();
            foreach (var samurai in query)
            {
                Console.WriteLine(samurai.Name);
            }
        }
        #endregion
        #endregion

    }
}
