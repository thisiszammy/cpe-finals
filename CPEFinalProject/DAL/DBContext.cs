using CPEFinalProject.Entities;
using CPEFinalProject.Entities.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPEFinalProject.DAL
{
    internal class DBContext<T> where T : BaseEntity
    {
        public delegate bool CustomQuery(T param);
        public static T? GetById(int id)
        {
            var db = Database.GetTable<T>();
            var entity = db.Find(x => x.Uid == id && !x.IsDeleted);
            if (entity == null) return null;
            return entity.Clone<T>();
        }

        public static List<T> GetByCustomQuery(CustomQuery query)
        {
            var db = Database.GetTable<T>();
            return db.Where(x => query.Invoke(x) && !x.IsDeleted).ToList();
        }

        public static void Add(T entity)
        {
            var db = Database.GetTable<T>();
            entity.Uid = (db.Count == 0) ? 1 : db.Max(x => x.Uid) + 1;
            entity.CreatedOn = DateTime.Now;

            db.Add(entity);
        }

        public static void Add(List<T> entities)
        {
            var db = Database.GetTable<T>();

            foreach (var item in entities)
            {
                item.Uid = (db.Count == 0) ? 1 : db.Max(x => x.Uid) + 1;
                item.CreatedOn = DateTime.Now;
                item.CreatedBy = item.CreatedBy;
                db.Add(item);
            }
        }
        public static void Update(T entity)
        {
            var db = Database.GetTable<T>();
            var temp = db.Where(x => x.Uid == entity.Uid).FirstOrDefault();
            if (temp == null) throw new Exception("Record could not be found!");

            var props = temp.GetType().GetProperties().Where(x => Attribute.IsDefined(x, typeof(UpdateEntity)));
            foreach (var item in props)
            {
                var _value = item.GetValue(entity);
                item.SetValue(temp, _value);
            }

            temp.LastModifiedBy = entity.LastModifiedBy;
            temp.LastModifiedOn = DateTime.Now;
        }
        public static void Delete(int uid, int deletedBy)
        {
            var db = Database.GetTable<T>();
            var temp = db.Where(x => x.Uid == uid).FirstOrDefault();
            if (temp == null) throw new Exception("Record could not be found!");
            temp.IsDeleted = true;
            temp.DeletedOn = DateTime.Now;
            temp.DeletedBy = deletedBy;
        }

        public static List<T> GetAllExisting()
        {
            var db = Database.GetTable<T>().Where(x => !x.IsDeleted).ToList();
            return db;
        }

        public static List<T> GetAll()
        {
            var db = Database.GetTable<T>();
            return db;
        }
        
    }
}
