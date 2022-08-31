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

Handlebars.registerHelper('ifIn', function (elem, list, options) {
    if (list!=null && list.indexOf(elem) > -1) {
        return options.fn(this);
    }
    return options.inverse(this);
});

function ActionsFilterWidget() {
    /***
    * This method must return type of registered widget type in 'SetFilterWidgetType' method
    */
    this.getAssociatedTypes = function () {
        return ["ActionsNameFilterWidget"];
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
        this.registerEvents(); //handle events
    };


    this.renderWidget = function () {
        var filters = this.value.filterValue.split('|');
        var template;
        if ("FiltreRead" === this.columName) {
            template = Handlebars.compile($("#renderwidget-template-actions-read").html());
        }
        else {
            template = Handlebars.compile($("#renderwidget-template-actions").html());
        }
        var html = template({ lang_selectClientToFilter: this.lang.selectActionsToFilter, lang_applyFilterButtonText: this.lang.applyFilterButtonText, filters: filters });
        this.container.append(html);
    };
    
    /***
    * Internal method that register event handlers for 'apply' button.
    */
    this.registerEvents = function () {
        //get list with customers
        var customerList = this.container.find(".actionslist");
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

            $('.actionslist input:checked').each(function (index, element) {
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