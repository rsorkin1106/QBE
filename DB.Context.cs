﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace QBE
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.Core.Objects;
    using System.Linq;
    
    public partial class TestEntities1 : DbContext
    {
        public TestEntities1()
            : base("name=TestEntities1")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Customer> Customers { get; set; }
        public virtual DbSet<Address> Addresses { get; set; }
        public virtual DbSet<QueriesTable> QueriesTables { get; set; }
        public virtual DbSet<Order> Orders { get; set; }
        public virtual DbSet<vw_allData> vw_allData { get; set; }
    
        [DbFunction("TestEntities1", "locateAllOrders")]
        public virtual IQueryable<locateAllOrders_Result> locateAllOrders()
        {
            return ((IObjectContextAdapter)this).ObjectContext.CreateQuery<locateAllOrders_Result>("[TestEntities1].[locateAllOrders]()");
        }
    
        public virtual int sp_changeAddress(string ad1, string ad2, string town, string state, string zip, Nullable<int> id)
        {
            var ad1Parameter = ad1 != null ?
                new ObjectParameter("ad1", ad1) :
                new ObjectParameter("ad1", typeof(string));
    
            var ad2Parameter = ad2 != null ?
                new ObjectParameter("ad2", ad2) :
                new ObjectParameter("ad2", typeof(string));
    
            var townParameter = town != null ?
                new ObjectParameter("town", town) :
                new ObjectParameter("town", typeof(string));
    
            var stateParameter = state != null ?
                new ObjectParameter("state", state) :
                new ObjectParameter("state", typeof(string));
    
            var zipParameter = zip != null ?
                new ObjectParameter("zip", zip) :
                new ObjectParameter("zip", typeof(string));
    
            var idParameter = id.HasValue ?
                new ObjectParameter("id", id) :
                new ObjectParameter("id", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("sp_changeAddress", ad1Parameter, ad2Parameter, townParameter, stateParameter, zipParameter, idParameter);
        }
    
        public virtual int sp_changeCustomer(string name, string code, string contact, Nullable<int> id)
        {
            var nameParameter = name != null ?
                new ObjectParameter("name", name) :
                new ObjectParameter("name", typeof(string));
    
            var codeParameter = code != null ?
                new ObjectParameter("code", code) :
                new ObjectParameter("code", typeof(string));
    
            var contactParameter = contact != null ?
                new ObjectParameter("contact", contact) :
                new ObjectParameter("contact", typeof(string));
    
            var idParameter = id.HasValue ?
                new ObjectParameter("id", id) :
                new ObjectParameter("id", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("sp_changeCustomer", nameParameter, codeParameter, contactParameter, idParameter);
        }
    
        public virtual int sp_insertLog(Nullable<int> level, string msg)
        {
            var levelParameter = level.HasValue ?
                new ObjectParameter("level", level) :
                new ObjectParameter("level", typeof(int));
    
            var msgParameter = msg != null ?
                new ObjectParameter("msg", msg) :
                new ObjectParameter("msg", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("sp_insertLog", levelParameter, msgParameter);
        }
    
        public virtual int sp_viewAll()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("sp_viewAll");
        }
    }
}
