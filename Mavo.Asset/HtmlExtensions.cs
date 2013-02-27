using System.Linq;
using System.ComponentModel;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Linq.Expressions;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Web.Script.Serialization;
using System.Text;
using System.Web;
using Inflector;
using System.ComponentModel.DataAnnotations;
namespace System.Web.Mvc
{
    public static class HtmlExtensions
    {
        private enum TimeSpanElement
        {
            Millisecond,
            Second,
            Minute,
            Hour,
            Day
        }

        public static MvcHtmlString DisplayNameTitleizedFor<T, TResult>(this HtmlHelper<T> helper, Expression<Func<T, TResult>> expression)
        {
            string propertyName = ExpressionHelper.GetExpressionText(expression);

            if (propertyName.IndexOf(".") > 0)
            {
                propertyName = propertyName.Substring(propertyName.LastIndexOf(".") + 1);
            }

            string labelValue = ModelMetadata.FromLambdaExpression(expression, helper.ViewData).DisplayName;

            if (string.IsNullOrEmpty(labelValue))
            {
                labelValue = Inflector.Inflector.Titleize(propertyName);
            }

            return MvcHtmlString.Create(labelValue);
        }

        public static MvcHtmlString LabelTitleizeFor<T, TResult>(this HtmlHelper<T> helper, Expression<Func<T, TResult>> expression)
        {
            string propertyName = ExpressionHelper.GetExpressionText(expression);
            var me = expression.Body as MemberExpression;
            var attr = me.Member
                 .GetCustomAttributes(typeof(DisplayAttribute), false)
                 .Cast<DisplayAttribute>()
                 .SingleOrDefault();

            if (propertyName.IndexOf(".") > 0)
            {
                propertyName = propertyName.Substring(propertyName.LastIndexOf(".") + 1);
            }
            string labelValue = null;
            if (attr != null)
            {
                if (attr.Name != null)
                    labelValue = attr.Name;
                else
                    labelValue = attr.Description;
            }
            else
                labelValue = ModelMetadata.FromLambdaExpression(expression, helper.ViewData).DisplayName;

            if (string.IsNullOrEmpty(labelValue))
            {
                labelValue = Inflector.Inflector.Titleize(propertyName);
            }

            string label = String.Format("<label for=\"{0}\" class='control-label'>{1}</label>", ExpressionHelper.GetExpressionText(expression), labelValue);
            return MvcHtmlString.Create(label);
        }

        public static string ToRelative(this TimeSpan timeSpan, int maxNrOfElements = 5)
        {
            maxNrOfElements = Math.Max(Math.Min(maxNrOfElements, 5), 1);
            var parts = new[]
                        {
                            Tuple.Create(TimeSpanElement.Day, timeSpan.Days),
                            Tuple.Create(TimeSpanElement.Hour, timeSpan.Hours),
                            Tuple.Create(TimeSpanElement.Minute, timeSpan.Minutes),
                            Tuple.Create(TimeSpanElement.Second, timeSpan.Seconds)
                        }
                                        .SkipWhile(i => i.Item2 <= 0)
                                        .Take(maxNrOfElements);

            return string.Join(", ", parts.Select(p => string.Format("{0} {1}{2}", p.Item2, p.Item1, p.Item2 > 1 ? "s" : string.Empty)));
        }
        public static string GetActive(string request, object target)
        {
            if (target == null)
                return String.Empty;
            if (!String.IsNullOrEmpty(request) && request == target.ToString())
                return "class=active";
            else
                return String.Empty;
        }
        public static MvcHtmlString TypeaheadFor<TModel, TValue>(
        this HtmlHelper<TModel> htmlHelper,
        Expression<Func<TModel, TValue>> expression,
        IEnumerable<string> source,
        int items = 8)
        {
            if (expression == null)
                throw new ArgumentNullException("expression");

            if (source == null)
                throw new ArgumentNullException("source");

            var jsonString = new JavaScriptSerializer().Serialize(source);

            return htmlHelper.TextBoxFor(
                expression,
                new
                {
                    autocomplete = "off",
                    data_provide = "typeahead",
                    data_items = items,
                    data_source = jsonString
                }
            );
        }
        public static MvcHtmlString ClientIdFor<TModel, TProperty>(
            this HtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TProperty>> expression)
        {
            return MvcHtmlString.Create(
                  htmlHelper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldId(
                      ExpressionHelper.GetExpressionText(expression)));
        }

