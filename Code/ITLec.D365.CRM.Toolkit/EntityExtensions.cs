using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITLec.D365.CRM.Toolkit
{
    public static class EntityExtensions
    {
        public static void SetAttribute(this Entity entity, string attributeName, object attribute)
        {
            if (entity.Attributes.Contains(attributeName))
            {
                entity.Attributes.Remove(attributeName);
            }
            entity.Attributes.Add(attributeName, attribute);
        }

        public static int GetValueOrDefault(this OptionSetValue value, int defaultValue)
        {
            if (value == null)
            {
                return defaultValue;
            }
            return value.Value;
        }

        public static decimal GetValueOrDefault(this Money money, decimal defaultValue)
        {
            if (money == null)
            {
                return defaultValue;
            }
            return money.Value;
        }

        public static Money Add(this Money a, Money b)
        {
            return new Money(a.GetValueOrDefault(decimal.Zero) + b.GetValueOrDefault(decimal.Zero));
        }

        public static string GetName(this EntityReference reference)
        {
            if (reference == null || reference.Name == null)
            {
                return string.Empty;
            }
            return reference.Name;
        }

        public static void CopyAttributes(this Entity toEntity, Entity fromEntity)
        {
            EntityExtensions.CombineAttributes(toEntity.Attributes, fromEntity.Attributes, true);
        }

        public static void MergeAttributes(this Entity toEntity, Entity fromEntity)
        {            EntityExtensions.CombineAttributes(toEntity.Attributes, fromEntity.Attributes, false);
        }

        private static void CombineAttributes(AttributeCollection toColl, AttributeCollection fromColl, bool destructive)
        {
            foreach (KeyValuePair<string, object> current in fromColl)
            {
                if (toColl.Contains(current.Key))
                {
                    if (destructive)
                    {
                        toColl[current.Key] =  current.Value;
                    }
                }
                else
                {
                    toColl.Add(current.Key, current.Value);
                }
            }
        }

        public static TValue GetAliasedAttributeValue<TValue>(this Entity me, string attributeKey)
        {
            AliasedValue attributeValue = me.GetAttributeValue<AliasedValue>(attributeKey);
            if (attributeValue == null)
            {
                return default(TValue);
            }
            return (TValue)((object)attributeValue.Value);
        }

        public static Entity WithoutAttribute(this Entity entity, string attributeToRemove)
        {
            if (entity.Attributes.Contains(attributeToRemove))
            {
                entity.Attributes.Remove(attributeToRemove);
            }
            return entity;
        }

        public static Entity WithoutAttribute(this Entity entity, params string[] attributesToRemove)
        {
            for (int i = 0; i < attributesToRemove.Length; i++)
            {
                string attributeToRemove = attributesToRemove[i];
                entity = entity.WithoutAttribute(attributeToRemove);
            }
            return entity;
        }
    }
}
