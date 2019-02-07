using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace client.Data
{
	public static class BindingHelper
	{
		public static void ClearBinding(this Control value)
		{
			value.DataBindings.Clear();
			foreach (var c in value.Controls)
			{
				var child = c as Control;
				if (child != null)
					child.ClearBinding();
			}
		}

		public static Binding AddBinding<TObject, TControl>(this TControl control, Expression<Func<TControl, object>> controlProperty, TObject value, Expression<Func<TObject, object>> property)
			where TControl : Control
		{
			var b = new Binding(GetName(controlProperty), value, GetName(property), false, DataSourceUpdateMode.OnValidation);
			control.DataBindings.Add(b);
			return b;
		}

		/// <summary>
		/// http://stackoverflow.com/questions/671968/retrieving-property-name-from-lambda-expression
		/// </summary>
		/// <param name="exp"></param>
		/// <returns></returns>
		public static string GetName<T>(Expression<Func<T, object>> exp)
		{
			string name = "";
			MemberExpression body = exp.Body as MemberExpression;
			if (body == null)
			{
				UnaryExpression ubody = (UnaryExpression)exp.Body;
				body = ubody.Operand as MemberExpression;
				name = body.Member.Name;
			}
			else
			{
				name = body.Member.Name;
			}
			return name;
		}
	}
}
