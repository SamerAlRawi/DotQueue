using System.Collections.Generic;
using System.Linq;
using DotQueue.HostLib;
using Raven.Client;
using Raven.Client.Linq;

namespace DotQueue.Persistence.RavenDB
{
    public class RavenDbPersistenceAdapter : IPersistenceAdapter
    {
        private IDocumentStore _documentStore;

        public RavenDbPersistenceAdapter(IDocumentStore documentStore)
        {
            _documentStore = documentStore;
            _documentStore.Initialize();
        }

        public IEnumerable<Message> GetAll()
        {
            using (IDocumentSession session = _documentStore.OpenSession())
            {
                return GetAllWithLoop<Message>(session);
            }
        }

        public void Add(Message customer)
        {
            using (IDocumentSession session = _documentStore.OpenSession())
            {
                session.Store(customer);
                session.SaveChanges();
            }
        }

        public void Delete(Message customer)
        {
            using (IDocumentSession session = _documentStore.OpenSession())
            {
                var customerId = customer.Id;

                var instance = session.Load<Message>(customerId);
                if (instance != null)
                {
                    session.Delete(instance);
                    session.SaveChanges();
                }
            }
        }

        private static List<T> GetAllWithLoop<T>(IDocumentSession session)
        {
            const int size = 1024;
            int page = 0;

            RavenQueryStatistics stats;
            List<T> objects = session.Query<T>()
                                  .Statistics(out stats)
                                  .Skip(page * size)
                                  .Take(size)
                                  .ToList();

            page++;

            while ((page * size) <= stats.TotalResults)
            {
                objects.AddRange(session.Query<T>()
                             .Skip(page * size)
                             .Take(size)
                             .ToList());
                page++;
            }

            return objects;
        }
    }
}
