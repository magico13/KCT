using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace KerbalConstructionTime
{
    public static class TypeExtensions
    {
        public static T GetPublicStaticValue<T>(this Type type, string name) where T : class
        {
            return (T)KCT_Utilities.GetMemberInfoValue(type.GetMember(name, BindingFlags.Public | BindingFlags.Static).FirstOrDefault(), null);
        }

        public static T GetPublicValue<T>(this Type type, string name, object instance) where T : class
        {
            return (T)KCT_Utilities.GetMemberInfoValue(type.GetMember(name, BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy).FirstOrDefault(), instance);
        }

        public static T GetPrivateMemberValue<T>(this Type type, string name, object instance, int index = -1)
        {
            BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy;
            object value = KCT_Utilities.GetMemberInfoValue(type.GetMember(name, flags).FirstOrDefault(), instance);
            if (value != null)
            {
                return (T)value;
            }
            
            KCTDebug.Log($"Could not get value by name '{name}', getting by index '{index}'");
            if (index >= 0)
            {
                List<MemberInfo> members = type.GetMembers(flags).Where(m => m.ToString().Contains(typeof(T).ToString())).ToList();
                if (members.Count > index)
                {
                    return (T)KCT_Utilities.GetMemberInfoValue(members[index], instance);
                }
            }
            throw new Exception($"No members of type '{typeof(T)}' found for name '{name}' at index '{index}' for type '{type}'");
        }

        public static object GetPrivateMemberValue(this Type type, string name, object instance, int index = -1)
        {
            BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy;
            object value = KCT_Utilities.GetMemberInfoValue(type.GetMember(name, flags).FirstOrDefault(), instance);
            if (value != null)
            {
                return value;
            }

            KCTDebug.Log($"Could not get value by name '{name}', getting by index '{index}'");
            if (index >= 0)
            {
                List<MemberInfo> members = type.GetMembers(flags).ToList();
                if (members.Count > index)
                {
                    return KCT_Utilities.GetMemberInfoValue(members[index], instance);
                }
            }
            throw new Exception($"No members found for name '{name}' at index '{index}' for type '{type}'");
        }
    }
}
