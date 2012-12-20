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

namespace Mavo.Assets
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
                    Value = ((int)item).ToString(),
                    Text = title,
                    Selected = selectedItem == ((int)item).ToString()
                };
                items.Add(listItem);
            }

            return new SelectList(items, "Value", "Text", selectedItem);
        }
    }
}