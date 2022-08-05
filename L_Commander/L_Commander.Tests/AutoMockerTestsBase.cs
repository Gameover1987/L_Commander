using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.AutoMock;

namespace L_Commander.Tests
{
	[TestClass]
    public class AutoMockerTestsBase<TTarget> where TTarget : class
    {
        private TTarget _target;

        /// <summary>
        /// Помечен атрибутом SetUp вызывается перед каждым тестом.
        /// </summary>
        [TestInitialize()]
        public virtual void SetUp()
        {
            Mocker = new AutoMocker();
        }

        /// <summary>
        /// Помечен атрибутом TearDown вызывается после каждого теста.
        /// </summary>
        [TestCleanup]
        public virtual void TearDown()
        {
            var disposable = _target as IDisposable;
            if (disposable != null)
                disposable.Dispose();
        }

        protected AutoMocker Mocker { get; private set; }

        protected TTarget Target
        {
            get
            {
                return _target ?? (_target = DirectCreateTarget());
            }
        }

        protected T CreateInstance<T>() where T : class
        {
            return Mocker.CreateInstance<T>();
        }

        /// <summary>
        /// Создание Target, можно переопределить если необходимо или использовать когда сама ссылка на Target не нужна
        /// </summary>
        /// <returns></returns>
        protected virtual TTarget DirectCreateTarget()
        {
            return CreateInstance<TTarget>();
        }

        protected T Get<T>() where T : class
        {
            try
            {
                return Mocker.Get<T>();
            }
            catch
            {
                //Если это не получилось, то пытаетмся создать для этого мок и получить от него объект
                return GetMock<T>().Object;
            }
        }

        protected Mock<T> GetMock<T>() where T : class
        {
            return Mocker.GetMock<T>();
        }

        public void Use<TService>(TService service)
        {
            Mocker.Use(service);
        }

        public void Use<TService>(Mock<TService> mockedService) where TService : class
        {
            Mocker.Use(mockedService);
        }

        public void Use<TService>(Expression<Func<TService, bool>> setup) where TService : class
        {
            Mocker.Use(setup);
        }
    }
}
