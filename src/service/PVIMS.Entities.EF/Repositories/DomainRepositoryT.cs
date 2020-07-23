using PVIMS.Core.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using VPS.Common.Collections;
using VPS.Common.Domain;
using VPS.Common.Repositories;

namespace PVIMS.Infrastructure.Shared.Repositories
{
    public class DomainRepository<TEntity> : IRepositoryInt<TEntity> where TEntity : Entity<int>
    {
        private readonly DbContext context;
        private readonly DbSet<TEntity> dbSet;

        public DomainRepository(DbContext dbContext)
        {
            context = dbContext;
            dbSet = context.Set<TEntity>();
        }

        public ICollection<TEntity> List()
        {
            return List(null, null, (string)null);
        }

        public async Task<ICollection<TEntity>> ListAsync()
        {
            return await ListAsync(null, null, (string)null);
        }

        public IQueryable<TEntity> Queryable()
        {
            return dbSet;
        }

        public ICollection<TEntity> List(Expression<Func<TEntity, bool>> filter,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy,
            params string[] relatedEntitiesToEagerlyLoad)
        {
            IQueryable<TEntity> query = dbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            foreach (var relatedEntityToEagerlyLoad in relatedEntitiesToEagerlyLoad.Where(s => !string.IsNullOrEmpty(s)))
            {
                query.Include(relatedEntityToEagerlyLoad);
            }

            if (orderBy != null)
            {
                return orderBy(query).ToList();
            }
            else
                return query.ToList();
        }

        public async Task<ICollection<TEntity>> ListAsync(Expression<Func<TEntity, bool>> filter,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy,
            params string[] relatedEntitiesToEagerlyLoad)
        {
            IQueryable<TEntity> query = dbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            foreach (var relatedEntityToEagerlyLoad in relatedEntitiesToEagerlyLoad.Where(s => !string.IsNullOrEmpty(s)))
            {
                query.Include(relatedEntityToEagerlyLoad);
            }

            if (orderBy != null)
            {
                return await orderBy(query).ToListAsync();
            }
            else
                return await query.ToListAsync();
        }

        public ICollection<TEntity> List<TProperty>(Expression<Func<TEntity, bool>> filter,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy,
            Expression<Func<TEntity, TProperty>> relatedEntitiesToEagerlyLoad)
        {
            IQueryable<TEntity> query = dbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            query.Include(relatedEntitiesToEagerlyLoad);

            if (orderBy != null)
            {
                return orderBy(query).ToList();
            }
            else
                return query.ToList();
        }

        public async Task<ICollection<TEntity>> ListAsync<TProperty>(Expression<Func<TEntity, bool>> filter,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy,
            Expression<Func<TEntity, TProperty>> relatedEntitiesToEagerlyLoad)
        {
            IQueryable<TEntity> query = dbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            query.Include(relatedEntitiesToEagerlyLoad);

            if (orderBy != null)
            {
                return await orderBy(query).ToListAsync();
            }
            else
                return await query.ToListAsync();
        }

        public PagedCollection<TEntity> List(IPagingInfo pagingInfo,
            Expression<Func<TEntity, bool>> filter,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy,
            params string[] relatedEntitiesToEagerlyLoad)
        {
            IQueryable<TEntity> queryBeforePaging = dbSet;

            if (filter != null)
            {
                queryBeforePaging = queryBeforePaging.Where(filter);
            }

            foreach (var relatedEntityToEagerlyLoad in relatedEntitiesToEagerlyLoad.Where(s => !string.IsNullOrEmpty(s)))
            {
                queryBeforePaging.Include(relatedEntityToEagerlyLoad);
            }

            var count = queryBeforePaging.Count();

            if (orderBy != null)
            {
                queryBeforePaging = orderBy(queryBeforePaging);
            }
            else
                queryBeforePaging = queryBeforePaging.OrderBy(h => h.Id);

            return PagedCollection<TEntity>.Create(queryBeforePaging,
                pagingInfo.PageNumber,
                pagingInfo.PageSize);
        }

        public PagedCollection<TEntity> List<TProperty>(IPagingInfo pagingInfo,
            Expression<Func<TEntity, bool>> filter,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy,
            Expression<Func<TEntity, TProperty>> relatedEntitiesToEagerlyLoad)
        {
            IQueryable<TEntity> queryBeforePaging = dbSet;

            if (filter != null)
            {
                queryBeforePaging = queryBeforePaging.Where(filter);
            }

            queryBeforePaging.Include(relatedEntitiesToEagerlyLoad);

            var count = queryBeforePaging.Count();

            if (orderBy != null)
            {
                queryBeforePaging = orderBy(queryBeforePaging);
            }
            else
                queryBeforePaging = queryBeforePaging.OrderBy(h => h.Id);

            return PagedCollection<TEntity>.Create(queryBeforePaging,
                pagingInfo.PageNumber,
                pagingInfo.PageSize);
        }

        public async Task<TEntity> GetAsync(object entityId)
        {
            return await dbSet.FindAsync(entityId);
        }

