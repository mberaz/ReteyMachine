using Microsoft.EntityFrameworkCore;
using RetryMachine.SQL.Models;
using System.Linq.Expressions;

namespace RetryMachine.SQL
{
    public class EntityBaseRepository<T>
             where T : class, new()
    {

        private readonly RetrymachineContext _context;

        #region Properties
        public EntityBaseRepository(RetrymachineContext context)
        {
            _context = context;
        }
        #endregion
        public IEnumerable<T> GetAll()
        {
            return _context.Set<T>().AsEnumerable();
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _context.Set<T>().ToListAsync();
        }
        public IEnumerable<T> AllIncluding(params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> query = _context.Set<T>();
            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }
            return query.AsEnumerable();
        }

        public async Task<IEnumerable<T>> AllIncludingAsync(params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> query = _context.Set<T>();
            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }
            return await query.ToListAsync();
        }

        public T GetSingle(Expression<Func<T, bool>> predicate)
        {
            return _context.Set<T>().FirstOrDefault(predicate);
        }

        public T GetSingle(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> query = _context.Set<T>();
            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }

            return query.Where(predicate).FirstOrDefault();
        }

        public async Task<T> GetSingleIncludeAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includeProperties)
        {
            var query = _context.Set<T>().Where(predicate);
            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }

            var result = await query.ToListAsync();
            return result.FirstOrDefault();
        }


        public IEnumerable<T> Where(Expression<Func<T, bool>> predicate)
        {
            return _context.Set<T>().Where(predicate);
        }

        public async Task<IEnumerable<T>> WhereAsync(Expression<Func<T, bool>> predicate)
        {
            return await _context.Set<T>().Where(predicate).ToListAsync();
        }

        public async Task<IEnumerable<T>> WhereAsyncWithTakeDesc<Tkey>(Expression<Func<T, Tkey>> orderPredicate,
            Expression<Func<T, bool>> predicate, int page, int pageSize, params Expression<Func<T, object>>[] includeProperties)
        {
            var query = _context.Set<T>().OrderByDescending(orderPredicate).Where(predicate);
            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }

            return await query.Skip(page * pageSize).Take(pageSize).ToListAsync();
        }

        public async Task<Tkey> MaxAsync<Tkey>(Expression<Func<T, bool>> predicate, Func<T, Tkey> selectPredicate)
        {
            var query = _context.Set<T>().Where(predicate);

            var whereResult = await query.ToListAsync();
            return whereResult.Select(selectPredicate).Max();
        }

        public async Task<Tkey> MinAsync<Tkey>(Expression<Func<T, bool>> predicate, Func<T, Tkey> selectPredicate)
        {
            var query = _context.Set<T>().Where(predicate);

            var whereResult = await query.ToListAsync();
            return whereResult.Select(selectPredicate).Min();
        }


        public async Task<IEnumerable<T>> WhereIncludingAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> query = _context.Set<T>().Where(predicate);
            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }
            return await query.ToListAsync();
        }

        public async Task<IEnumerable<T>> WhereTopAsync(Expression<Func<T, bool>> predicate, int page = 0, int pageSize = 10)
        {
            return await _context.Set<T>().Where(predicate)
                .Skip(page * pageSize)
                .Take(pageSize).ToListAsync();
        }

        public Task<int> CountAsync(Expression<Func<T, bool>> predicate)
        {
            return _context.Set<T>().CountAsync(predicate);
        }

        public Task<bool> AnyAsync(Expression<Func<T, bool>> predicate)
        {
            return _context.Set<T>().AnyAsync(predicate);
        }

        public void Add(T entity)
        {
            var dbEntityEntry = _context.Entry<T>(entity);
            _context.Set<T>().Add(entity);
        }

        public void Edit(T entity)
        {
            var dbEntityEntry = _context.Entry<T>(entity);
            dbEntityEntry.State = EntityState.Modified;
        }
        public void Delete(T entity)
        {
            var dbEntityEntry = _context.Entry<T>(entity);
            dbEntityEntry.State = EntityState.Deleted;
        }

        public void Commit()
        {
            _context.SaveChanges();
        }

        public async Task<int> CommitAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void AddRange(List<T> entities)
        {
            _context.Set<T>().AddRange(entities);
        }

        public void DeleteRange(List<T> entities)
        {
            _context.Set<T>().RemoveRange(entities);
        }


    }
}