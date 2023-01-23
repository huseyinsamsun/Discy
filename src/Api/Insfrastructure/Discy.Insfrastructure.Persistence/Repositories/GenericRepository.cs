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
        private readonly DiscyContext discyContext;
        protected DbSet<TEntity> entity => discyContext.Set<TEntity>();
        public GenericRepository(DiscyContext discyContext)
        {
            this.discyContext = discyContext ?? throw new ArgumentNullException(nameof(discyContext));
        }

        public virtual  int Add(TEntity entity)
        {
            this.entity.Add(entity);
            return discyContext.SaveChanges(); ;

        }

        public virtual int Add(IEnumerable<TEntity> entities)
        {
             this.entity.AddRange(entities);
            return discyContext.SaveChanges();
        }

        public virtual async Task<int> AddAsync(TEntity entity)
        {
            await this.entity.AddAsync(entity);
            return await discyContext.SaveChangesAsync();
        }

        public virtual async Task<int> AddAsync(IEnumerable<TEntity> entities)
        {
            await this.entity.AddRangeAsync(entities);
            return await discyContext.SaveChangesAsync();
             
        }

        public virtual int AddOrUpdate(TEntity entity)
        {
            if(!this.entity.Local.Any(i=>EqualityComparer<Guid>.Default.Equals(i.Id,entity.Id)))
                
              discyContext.Update(entity);
            return discyContext.SaveChanges();
            
        }

        public virtual Task<int> AddOrUpdateAsync(TEntity entity)
        {

            if (!this.entity.Local.Any(i => EqualityComparer<Guid>.Default.Equals(i.Id, entity.Id))) //memory de entry kontrolü

                discyContext.Update(entity);
            return discyContext.SaveChangesAsync();
        }

        public virtual IQueryable<TEntity> AsQueryable() => entity.AsQueryable();


        public virtual  async Task BulkAdd(IEnumerable<TEntity> entities)
        {
            if (entities != null && !entities.Any())
           await Task.CompletedTask;
        
            await entity.AddRangeAsync(entities);
            await discyContext.SaveChangesAsync();
        }

        public virtual async Task BulkDelete(Expression<Func<TEntity, bool>> predicate)
        {
            var entitiesToDelete = await entity.Where(predicate).ToListAsync();
            if (entitiesToDelete.Any())
            {
                entity.RemoveRange(entitiesToDelete);
                await discyContext.SaveChangesAsync();
            }
        }

        public virtual  Task BulkDelete(IEnumerable<TEntity> entities)
        {
 

             entity.RemoveRange(entities);
             return discyContext.SaveChangesAsync();
        }

        public virtual Task BulkDeleteById(IEnumerable<Guid> ids)
        {
            if (ids != null && ids.Any())
                return Task.CompletedTask;

            discyContext.RemoveRange(entity.Where(i => ids.Contains(i.Id)));
            return discyContext.SaveChangesAsync();
        }

        public async Task  BulkUpdate(IEnumerable<TEntity> entities)
        {
            if (entities != null && entities.Any())
            {
                entity.UpdateRange(entities);
                await discyContext.SaveChangesAsync();
            }
        }

        public virtual int Delete(TEntity entity)
        {
            if(discyContext.Entry(entity).State==EntityState.Detached)
            {
                this.entity.Attach(entity);
            }
            this.entity.Remove(entity);
            return discyContext.SaveChanges();
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
           if(discyContext.Entry(entity).State==EntityState.Detached)
            {
                this.entity.Attach(entity);
            }
            this.entity.Remove(entity);
            return discyContext.SaveChangesAsync();
        }

        public virtual bool DeleteRange(Expression<Func<TEntity, bool>> predicate)
        {
            discyContext.RemoveRange(predicate);
            return discyContext.SaveChanges() > 0;
        }

        public virtual async Task<bool> DeleteRangeAsync(Expression<Func<TEntity, bool>> predicate)
        {
            discyContext.RemoveRange(predicate);
            return await discyContext.SaveChangesAsync()>0;
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
                discyContext.Entry(found).State = EntityState.Detached;
            foreach (Expression<Func<TEntity, object>> include in includes)
            {
                discyContext.Entry(found).Reference(include).Load();
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
            discyContext.Entry(entity).State = EntityState.Modified;
            return discyContext.SaveChanges();
        }

        public virtual async Task<int> UpdateAsync(TEntity entity)
        {
            this.entity.Attach(entity);
            discyContext.Entry(entity).State = EntityState.Modified;
            return await discyContext.SaveChangesAsync();
        }
        public int SaveChanges()
        {
            return discyContext.SaveChanges();
        }
        public Task<int> SaveChangesAsync()
        {
            return discyContext.SaveChangesAsync();
        }

    }
}