        public async Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> filter)
        {
            IQueryable<TEntity> query = dbSet;

            return await query.SingleOrDefaultAsync(filter);
        }

        public async Task<TEntity> GetAsync(object entityId, string[] relatedEntitiesToEagerlyLoad)
        {
            IQueryable<TEntity> query = dbSet;

            foreach (var relatedEntityToEagerlyLoad in relatedEntitiesToEagerlyLoad.Where(s => !string.IsNullOrEmpty(s)))
            {
                query.Include(relatedEntityToEagerlyLoad);
            }

            return await query.SingleOrDefaultAsync(q => q.Id == (long)entityId);
        }

        public async Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> filter, string[] relatedEntitiesToEagerlyLoad)
        {
            IQueryable<TEntity> query = dbSet;

            foreach (var relatedEntityToEagerlyLoad in relatedEntitiesToEagerlyLoad.Where(s => !string.IsNullOrEmpty(s)))
            {
                query.Include(relatedEntityToEagerlyLoad);
            }

            return await query.SingleOrDefaultAsync(filter);
        }

        public TEntity Get(object entityId)
        {
            return dbSet.Find(entityId);
        }

        public TEntity Get(Expression<Func<TEntity, bool>> filter)
        {
            IQueryable<TEntity> query = dbSet;

            return query.SingleOrDefault(filter);
        }

        public TEntity Get(object entityId, string[] relatedEntitiesToEagerlyLoad)
        {
            IQueryable<TEntity> query = dbSet;

            foreach (var relatedEntityToEagerlyLoad in relatedEntitiesToEagerlyLoad.Where(s => !string.IsNullOrEmpty(s)))
            {
                query.Include(relatedEntityToEagerlyLoad);
            }

            return query.SingleOrDefault(q => q.Id == (long)entityId);
        }

        public TEntity Get(Expression<Func<TEntity, bool>> filter, string[] relatedEntitiesToEagerlyLoad)
        {
            IQueryable<TEntity> query = dbSet;

            foreach (var relatedEntityToEagerlyLoad in relatedEntitiesToEagerlyLoad.Where(s => !string.IsNullOrEmpty(s)))
            {
                query.Include(relatedEntityToEagerlyLoad);
            }

            return query.SingleOrDefault(filter);
        }

        public void Save(TEntity entityToSave)
        {
            dbSet.Add(entityToSave);

            // Need the Id to be returned.
            context.SaveChanges();
        }

        public async Task SaveAsync(TEntity entityToSave)
        {
            dbSet.Add(entityToSave);

            // Need the Id to be returned.
            await context.SaveChangesAsync();
        }

        public void Save(TEntity[] entitiesToSave)
        {
            dbSet.AddRange(entitiesToSave).ToArray();
            // This should be saved in the UnitOfWork.
            //context.SaveChanges();
        }

        public void Update(TEntity entityToUpdate)
        {
            try
            {
                dbSet.Attach(entityToUpdate);
                context.Entry(entityToUpdate).State = EntityState.Modified;
            }
            finally
            {
            }
        }

        public void Update(TEntity[] entitiesToUpdate)
        {
            foreach (var entityToUpdate in entitiesToUpdate)
            {
                Update(entityToUpdate);
            }
        }

        public void Delete(TEntity entityToDelete)
        {
            if (context.Entry(entityToDelete).State == EntityState.Detached)
            {
                dbSet.Attach(entityToDelete);
            }

            dbSet.Remove(entityToDelete);
        }

        public void Delete(object entityToDeleteId)
        {
            TEntity entityToDelete = dbSet.Find(entityToDeleteId);
            Delete(entityToDelete);
        }

        public void Delete(Expression<Func<TEntity, bool>> filter = null)
        {
            var entitiesToDelete = dbSet.Where(filter);
            dbSet.RemoveRange(entitiesToDelete);
        }

        public bool Exists(Expression<Func<TEntity, bool>> filter = null)
        {
            return filter == null
                ? dbSet.Any()
                : dbSet.Any(filter);
        }

        public async Task<ICollection<TEntity>> ExecuteSqlAsync(string sql, params SqlParameter[] parameters)
        {
            context.Database.CommandTimeout = 4800;
            return await context.Database.SqlQuery<TEntity>(sql, parameters).ToListAsync();
        }

        public ICollection<TEntity> ExecuteSql(string sql, params SqlParameter[] parameters)
        {
            context.Database.CommandTimeout = 4800;
            return context.Database.SqlQuery<TEntity>(sql, parameters).ToList();
        }

        public async Task<int> ExecuteSqlCommandAsync(string sql, params SqlParameter[] parameters)
        {
            return await context.Database.ExecuteSqlCommandAsync(sql, parameters);
        }

        public int ExecuteSqlCommand(string sql, params SqlParameter[] parameters)
        {
            try
            {
                return context.Database.ExecuteSqlCommand(sql, parameters);
            }
            catch (Exception)
            {

                throw new Exception(sql);
            }
        }

        public int ExecuteSqlScalar(string sql, params SqlParameter[] parameters)
        {
            return context.Database.SqlQuery<int>(sql, parameters).FirstOrDefault();
        }
    }
}
