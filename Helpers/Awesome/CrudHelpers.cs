using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Omu.Awem.Helpers;
using Omu.AwesomeMvc;

namespace AweRazorPages.Helpers.Awesome
{
    public static class CrudHelpers
    {
        private static IUrlHelper GetUrlHelper<T>(IHtmlHelper<T> html)
        {
            return ((IUrlHelperFactory)html.ViewContext.HttpContext.RequestServices.GetService(typeof(IUrlHelperFactory))).GetUrlHelper(html.ViewContext);
        }

        /*beging*/
        /// <summary>
        /// initialize PopupForms for grid crud
        /// </summary>
        /// <param name="html"></param>
        /// <param name="gridId"></param>
        /// <param name="crudController">controller containing the crud actions</param>
        /// <param name="createPopupHeight">height of the create/edit popup</param>
        /// <param name="maxWidth"> max popup width</param>
        /// <param name="reload">reload grid after save/delete action success</param>
        /// <param name="area"></param>
        /// <param name="usePageMethods">generate urls for razor pages methods instead of mvc controllers</param>
        public static IHtmlContent InitCrudPopupsForGrid<T>(
            this IHtmlHelper<T> html,
            string gridId,
            string crudController = null,
            int createPopupHeight = 430,
            int maxWidth = 0,
            bool reload = false,
            string area = null,
            bool usePageMethods = false)
        {
            var url = GetUrlHelper(html);
            gridId = html.Awe().GetContextPrefix() + gridId;

            var refreshGrid = "refreshGrid";
            var format = "utils.{0}('" + gridId + "')";

            var createFunc = string.Format(format, reload ? refreshGrid : "itemCreated");
            var editFunc = string.Format(format, reload ? refreshGrid : "itemEdited");
            var delFunc = string.Format(format, reload ? refreshGrid : "itemDeleted");
            var delConfirmFunc = string.Format(format, "delConfirmLoad");

            var result =
            html.Awe()
                .InitPopupForm()
                .Name("create" + gridId)
                .Group(gridId)
                .Height(createPopupHeight)
                .MaxWidth(maxWidth)
                .Url(usePageMethods ? url.Page("", "Create") : url.Action("Create", crudController, new { area }))
                .Title("Create item")
                .Modal()
                .Success(createFunc)
                .ToString()

            + html.Awe()
                  .InitPopupForm()
                  .Name("edit" + gridId)
                  .Group(gridId)
                  .Height(createPopupHeight)
                  .MaxWidth(maxWidth)
                  .Url(usePageMethods ? url.Page("", "Edit") : url.Action("Edit", crudController, new { area }))
                  .Title("Edit item")
                  .Modal()
                  .Success(editFunc)

            + html.Awe()
                  .InitPopupForm()
                  .Name("delete" + gridId)
                  .Group(gridId)
                  .Url(usePageMethods ? url.Page("", "Delete") : url.Action("Delete", crudController, new { area }))
                  .Title("Delete item")
                  .Success(delFunc)
                  .OnLoad(delConfirmFunc) // calls grid.api.select and animates the row
                  .Height(200)
                  .Modal();

            return new HtmlString(result);
        }
        /*endg*/

        /// <summary>
        /// initialize PopupForms for grid nest crud
        /// </summary>
        /// <param name="html"></param>
        /// <param name="gridId"></param>
        /// <param name="crudController">controller containing the crud actions</param>
        /// <param name="reload">reload grid after save/delete action success</param>
        /// <param name="area"></param>
        public static IHtmlContent InitCrudForGridNest<T>(
            this IHtmlHelper<T> html,
            string gridId,
            string crudController,
            bool reload = false,
            string area = null)
        {
            var url = GetUrlHelper(html);
            gridId = html.Awe().GetContextPrefix() + gridId;

            var refreshGrid = "refreshGrid";
            var format = "utils.{0}('" + gridId + "')";

            var createFunc = string.Format(format, reload ? refreshGrid : "itemCreated");
            var editFunc = string.Format(format, reload ? refreshGrid : "itemEdited");
            var delFunc = string.Format(format, reload ? refreshGrid : "itemDeleted");

            var result =
                html.Awe()
                    .InitPopupForm()
                    .Name("create" + gridId)
                    .Group(gridId)
                    .Url(url.Action("Create", crudController, new { area }))
                    .Mod(o => o.Inline().ShowHeader(false))
                    .Success(createFunc)
                    .ToString()
                + html.Awe()
                      .InitPopupForm()
                      .Name("edit" + gridId)
                      .Group(gridId)
                      .Url(url.Action("Edit", crudController, new { area }))
                      .Mod(o => o.Inline().ShowHeader(false))
                      .Success(editFunc)
                + html.Awe()
                      .InitPopupForm()
                      .Name("delete" + gridId)
                      .Group(gridId)
                      .Url(url.Action("Delete", crudController, new { area }))
                      .Mod(o => o.Inline().ShowHeader(false))
                      .Success(delFunc);

            return new HtmlString(result);
        }

        /*beginal*/
        public static IHtmlContent InitCrudPopupsForAjaxList<T>(
           this IHtmlHelper<T> html,
           string ajaxListId,
           string controller,
           string popupName)
        {
            var url = GetUrlHelper(html);

            var result =
                html.Awe()
                    .InitPopupForm()
                    .Name("create" + popupName)
                    .Url(url.Action("Create", controller))
                    .Height(430)
                    .Success("utils.itemCreatedAlTbl('" + ajaxListId + "')")
                    .Group(ajaxListId)
                    .Title("create item")
                    .ToString()

                + html.Awe()
                      .InitPopupForm()
                      .Name("edit" + popupName)
                      .Url(url.Action("Edit", controller))
                      .Height(430)
                      .Success("utils.itemEditedAl('" + ajaxListId + "')")
                      .Group(ajaxListId)
                      .Title("edit item")

                + html.Awe()
                      .InitPopupForm()
                      .Name("delete" + popupName)
                      .Url(url.Action("Delete", controller))
                      .Success("utils.itemDeletedAl('" + ajaxListId + "')")
                      .Group(ajaxListId)
                      .OkText("Yes")
                      .CancelText("No")
                      .Height(200)
                      .Title("delete item");

            return new HtmlString(result);
        }
        /*endal*/

        /// <summary>
        /// initialize Delete PopupForms for grid
        /// </summary>
        /// <param name="html"></param>
        /// <param name="gridId"></param>
        /// <param name="crudController">controller containing the crud actions</param>
        /// <param name="action">delete action name</param>
        /// <param name="reload">reload grid after delete action success</param>
        /// <param name="area"></param>
        /// <param name="url">url to load popup content</param>
        public static IHtmlContent InitDeletePopupForGrid<T>(
            this IHtmlHelper<T> html,
            string gridId,
            string crudController = null,
            string action = "Delete",
            bool reload = false,
            string area = null,
            string url = null)
        {
            var urlh = GetUrlHelper(html);
            gridId = html.Awe().GetContextPrefix() + gridId;

            var refreshGrid = "refreshGrid";
            var format = "utils.{0}('" + gridId + "')";

            var delFunc = string.Format(format, reload ? refreshGrid : "itemDeleted");
            var delConfirmFunc = string.Format(format, "delConfirmLoad");

            var result =
                html.Awe()
                  .InitPopupForm()
                  .Name("delete" + gridId)
                  .Group(gridId)
                  .Url(url ?? urlh.Action(action, crudController, new { area }))
                  .Success(delFunc)
                  .OnLoad(delConfirmFunc) // calls grid.api.select and animates the row
                  .Height(200)
                  .Modal()
                  .ToString();

            return new HtmlString(result);
        }
    }
}