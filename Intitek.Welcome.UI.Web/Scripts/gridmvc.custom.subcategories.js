/***
* This script demonstrates how you can build you own custom filter widgets:
* 1. Specify widget type for column:
*       columns.Add(o => o.Customers.CompanyName)
*           .SetFilterWidgetType("CustomCompanyNameFilterWidget")
* 2. Register script with custom widget on the page:
*       <script src="@Url.Content("~/Scripts/gridmvc.customwidgets.js")" type="text/javascript"> </script>
* 3. Register your own widget in Grid.Mvc:
*       GridMvc.addFilterWidget(new CustomersFilterWidget());
*
* For more documentation see: http://gridmvc.codeplex.com/documentation
*/
//Updated by Aman Sharma (http://HarvestingClouds.com)

function SubCategoriesFilterWidget() {
    /***
    * This method must return type of registered widget type in 'SetFilterWidgetType' method
    */
    this.getAssociatedTypes = function () {
        return ["SubCategoryNameFilterWidget"];
    };
    /***
    * This method invokes when filter widget was shown on the page
    */
    this.onShow = function () {
        /* Place your on show logic here */
    };

    this.showClearFilterButton = function () {
        return true;
    };
    /***
    * This method will invoke when user was clicked on filter button.
    * container - html element, which must contain widget layout;
    * lang - current language settings;
    * typeName - current column type (if widget assign to multipile types, see: getAssociatedTypes);
    * values - current filter values. Array of objects [{filterValue: '', filterType:'1'}];
    * cb - callback function that must invoked when user want to filter this column. Widget must pass filter type and filter value.
    * data - widget data passed from the server
    */
 this.onRender = function (container, lang, typeName, values, cb, data) {
    //store parameters:
    this.cb = cb;
    this.url = data;
    this.container = container;
    this.columName = this.container.closest(".grid-filter").attr("data-name");
    this.lang = lang;

    //this filterwidget demo supports only 1 filter value for column column
    this.value = values.length > 0 ? values[0] : { filterType: 1, filterValue: "" };

    this.renderWidget(); //onRender filter widget
    this.loadSubCategories(); //load customer's list from the server
    this.registerEvents(); //handle events
 };
 

    this.renderWidget = function () {
    var html = "";
    var template = Handlebars.compile($("#renderwidget-template-subcategories").html()); 
    html = template({ lang_selectClientToFilter: this.lang.selectSubCategoriesToFilter, lang_applyFilterButtonText: this.lang.applyFilterButtonText });
    this.container.append(html);
};
/***
* Method loads all Categories from the server via Ajax:
*/
    this.loadSubCategories = function () {
        var $this = this;
        openLoading();
        $.post(this.url, function (data) {
            $this.fillSubCategories(data.Items);
            closeLoading();
        });
    };
    /***
    * Method fill customers select list by data
    */
    this.fillSubCategories = function (items) {
        var filters = this.value.filterValue.split('|');
        var customerList = this.container.find(".subcategorieslist");
        var old_id_document_categ = -1;
        jQuery.each(items, function (index, val) {
            if (index > 0) {
                old_id_document_categ = items[index - 1].ID_DocumentCategory;
            }
            //console.log('old',old_id_document_categ);
            //console.log('current', val.ID_DocumentCategory) 
            if (old_id_document_categ != val.ID_DocumentCategory && val.ID_DocumentCategory != -1) {
                var categ = '<span class="groupcateg">' + val.NameCategory + '</span><br/>';
                if (val.IsDefaultLangNameCategory)
                    categ = '<span class="groupcateg defaultName">' + val.NameCategory + '</span><br/>';
                customerList.append(categ);
            }
            var isDefaultLangeName = items[index].IsDefaultLangName;
            var name = "<span>" + items[index].Name+"</span>";
            if (isDefaultLangeName)
                name = '<span class="defaultName">' + items[index].Name + '</span>';
            console.log("name...", name);
            if (val.ID_DocumentCategory == -1) {
                customerList.append('<span class="nonesubcateg"><input type = "checkbox"' + (filters.indexOf(items[index].ID + "") > -1 ? 'checked="checked"' : '') + ' value="' + items[index].ID + '"><b>' + name + '</b></input></span><br/>');
            }
            else {
                customerList.append('<input type = "checkbox"' + (filters.indexOf(items[index].ID + "") > -1 ? 'checked="checked"' : '') + ' value="' + items[index].ID + '">' + name + '</input><br />');
            }           
        });
    };
    
    /***
    * Internal method that register event handlers for 'apply' button.
    */
    this.registerEvents = function () {
        //get list with customers
        var customerList = this.container.find(".subcategorieslist");
        //get apply button from:
        var applyBtn = this.container.find(".grid-apply");
        //save current context:
        var $context = this;
        //register onclick event handler
        applyBtn.click(function () {
            //get selected filter type:
            //var type = $context.container.find(".grid-filter-type").val();
            var type = 1;
            //get filter value:
            //var value = $context.container.find(".grid-filter-input").val();
            var value = "";

            $('.subcategorieslist input:checked').each(function (index, element) {
                if (index == 0) {
                    value += element.value;
                }
                else {
                    value += "|" + element.value;
                }
            });


            //invoke callback with selected filter values:
            var filterValues = [{ filterType: type, filterValue: value }];
            $context.cb(filterValues);
        });
        //register onclick event handler
        //customerList.change(function () {
        ////invoke callback with selected filter values:
        //var values = [{ filterValue: $(this).val(), filterType: 1 /* Equals */ }];
        //$context.cb(values);
        //});

        //register onEnter event for filter text box:
        this.container.find(".grid-filter-input").keyup(function (event) {
            if (event.keyCode == 13) { applyBtn.click(); }
            if (event.keyCode == 27) { GridMvc.closeOpenedPopups(); }
        });
    };

}