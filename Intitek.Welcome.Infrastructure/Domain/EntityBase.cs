using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Intitek.Welcome.Infrastructure.Domain
{
    public abstract class EntityBase<TId>
    {
        private readonly List<BusinessRule> _brokenRules = new List<BusinessRule>();

        [NotMapped]
        public TId Id { get; set; }
        protected abstract void Validate();

        public IEnumerable<BusinessRule> GetBrokenRules()
        {
            _brokenRules.Clear();
            Validate();
            return _brokenRules;
        }

        protected void AddBrokenRule(BusinessRule businessRule)
        {
            _brokenRules.Add(businessRule);
        }

        public override bool Equals(object obj)
        {
            return obj != null
                   && obj is EntityBase<TId>
                   && this == (EntityBase<TId>)obj;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public static bool operator ==(EntityBase<TId> entity1, EntityBase<TId> entity2)
        {
            if ((object) entity1 == null && (object) entity2 == null)
            {
                return true;
            }

            if ((object) entity1 == null || (object) entity2 == null)
            {
                return false;
            }

            if (entity1.Id.ToString() == entity2.Id.ToString())
            {
                return true;
            }

            return false;
        }

        public static bool operator !=(EntityBase<TId> entity1, EntityBase<TId> entity2)
        {
            return (!(entity1 == entity2));
        }
    }
}