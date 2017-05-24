// Made by Felipe "Tidão" Almeida
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Linq.Expressions;

public class VoxJSON
{
    public static string ToJSON(bool p_showClassName, params object[] p_data)
    {
        if (p_data == null)
            return string.Empty;
        string __stringJSON = "{";

            for (int i = 0;i < p_data.Length;i++)
            {
                bool __isGenericClass = CheckIfIsGenericClass(p_data[i]);
                if (__isGenericClass == true)
                {
                    if (p_showClassName == true)
                    {
                        __stringJSON += "\"" + p_data[i].ToString() + "\": {";
    
                }
                    FieldInfo[] __fieldInfoArray = p_data[i].GetType().GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic);

                    int __fieldInfoArrayIndex = 0;
                    foreach (FieldInfo __fieldInfo in __fieldInfoArray)
                    {
                        if (__fieldInfo.GetValue(p_data[i]) is IList)
                        {
                            IList __list = (IList)__fieldInfo.GetValue(p_data[i]);
                            __stringJSON += "\"" + __fieldInfo.Name + "\":" +  RecursiveListBuilder(__list);
    
                    }
                        else if (__fieldInfo.GetValue(p_data[i]) is IDictionary)
                        {
                            IDictionary __iDict = (IDictionary)__fieldInfo.GetValue(p_data[i]);
                            __stringJSON += "\"" + __fieldInfo.Name + "\":" +  RecursiveDictionaryBuilder(__iDict);
    
                    }
                        else if (__fieldInfo.GetValue(p_data[i]) is ICollection)
                        {
                            ICollection __iCollection = (ICollection)__fieldInfo.GetValue(p_data[i]);
                            __stringJSON += "\"" + __fieldInfo.Name + "\":" +  RecursiveCollectionBuilder(__iCollection);
    
                    }
                        else
                        {
                            string __value = (__fieldInfo.GetValue(p_data[i]) != null) ? __fieldInfo.GetValue(p_data[i]).ToString() : "";
                            __stringJSON += "\"" + __fieldInfo.Name + "\":\"" + __value + "\"";
    
                    }

                        if (__fieldInfoArrayIndex < __fieldInfoArray.Length - 1)
                        {
                            __stringJSON += ",";
                        }
                        __fieldInfoArrayIndex++;
                    }
                    if (p_showClassName == true)
                    {
                        __stringJSON += (i < p_data.Length - 1) ? "}," : "}";
            }
        }
			else
			{
            __stringJSON += "\"" + p_data[i].GetType().Name + "\":\"" + p_data[i].ToString() + "\"";

                __stringJSON += (i < p_data.Length - 1) ? "," : "";
        }
    }

    __stringJSON += "}";

		return __stringJSON;
	}

	private static bool CheckIfIsGenericClass(object p_object)
{
    bool __isGenericClass;
    if (p_object.GetType().Assembly.GetName().Name == "mscorlib" && (p_object is IList) == false) 
		{
        __isGenericClass = false;
    }
		else
		{
        __isGenericClass = true;
    }
    return __isGenericClass;
}

private static string RecursiveListBuilder(IList p_list)
{
    string __listJson = "[";

		for (int i = 0; i<p_list.Count; i ++)
		{
			if (p_list[i] is IList)
			{

            __listJson += RecursiveListBuilder((IList)p_list[i]);
			}
			else if (p_list[i] is IDictionary)
			{
				__listJson += RecursiveDictionaryBuilder((IDictionary)p_list[i]);
			}
			else
			{
				if (p_list[i] != null)
				{
					if (CheckIfIsGenericClass(p_list[i]) == true)
					{
						__listJson += ToJSON(true, p_list[i]);
					}
					else
					{
						__listJson += "\"" + p_list[i].ToString() + "\"";
					}
				}
				else
				{
					__listJson += "\"\"";

				}
			}
			if ( i<p_list.Count -1)
			{
				__listJson += ",";
			}
		}

		__listJson += "]";
		return __listJson;
	}

	private static string RecursiveCollectionBuilder(ICollection p_collection)
{
    string __collectionJson = "[";
		int __collectionIndex = 0;
		foreach(var collection in p_collection)
		{
			if (collection is IList)
			{

            __collectionJson += RecursiveListBuilder((IList)collection);
			}
			else if (collection is IDictionary)
			{
				__collectionJson += RecursiveDictionaryBuilder((IDictionary)collection);
			}
			else if (collection is ICollection)
			{
				__collectionJson += RecursiveCollectionBuilder((ICollection)collection);
			}
			else
			{
				if (collection != null)
				{
					if (CheckIfIsGenericClass(collection) == true)
					{
						__collectionJson += ToJSON(true, collection);
					}
					else
					{
						__collectionJson += "\"" + collection + "\"";
					}
				}
				else
				{
					__collectionJson += "\"\"";

				}
			}
			if ( __collectionIndex<p_collection.Count -1)
			{
				__collectionJson += ",";
			}
			__collectionIndex++;
		}

		__collectionJson += "]";
		return __collectionJson;
	}

	private static string RecursiveDictionaryBuilder(IDictionary p_dict)
{
    string __dictJson = "{";
        int __dictIndex = 0;
        foreach (DictionaryEntry entry in p_dict)
        {
            if (entry.Value is IDictionary)
            {
                __dictJson += "\"" + entry.Key + "\":\"" + RecursiveDictionaryBuilder((IDictionary)entry.Value);

            }
            else if (entry.Value is IList)
            {
                __dictJson += "\"" + entry.Key + "\":" + RecursiveListBuilder((IList)entry.Value);

            }
            else
            {
                if (entry.Value != null)
                {
                    if (CheckIfIsGenericClass(entry.Value))
                    {
                        __dictJson += ToJSON(true, entry.Value);
                    }
                    else
                    {
                        __dictJson += "\"" + entry.Key + "\":\"" + entry.Value.ToString() + "\"";

                    }
                }
                else
                {
                    __dictJson += "\"" + entry.Key + "\":\"\"";

                }
            }

            if (__dictIndex < p_dict.Count - 1)
            {
                __dictJson += ",";
            }
            __dictIndex++;
        }
        __dictJson += "}";
    return __dictJson;
}
}