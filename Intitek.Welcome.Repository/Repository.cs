using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Linq.Dynamic;
using Intitek.Welcome.Domain;
using Intitek.Welcome.Infrastructure.Domain;
using Intitek.Welcome.Infrastructure.Specification;
using Intitek.Welcome.Infrastructure.UnitOfWork;



namespace Intitek.Welcome.Repository
{
    public abstract class Repository<T, TId> : IRepository<T, TId> where T : EntityBase<TId>, IAggregateRoot
    {
        private readonly IQueryable<T> _repositoryQuery;
        private readonly DbSet<T> _repositoryTable;
        protected Infrastructure.Config.Config config;
        protected IUnitOfWork UnitOfWork { get; set; }

        protected Repository(IUnitOfWork uow)
        {
            if (uow == null) throw new ArgumentNullException("uow");
            UnitOfWork = uow;

            _repositoryTable = GetDbSet<T>();
            _repositoryQuery = _repositoryTable.AsQueryable();
            //this.Context.Configuration.LazyLoadingEnabled = false;
            Context.Configuration.ProxyCreationEnabled = false;
        }

        protected WelcomeDB Context
        {
            get { return (WelcomeDB)UnitOfWork; }
        }

      

        protected DbSet<TEntity> GetDbSet<TEntity>() where TEntity : class
        {
            return Context.Set<TEntity>();
        }

        public IQueryable<T> RepositoryQuery
        {
            get { return _repositoryQuery; }
        }
        public DbSet<T> RepositoryTable
        {
            get { return _repositoryTable; }
        }



        // Constructor method of the class

        public int Count()
        {
            return _repositoryTable.Count();
        }


        public int Count(ISpecification<T> specification)
        {
            return _repositoryQuery.Where(specification.Predicate).Count();
        }

        public void Add(T entity)
        {
            _repositoryTable.Add(entity);
            UnitOfWork.SaveChanges();
        }

        public void Remove(T entity)
        {
            _repositoryTable.Remove(entity);
            UnitOfWork.SaveChanges();
        }
        public void RemoveAll()
        {
            foreach (T entity in RepositoryTable)
            {
                SetEntityState(entity, EntityState.Deleted);

            }

            UnitOfWork.SaveChanges();
        }

        public void RemoveAll(IEnumerable<T> entities)
        {
            foreach (T entity in entities)
            {
                SetEntityState(entity, EntityState.Deleted);

            }

            UnitOfWork.SaveChanges();
        }

        public void UpdateAll(IEnumerable<T> entities)
        {
            foreach (T entity in entities)
            {
                SetEntityState(entity, entity.Id.Equals(default(TId)) ? EntityState.Added : EntityState.Modified);

            }

            UnitOfWork.SaveChanges();
        }

        public void Save(T entity)
        {
            SetEntityState(entity, entity.Id.Equals(default(TId)) ? EntityState.Added : EntityState.Modified);
            UnitOfWork.SaveChanges();

        }

        public void Save(T entity, TId key1, TId key2)
        {
            SetEntityState(entity, entity.Id.Equals(default(TId)) ? EntityState.Added : EntityState.Modified);
            UnitOfWork.SaveChanges();

        }

        public T FindBy(TId id)
        {
            return _repositoryTable.Find(id);
        }

        public IEnumerable<T> FindAll()
        {
            return _repositoryQuery.AsEnumerable();
        }

        public IEnumerable<T> FindAll(string orderBy, int index, int count)
        {
            return _repositoryQuery.OrderBy(orderBy).Skip(index * count).Take(count).AsEnumerable();
        }

        public IEnumerable<T> FindBy(ISpecification<T> specification)
        {
            return _repositoryQuery.Where(specification.Predicate);
        }

        public IEnumerable<T> FindBy(ISpecification<T> specification, string orderBy, int index, int count)
        {
            return _repositoryQuery.Where(specification.Predicate).OrderBy(orderBy).Skip(index * count).Take(count).AsEnumerable();
        }

        public IEnumerable<T> OrderBy(ISpecification<T> specification)
        {
            return _repositoryQuery.OrderBy(specification.Predicate).AsEnumerable();
        }

        public IEnumerable<T> OrderByDescending(ISpecification<T> specification)
        {
            return _repositoryQuery.OrderByDescending(specification.Predicate).AsEnumerable();
        }

        protected void SetEntityState(object entity, EntityState entityState)
        {
            Context.Entry(entity).State = entityState;
        }



    }
}
