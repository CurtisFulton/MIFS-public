using Microsoft.Extensions.DependencyInjection;
using System;

namespace Mifs
{
    /// <summary>
    /// Creates simple factory that generates an entity of type TEntity
    /// </summary>
    /// <typeparam name="TEntity">Entity Type to create</typeparam>
    public interface IFactory<TEntity> where TEntity : class
    {
        TEntity? Create();
    }

    /// <summary>
    /// Default Implementation of the IFactory<>.
    /// Will use the service provider to create instances of TEntity
    /// </summary>
    /// <typeparam name="TEntity">Entity Type to create</typeparam>
    internal class GenericFactory<TEntity> : IFactory<TEntity>
        where TEntity : class
    {
        public GenericFactory(IServiceProvider serviceProvider)
        {
            this.ServiceProvider = serviceProvider;
        }

        private IServiceProvider ServiceProvider { get; }

        public TEntity? Create()
            => this.ServiceProvider.GetService<TEntity>();
    }
}
