using Discy.Api.Application.Repositories;
using Discy.Api.Domain.Models;
using Discy.Insfrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer.Query.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Discy.Insfrastructure.Persistence.Repositories
{
    public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : BaseEntity
    {
        private readonly DbContext dbContext;
        protected DbSet<TEntity> entity => dbContext.Set<TEntity>();
        public GenericRepository(DbContext dbContext)
        {
            this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public virtual  int Add(TEntity entity)
        {
            this.entity.Add(entity);
            return dbContext.SaveChanges(); ;

        }

        public virtual int Add(IEnumerable<TEntity> entities)
        {
             this.entity.AddRange(entities);
            return dbContext.SaveChanges();
        }

        public virtual async Task<int> AddAsync(TEntity entity)
        {
            await this.entity.AddAsync(entity);
            return await dbContext.SaveChangesAsync();
        }

        public virtual async Task<int> AddAsync(IEnumerable<TEntity> entities)
        {
            await this.entity.AddRangeAsync(entities);
            return await dbContext.SaveChangesAsync();
             
        }

        public virtual int AddOrUpdate(TEntity entity)
        {
            if(!this.entity.Local.Any(i=>EqualityComparer<Guid>.Default.Equals(i.Id,entity.Id)))
                
              dbContext.Update(entity);
            return dbContext.SaveChanges();
            
        }

        public virtual Task<int> AddOrUpdateAsync(TEntity entity)
        {

            if (!this.entity.Local.Any(i => EqualityComparer<Guid>.Default.Equals(i.Id, entity.Id))) //memory de entry kontrolü

                dbContext.Update(entity);
            return dbContext.SaveChangesAsync();
        }

        public virtual IQueryable<TEntity> AsQueryable() => entity.AsQueryable();


        public virtual  async Task BulkAdd(IEnumerable<TEntity> entities)
        {
            if (entities != null && !entities.Any())
           await Task.CompletedTask;
        
            await entity.AddRangeAsync(entities);
            await dbContext.SaveChangesAsync();
        }

        public virtual async Task BulkDelete(Expression<Func<TEntity, bool>> predicate)
        {
            var entitiesToDelete = await entity.Where(predicate).ToListAsync();
            if (entitiesToDelete.Any())
            {
                entity.RemoveRange(entitiesToDelete);
                await dbContext.SaveChangesAsync();
            }
        }

        public virtual  Task BulkDelete(IEnumerable<TEntity> entities)
        {
 

             entity.RemoveRange(entities);
             return dbContext.SaveChangesAsync();
        }

        public virtual Task BulkDeleteById(IEnumerable<Guid> ids)
        {
            if (ids != null && ids.Any())
                return Task.CompletedTask;

            dbContext.RemoveRange(entity.Where(i => ids.Contains(i.Id)));
            return dbContext.SaveChangesAsync();
        }

        public async Task  BulkUpdate(IEnumerable<TEntity> entities)
        {
            if (entities != null && entities.Any())
            {
                entity.UpdateRange(entities);
                await dbContext.SaveChangesAsync();
            }
        }

        public virtual int Delete(TEntity entity)
        {
            if(dbContext.Entry(entity).State==EntityState.Detached)
            {
                this.entity.Attach(entity);
            }
            this.entity.Remove(entity);
            return dbContext.SaveChanges();
        }

        public virtual int Delete(Guid id)
        {
            var entity = this.entity.Find(id);
            return Delete(entity);
        }

        public virtual Task<int> DeleteAsync(Guid id)
        {
            var entity = this.entity.Find(id);
            return DeleteAsync(entity);
        }

        public virtual Task<int> DeleteAsync(TEntity entity)
        {
           if(dbContext.Entry(entity).State==EntityState.Detached)
            {
                this.entity.Attach(entity);
            }
            this.entity.Remove(entity);
            return dbContext.SaveChangesAsync();
        }

        public virtual bool DeleteRange(Expression<Func<TEntity, bool>> predicate)
        {
            dbContext.RemoveRange(entity.Where(predicate));
            return dbContext.SaveChanges() > 0;
        }

        public virtual async Task<bool> DeleteRangeAsync(Expression<Func<TEntity, bool>> predicate)
        {
            dbContext.RemoveRange(predicate);
            return await dbContext.SaveChangesAsync()>0;
        }

        public Task<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, bool noTracking = true, params Expression<Func<TEntity, object>>[] includes)
        {
            throw new NotImplementedException();
        }

        public virtual IQueryable<TEntity> Get(Expression<Func<TEntity, bool>> predicate, bool noTracking = true, params Expression<Func<TEntity, object>>[] includes)
        {
            var query = entity.AsQueryable();
            if(predicate!=null)
            {
                query = query.Where(predicate);
            }
             
            query = ApplyIncludes(query, includes);
            if (noTracking)
                query = query.AsNoTracking();
            return query;
        }

        private static IQueryable<TEntity> ApplyIncludes(IQueryable<TEntity> query, params Expression<Func<TEntity, object>>[] includes)
        {
            if(includes!=null)
            {
                foreach (var includeItem in includes)
                {
                    query = query.Include(includeItem);
                }
            }
            return query;
        }
   

        public async Task<List<TEntity>> GetAll(bool noTracking = true)
        {
            var query = entity.AsQueryable();
            if (noTracking)
                query = query.AsNoTracking();
            return await query.ToListAsync();
        }

        public async Task<TEntity> GetByIdAsync(Guid id, bool noTracking = true, params Expression<Func<TEntity, object>>[] includes)
        {
            TEntity found = await entity.FindAsync(id);
            if (found == null)
                return null;
            if(noTracking)
                dbContext.Entry(found).State = EntityState.Detached;
            foreach (Expression<Func<TEntity, object>> include in includes)
            {
                dbContext.Entry(found).Reference(include).Load();
            }
            return found;
        }

        public virtual async Task<List<TEntity>> GetList(Expression<Func<TEntity, bool>> predicate, bool noTracking = true, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, params Expression<Func<TEntity, object>>[] includes)
        {
            IQueryable<TEntity> query = entity;
            if(predicate!=null)
            {
                query = query.Where(predicate);

            }
            foreach (Expression<Func<TEntity,object>> include in includes)
            {
                query.Include(include);
            }
            if(orderBy!=null)
            {
                query = orderBy(query);

            }
            if (noTracking)
                query = query.AsNoTracking();
            return await query.ToListAsync();
        }

        public async Task<TEntity> GetSingleAsync(Expression<Func<TEntity, bool>> predicate, bool noTracking = true, params Expression<Func<TEntity, object>>[] includes)
        {

            IQueryable<TEntity> query = entity;
            if(predicate!=null)
            {
                query = query.Where(predicate);
            }
            query=ApplyIncludes(query, includes);
            if (noTracking)
                query = query.AsNoTracking();
            return await query.SingleOrDefaultAsync();
        }

        public virtual int Update(TEntity entity)
        {
            this.entity.Attach(entity);
            dbContext.Entry(entity).State = EntityState.Modified;
            return dbContext.SaveChanges();
        }

        public virtual async Task<int> UpdateAsync(TEntity entity)
        {
            this.entity.Attach(entity);
            dbContext.Entry(entity).State = EntityState.Modified;
            return await dbContext.SaveChangesAsync();
        }
        public int SaveChanges()
        {
            return dbContext.SaveChanges();
        }
        public Task<int> SaveChangesAsync()
        {
            return dbContext.SaveChangesAsync();
        }

    }
}
