﻿using System;
using System.Collections.Generic;
using System.Linq;
using Embeddinator;
using IKVM.Reflection;
using Type = IKVM.Reflection.Type;

namespace ObjC {
	public static class NameGenerator {
		public static Dictionary<string, string> ObjCTypeToArgument = new Dictionary<string, string> {
			{ "int", "anInt" },
			{ "unsigned int", "aUint" },
			{ "double", "aDouble" },
			{ "float", "aFloat" },
			{ "NSString", "aString" },
			{ "NSString *", "aString" },
			{ "id", "anObject" },
			{ "NSObject", "anObject" },
			{ "NSPoint", "aPoint" },
			{ "NSRect", "aRect" },
			{ "NSFont", "fontObj" },
			{ "SEL", "aSelector" },
			{ "short", "aShort" },
			{ "unsigned short", "aUshort" },
			{ "long long", "aLong" },
			{ "unsigned long long", "aUlong" },
			{ "bool", "aBool" },
			{ "char", "aChar" },
			{ "unsigned char", "aChar" },
			{ "signed char", "aChar" }
		};

		public static string GetObjCName (Type t)
		{
			return t.FullName.Replace ('.', '_');
		}

		// TODO complete mapping (only with corresponding tests)
		// TODO override with attribute ? e.g. [Obj.Name ("XAMType")]
		public static string GetTypeName (Type t)
		{
			if (t.IsByRef) {
				var et = t.GetElementType ();
				return GetTypeName (et) + (et.IsValueType ? " " : " _Nonnull ") + "* _Nullable";
			}

			if (t.IsEnum)
				return GetObjCName (t);

			switch (Type.GetTypeCode (t)) {
			case TypeCode.Object:
				switch (t.Namespace) {
				case "System":
					switch (t.Name) {
					case "Object":
					case "ValueType":
						return "NSObject";
					case "Void":
						return "void";
					default:
						return GetObjCName (t);
					}
				default:
					return GetObjCName (t);
				}
			case TypeCode.Boolean:
				return "bool";
			case TypeCode.Char:
				return "unsigned short";
			case TypeCode.Double:
				return "double";
			case TypeCode.Single:
				return "float";
			case TypeCode.Byte:
				return "unsigned char";
			case TypeCode.SByte:
				return "signed char";
			case TypeCode.Int16:
				return "short";
			case TypeCode.Int32:
				return "int";
			case TypeCode.Int64:
				return "long long";
			case TypeCode.UInt16:
				return "unsigned short";
			case TypeCode.UInt32:
				return "unsigned int";
			case TypeCode.UInt64:
				return "unsigned long long";
			case TypeCode.String:
				return "NSString *";
			default:
				throw new NotImplementedException ($"Converting type {t.Name} to a native type name");
			}
		}

		public static string GetMonoName (Type t)
		{
			if (t.IsByRef)
				return GetMonoName (t.GetElementType ()) + "&";

			if (t.IsEnum)
				return t.FullName;

			switch (Type.GetTypeCode (t)) {
			case TypeCode.Object:
				switch (t.Namespace) {
				case "System":
					switch (t.Name) {
					case "Void":
						return "void";
					default:
						return "object";
					}
				default:
					return t.FullName;
				}
			case TypeCode.Boolean:
				return "bool";
			case TypeCode.Char:
				return "char";
			case TypeCode.Double:
				return "double";
			case TypeCode.Single:
				return "single";
			case TypeCode.Byte:
				return "byte";
			case TypeCode.SByte:
				return "sbyte";
			case TypeCode.Int16:
				return "int16";
			case TypeCode.Int32:
				return "int";
			case TypeCode.Int64:
				return "long";
			case TypeCode.UInt16:
				return "uint16";
			case TypeCode.UInt32:
				return "uint";
			case TypeCode.UInt64:
				return "ulong";
			case TypeCode.String:
				return "string";
			default:
				throw new NotImplementedException ($"Converting type {t.Name} to a mono type name");
			}
		}

		public static string GetExtendedParameterName (ParameterInfo p, ParameterInfo [] parameters)
		{
			string pName = p.Name;
			string ptname = GetTypeName (p.ParameterType);
			if (p.Name.Length < 3) {
				if (!ObjCTypeToArgument.TryGetValue (ptname, out pName))
					pName = "anObject";

				if (parameters.Count (p2 => GetTypeName (p2.ParameterType) == ptname && p2.Name.Length < 3) > 1 ||
					pName == "anObject" && parameters.Count (p2 => !ObjCTypeToArgument.ContainsKey (GetTypeName (p2.ParameterType))) > 1)
					pName += p.Name.PascalCase ();
			}

			return pName;
		}

		public static string GetObjCParamTypeName (ParameterInfo param, List<ProcessedType> allTypes)
		{
			Type pt = param.ParameterType;
			string ptname = GetTypeName (pt);
			if (pt.IsInterface)
				ptname = $"id<{ptname}>";
			if (allTypes.Contains (pt))
				ptname += " *";
			return ptname;
		}
	}
}
