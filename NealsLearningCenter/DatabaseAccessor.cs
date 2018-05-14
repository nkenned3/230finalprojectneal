using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NealsLearningCenter
{
    public class DatabaseAccessor
    {
        private static readonly Entities1 entities;

        static DatabaseAccessor()
        {
            entities = new Entities1();
            entities.Database.Connection.Open();
        }

        public static Entities1 Instance
        {
            get
            {
                return entities;
            }
        }
    }
}