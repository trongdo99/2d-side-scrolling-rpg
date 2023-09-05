using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations;

namespace BanhMy.Tools
{
	public static class BMAnimatorExtensions
	{
		public static bool HasParameterOfType(this Animator self, string name, AnimatorControllerParameterType type)
		{
			if (string.IsNullOrEmpty(name)) return false;
			AnimatorControllerParameter[] parameters = self.parameters;
			foreach (AnimatorControllerParameter curParam in parameters)
			{
				if (curParam.type == type && curParam.name == name)
				{
					return true;
				}
			}
			return false;
		}

		public static void AddAnimatorParameterIfExists(this Animator self, string parameterName, out int parameter, AnimatorControllerParameterType type, HashSet<int> parameterList)
		{
			if (string.IsNullOrEmpty(parameterName))
			{
				parameter = -1;
				return;
			}

			parameter = Animator.StringToHash(parameterName);
			
			if (self.HasParameterOfType(parameterName, type))
			{
				parameterList.Add(parameter);
			}
		}

		public static bool UpdateAnimatorBool(this Animator self, int parameter, bool value, HashSet<int> parameterList, bool performSanityCheck = true)
		{
			if (performSanityCheck && !parameterList.Contains(parameter)) return false;
			self.SetBool(parameter, value);
			return true;
		}

		public static bool UpdateAnimatorTrigger(this Animator self, int parameter, HashSet<int> parameterList, bool performSanityCheck = true)
		{
			if (performSanityCheck && !parameterList.Contains(parameter)) return false;
			self.SetTrigger(parameter);
			return true;
		}

		public static bool UpdateAnimtorInteger(this Animator self, int parameter, int value, HashSet<int> parameterList, bool performSanityCheck = true)
		{
			if (performSanityCheck && !parameterList.Contains(parameter)) return false;
			self.SetInteger(parameter, value);
			return true;
		}

		public static bool UpdateAnimatorFloat(this Animator self, int parameter, float value, HashSet<int> parameterList, bool performSanityCheck = true)
		{
			if (performSanityCheck && !parameterList.Contains(parameter)) return false;
			self.SetFloat(parameter, value);
			return true;
		}
	}
}