        public static string GetInputName<TModel, TProperty>(Expression<Func<TModel, TProperty>> expression)
        {
            if (expression.Body.NodeType == ExpressionType.Call)
            {
                MethodCallExpression methodCallExpression = (MethodCallExpression)expression.Body;
                string name = GetInputName(methodCallExpression);
                return name.Substring(expression.Parameters[0].Name.Length + 1);

            }
            return expression.Body.ToString().Substring(expression.Parameters[0].Name.Length + 1);
        }

        private static string GetInputName(MethodCallExpression expression)
        {
            // p => p.Foo.Bar().Baz.ToString() => p.Foo OR throw...
            MethodCallExpression methodCallExpression = expression.Object as MethodCallExpression;
            if (methodCallExpression != null)
            {
                return GetInputName(methodCallExpression);
            }
            return expression.Object.ToString();
        }

        public static IHtmlString EnumCheckBoxListFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, bool? disabled = null) where TModel : class
        {
            var value = htmlHelper.ViewData.Model == null
              ? default(TProperty)
              : expression.Compile()(htmlHelper.ViewData.Model);

            StringBuilder sb = new StringBuilder();

            string inputName = GetInputName(expression);

            foreach (TProperty item in Enum.GetValues(typeof(TProperty)).Cast<TProperty>())
            {
                TagBuilder builder = new TagBuilder("input");
                long targetValue = Convert.ToInt64(item);
                long flagValue = Convert.ToInt64(value);

                if ((targetValue & flagValue) == targetValue)
                    builder.MergeAttribute("checked", "checked");
                //<label class="checkbox">
                //  <input type="checkbox"> Check me out
                //</label>
                builder.MergeAttribute("type", "checkbox");
                builder.MergeAttribute("value", targetValue.ToString());
                builder.MergeAttribute("name", inputName);
                if (disabled.HasValue && disabled.Value)
                    builder.MergeAttribute("disabled", "disabled");
                builder.InnerHtml = item.ToString().Titleize();

                sb.Append(String.Format("<label class='checkbox'>{0}</label>", builder.ToString(TagRenderMode.Normal)));
            }

            return new HtmlString(sb.ToString());
        }

        public static MvcHtmlString EnumDropDownListFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, string firstElement = null) where TModel : class
        {
            string inputName = GetInputName(expression);
            var value = htmlHelper.ViewData.Model == null
                ? default(TProperty)
                : expression.Compile()(htmlHelper.ViewData.Model);

            return htmlHelper.DropDownList(inputName, ToSelectList(typeof(TProperty), value.ToString(), firstElement));
        }

        public static SelectList ToSelectList(Type enumType, string selectedItem, string firstItem)
        {
            List<SelectListItem> items = new List<SelectListItem>();
            if (!String.IsNullOrEmpty(firstItem))
            {
                items.Add(new SelectListItem() { Text = firstItem });
            }
            foreach (var item in Enum.GetValues(enumType))
            {
                FieldInfo fi = enumType.GetField(item.ToString());
                var attribute = fi.GetCustomAttributes(typeof(DescriptionAttribute), true).FirstOrDefault();
                var title = attribute == null ? item.ToString() : ((DescriptionAttribute)attribute).Description;
                var listItem = new SelectListItem
                {
                    Value = ((int)item).ToString().Titleize(),
                    Text = title,
                    Selected = selectedItem == ((int)item).ToString()
                };
                items.Add(listItem);
            }

            return new SelectList(items, "Value", "Text", selectedItem);
        }

        /// <summary>
        /// Check to see if a flags enumeration has a specific flag set.
        /// </summary>
        /// <param name="variable">Flags enumeration to check</param>
        /// <param name="value">Flag to check for</param>
        /// <returns></returns>
        public static bool HasFlag(this Enum variable, Enum value)
        {
            if (variable == null)
                return false;

            if (value == null)
                throw new ArgumentNullException("value");

            // Not as good as the .NET 4 version of this function, but should be good enough
            if (!Enum.IsDefined(variable.GetType(), value))
            {
                throw new ArgumentException(string.Format(
                    "Enumeration type mismatch.  The flag is of type '{0}', was expecting '{1}'.",
                    value.GetType(), variable.GetType()));
            }

            ulong num = Convert.ToUInt64(value);
            return ((Convert.ToUInt64(variable) & num) == num);

        }
    }
}