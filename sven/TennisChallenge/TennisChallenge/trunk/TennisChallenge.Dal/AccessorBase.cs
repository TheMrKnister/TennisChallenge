using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Objects.DataClasses;
using System.Linq;
using System.Linq.Expressions;

namespace TennisChallenge.Dal
{
  public class AccessorBase<TEntityType>
    where TEntityType : EntityObject
  {
    private readonly DbContext _dbContext;

    public AccessorBase(DbContext dbContext = null)
    {
      _dbContext = dbContext ?? new DbContext(new TennisChallengeEntities(), true);
    }

    public DbSet<TEntityType> QuerySource
    {
      get { return _dbContext.Set<TEntityType>(); }
    }

    public virtual IEnumerable<TEntityType> GetAll()
    {
      return QuerySource.ToList();
    }

    public virtual TEntityType GetEntity(object key)
    {
      var e = QuerySource.Find(new object[] { key });
      return e;
    }

    public TEntityType GetFirstOrDefaultWhereFunc(Func<TEntityType, bool> filter, params Expression<Func<TEntityType, object>>[] includes)
    {
      return GetFirstOrDefaultWhereFunc<TEntityType>(filter, includes);
    }

    public TEntityType GetFirstOrDefaultWhere(Expression<Func<TEntityType, bool>> filter, params Expression<Func<TEntityType, object>>[] includes)
    {
      return GetFirstOrDefaultWhere<TEntityType>(filter, includes);
    }

    public TDerivedType GetFirstOrDefaultWhereFunc<TDerivedType>(Func<TDerivedType, bool> filter, params Expression<Func<TDerivedType, object>>[] includes)
      where TDerivedType : TEntityType
    {
      var queryable = this.GenerateQueryable(includes);
      return queryable.FirstOrDefault(filter);
    }

    public TDerivedType GetFirstOrDefaultWhere<TDerivedType>(Expression<Func<TDerivedType, bool>> filter, params Expression<Func<TDerivedType, object>>[] includes)
      where TDerivedType : TEntityType
    {
      var queryable = this.GenerateQueryable(includes);
      return queryable.FirstOrDefault(filter);
    }

    public virtual IEnumerable<TEntityType> GetAllWhereFunc(Func<TEntityType, bool> filter, params Expression<Func<TEntityType, object>>[] includes)
    {
      var queryable = this.GenerateQueryable(includes);
      return queryable.Where(filter).ToList();
    }

    public virtual IEnumerable<TEntityType> GetAllWhere(Expression<Func<TEntityType, bool>> filter, params Expression<Func<TEntityType, object>>[] includes)
    {
      var queryable = this.GenerateQueryable(includes);
      return queryable.Where(filter).ToList();
    }

    public virtual void Add(TEntityType entity)
    {
      QuerySource.Add(entity);
      SaveChanges();
    }

    public virtual void Remove(TEntityType entity)
    {
      QuerySource.Remove(entity);
      SaveChanges();
    }

    public virtual void RemoveByKey(object key)
    {
      Remove(GetEntity(key));
    }

    public virtual void RemoveAllWhereFunc(Func<TEntityType, bool> filter, params Expression<Func<TEntityType, object>>[] includes)
    {
      foreach (var toRemove in GetAllWhereFunc(filter, includes))
        Remove(toRemove);
    }

    public virtual void RemoveAllWhere(Expression<Func<TEntityType, bool>> filter, params Expression<Func<TEntityType, object>>[] includes)
    {
      foreach (var toRemove in GetAllWhere(filter, includes))
        Remove(toRemove);
    }

    public int SaveChanges()
    {
      return _dbContext.SaveChanges();
    }

    protected IQueryable<TDerivedType> GenerateQueryable<TDerivedType>(params Expression<Func<TDerivedType, object>>[] includes) where TDerivedType : TEntityType
    {
      var queryable = QuerySource.OfType<TDerivedType>();

      foreach (var expression in includes)
      {
        queryable = queryable.Include(expression);
      }

      return queryable;
    }
  }
}
