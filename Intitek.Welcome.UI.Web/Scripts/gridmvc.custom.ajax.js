function loadGrid(url, mvcGrid, callback) {
    openLoading();
    console.log("loadGrid.." + url);
    var ajaxLoadUrlSave = mvcGrid.loadDataAction;
    var saveGridColumnFilters = mvcGrid.gridColumnFilters;
    var saveGridSort = mvcGrid.gridSort;
    var saveGridFilterForm = mvcGrid.gridFilterForm;
    var saveFiltersWidget = mvcGrid.filterWidgets;
    $.ajax({
        url: url,
        type: 'get',
        cache: false
    }).done(function (response) {
        var gridName = mvcGrid.jqContainer.data("gridname");
        $("#parent_" + gridName).html(response.Html);
        $("#" + gridName).gridmvc();
        //Tooltip
        $('[data-toggle="tooltip"]').tooltip();
        window.pageGrids[gridName].filterWidgets = saveFiltersWidget;
        window.pageGrids[gridName].ajaxify({
            isFirstLoadPage: false,
            getData: ajaxLoadUrlSave
        });
        window.pageGrids[gridName].gridColumnFilters = saveGridColumnFilters;
        window.pageGrids[gridName].gridSort = saveGridSort;
        window.pageGrids[gridName].gridFilterForm = saveGridFilterForm;
        console.log("done...");
        return false;
        }).fail(function (xhr) {
            console.log("error..");
            var errorMessage = $.trim(xhr.responseText);
            var htmlTag = errorMessage.indexOf("<html>");
            if (htmlTag !== -1) {
                document.write(errorMessage);
                return false;
            }
            //si <CustomErrors> est activé dans web.config
            $("#content").append(errorMessage);
            $("#ajaxErrorModal").on("shown.bs.modal", function () {
                var contentHeight = $(window).height();
                var footerHeight = $(this).find('.modal-footer').outerHeight() || 0;
                var maxHeight = contentHeight - footerHeight ;
                $(this).find('.modal-body').css({
                        'max-height': maxHeight,
                        'overflow-y': 'auto'
                    });
                $(this).before($('.modal-backdrop'));
                $(this).css("z-index", parseInt($('.modal-backdrop').css('z-index')) + 1);
            });
            $("#ajaxErrorModal").modal("show");
    }).always(function (response) {
        if (callback) {
            callback(response);
        }
        closeLoading();
    });
}

function getToPage(elementSelected, pageCible) {
    var context = $(elementSelected).closest('.grid-mvc');
    var gridName = $(elementSelected).closest('.grid-mvc').attr('id');
    var parameterPage = gridName + "-page";
    var parameterPageSize = gridName + "-pagesize";
    //var pageSize = $("#PageSize", context).val();
    var pageSize = $("#PageSize[data-grid=" + gridName + "]").val();
    var mvcGrid = window.pageGrids[gridName];
    var gridQuery = mvcGrid.getGridUrl(mvcGrid.loadDataAction, mvcGrid.gridColumnFilters, null);
    var gridUrl = URI(gridQuery).addQuery(parameterPage, pageCible).
        addQuery(parameterPageSize, pageSize).addQuery("grid", gridName);
    
    gridUrl = URI.decode(gridUrl);
    loadGrid(gridUrl, mvcGrid);
    return false;
}

/*function changePageSize(elementSelected, pageSize) {
    var context = $(elementSelected).closest('.grid-mvc').find('.grid-table');
    var gridName = $(elementSelected).closest('.grid-mvc').attr('id');
    var parameterPageSize = gridName + "-pagesize";
    var mvcGrid = window.pageGrids[gridName];
    var gridQuery = mvcGrid.getGridUrl(mvcGrid.loadDataAction, mvcGrid.gridColumnFilters, null);
    var gridUrl = URI(gridQuery).addQuery(parameterPageSize, pageSize).normalizeSearch().addQuery("grid", gridName);
   
    gridUrl = URI.decode(gridUrl);
    //console.log("URL=" + gridUrl);
    loadGrid(gridUrl, mvcGrid);
    return false;
}*/
function changePageSize(elementSelected, pageSize) {
    var gridName = $(elementSelected).data("grid");
    var parameterPageSize = gridName + "-pagesize";
    var mvcGrid = window.pageGrids[gridName];
    var gridQuery = mvcGrid.getGridUrl(mvcGrid.loadDataAction, mvcGrid.gridColumnFilters, null);
    var gridUrl = URI(gridQuery).addQuery(parameterPageSize, pageSize).normalizeSearch().addQuery("grid", gridName);

    gridUrl = URI.decode(gridUrl);
    //console.log("URL=" + gridUrl);
    loadGrid(gridUrl, mvcGrid);
    return false;
}
function sorted(elementSelected) {
    var context = $(elementSelected).closest('.grid-mvc');
    var href = $(elementSelected).attr('href');
    console.log('href', href);
    var gridName = context.attr('id');

    var parameterColumn = gridName + "-column";
    var parameterDirection = gridName + "-dir";
    var parameterPageSize = gridName + "-pagesize";
    //var pageSize = $("#PageSize", context).val();
    var pageSize = $("#PageSize[data-grid=" + gridName+"]").val();

    var mvcGrid = window.pageGrids[gridName];
    mvcGrid.gridSort = "";
    var gridQuery = mvcGrid.getGridUrl(mvcGrid.loadDataAction, mvcGrid.gridColumnFilters, null);
    var mySort = URI.parseQuery(href);
    var gridColumnSort = URI("").addQuery(parameterColumn, mySort[parameterColumn]).addQuery(parameterDirection, mySort[parameterDirection]).query();
    mvcGrid.gridSort = gridColumnSort;
    //console.log("mvcGrid.gridSort=" + mvcGrid.gridSort);
    var gridUrl = URI(gridQuery);
    gridUrl.addSearch(parameterColumn, mySort[parameterColumn]);
    gridUrl.addSearch(parameterDirection, mySort[parameterDirection]);
    gridUrl.addQuery(parameterPageSize, pageSize);
    gridUrl.addQuery("grid", gridName);
   
    gridUrl = URI.decode(gridUrl);
    //console.log("URL=" + gridUrl);
    loadGrid(gridUrl, mvcGrid);
    return false;
}
function rechercherFiltre(gridName) {
    var parameterPageSize = gridName + "-pagesize";
    var pageSize = $("#PageSize[data-grid=" + gridName + "]").val();
    var mvcGrid = window.pageGrids[gridName];
    var gridQuery = mvcGrid.getGridUrl(mvcGrid.loadDataAction, mvcGrid.gridColumnFilters, null);
    var gridUrl = URI(gridQuery).addQuery(parameterPageSize, pageSize).normalizeSearch().addQuery("grid", gridName);

    gridUrl = URI.decode(gridUrl);
    //console.log("URL=" + gridUrl);
    loadGrid(gridUrl, mvcGrid, function (response) {
        $("#total-" + gridName).text(response.total);
    });
    return false;
}

(function ($) {
    var plugin = GridMvc.prototype;

    // store copies of the original plugin functions before overwriting
    var functions = {};
    for (var i in plugin) {
        if (typeof (plugin[i]) === 'function') {
            functions[i] = plugin[i];
        }
    }


    // extend existing functionality of the gridmvc plugin
    $.extend(true, plugin, {
        initGridRowsEvents : function () {
            var $this = this;
            this.jqContainer.on("click", ".grid-row", function () {
                $this.rowClicked.call(this, $this);
            });
            //affectation document ou profil
            $('[name="DocsList"],[name="ProfsList"]', this.jqContainer).each(function () {
                var chk = $(this).is(":checked");
                var docId = $(this).val();
                var docstate = $("#docState_" + docId).val();
                var oldCheck = (docstate === "True") ? true : false;
                if (oldCheck != chk) {
                    $(this).closest("tr").addClass("coched");
                }
                else {
                    $(this).closest("tr").removeClass("coched");
                }
            });
        },
        parseFilterValues: function (filterData) {
            var opt = $.parseJSON(filterData);
            var filters = [];
            for (var i = 0; i < opt.length; i++) {
                filters.push({ filterValue: opt[i].FilterValue, filterType: opt[i].FilterType, columnName: opt[i].ColumnName });
            }
            return filters;
        },
        getSessionFilterValues: function () {
            var filtersUrl = "";
            var self = this;
            var gridName = self.jqContainer.data("gridname");
            var filters = self.jqContainer.find(".grid-filter");
            if (this.options.multiplefilters) { //multiple filters enabled
                for (var i = 0; i < filters.length; i++) {
                    var filterData = this.parseFilterValues($(filters[i]).attr("data-filterdata"));
                    if (filterData.length == 0) continue;
                    if (filtersUrl.length > 0) filtersUrl += "&";
                    filtersUrl += this.getFilterQueryData($(filters[i]).attr("data-name"), filterData);
                }
            }
            if (filtersUrl != "" && !self.gridColumnFilters) {
                console.log('filtersUrl', filtersUrl);
                self.gridColumnFilters = filtersUrl;
                window.pageGrids[gridName].gridColumnFilters = filtersUrl;
            }
        },
        applyFilterValues: function (initialUrl, columnName, values, skip) {
            //console.log("initialUrl", initialUrl);
            var self = this;
            var gridName = self.jqContainer.data("gridname");
            self.gridColumnFilters = null;
            var filters = self.jqContainer.find(".grid-filter");
            var url = URI(initialUrl).addQuery("grid", gridName).normalizeSearch().search();

            self.gridColumnFilters = "";
            if (!skip) {
                self.gridColumnFilters += this.getFilterQueryData(columnName, values);
            }

            if (this.options.multiplefilters) { //multiple filters enabled
                for (var i = 0; i < filters.length; i++) {
                    if ($(filters[i]).attr("data-name") != columnName) {
                        var filterData = this.parseFilterValues($(filters[i]).attr("data-filterdata"));
                        if (filterData.length == 0) continue;
                        if (self.gridColumnFilters.length > 0) self.gridColumnFilters += "&";
                        self.gridColumnFilters += this.getFilterQueryData($(filters[i]).attr("data-name"), filterData);
                    } else {
                        continue;
                    }
                }
            }

            if (self.gridColumnFilters.length > 0) {
                url += "&" + self.gridColumnFilters;
            }
            if (self.gridFilterForm) {
                //console.log("last url", url);
                url += "&" + self.gridFilterForm.serialize();
                url = URI(url).normalizeSearch().search();
                //console.log("afetr url", url);
            } 
            var fullSearch = url;
            if (fullSearch.indexOf("?") == -1) {
                fullSearch = "?" + fullSearch;
            }

            self.currentPage = 1;
            var urlA = self.loadDataAction + fullSearch;
            loadGrid(urlA, self, function (response) {
                $("#total-" + gridName).text( response.total);
            });
            return false ;
        },
        ajaxify: function (options) {
            var self = this;
            if(typeof options.isFirstLoadPage === "undefined")
                options.isFirstLoadPage = true;
            self.currentPage = 1;
            self.loadPagedDataAction = options.getPagedData;
            self.loadDataAction = options.getData;
            self.gridFilterForm = options.gridFilterForm;
            self.gridSort = self.jqContainer.find("div.sorted a").attr('href');
            self.pageSetNum = 1;
            self.partitionSize = parseInt(self.jqContainer.find(".grid-pageSetLink").attr("data-partitionSize"));
            self.lastPageNum = parseInt(self.jqContainer.find(".grid-page-link:last").attr('data-page'));
            var $namedGrid = $('[data-gridname="' + self.jqContainer.data("gridname") + '"]');
            self.jqContainer = $namedGrid.length === 1 ? $namedGrid : self.jqContainer;
            var gridName = self.jqContainer.data("gridname");
            var parameterColumn = gridName + "-column";
            var parameterDirection = gridName + "-dir";
            self.preFilterFormAction = options.preFilterFormAction;
            self.preFilterFormClient = options.preFilterFormClient;

            if (self.gridSort) {
                if (self.gridSort.indexOf(parameterDirection + "=0") != -1) {
                    self.gridSort = self.gridSort.replace(parameterDirection + "=0", parameterDirection + "=1");
                } else {
                    self.gridSort = self.gridSort.replace(parameterDirection + "=1", parameterDirection + "=0");
                }

                self.orginalSort = self.gridSort;
            }
            //Récuperer les filtres en sessions
            if (options.isFirstLoadPage) {
                self.getSessionFilterValues(); 
            }   
            self.getGridUrl = function (griLoaddAction, search, renderRowsOnly) {
                console.log('search', search);
                var gridName = self.jqContainer.data("gridname");
                var parameterColumn = gridName + "-column";
                var parameterDirection = gridName + "-dir";
                var gridQuery = "?";

                if (self.gridFilterForm) {
                    gridQuery += self.gridFilterForm.serialize();
                } else if (search) {
                    gridQuery = search;
                }

                gridQuery = URI(gridQuery);

                var myColFilters = URI.parseQuery(search);
                if (myColFilters['grid-filter']) {
                    gridQuery.addSearch("grid-filter", myColFilters["grid-filter"]);
                }

                if (self.gridSort) {
                    var mySort = URI.parseQuery(self.gridSort);
                    gridQuery.addSearch(parameterColumn, mySort[parameterColumn]);
                    gridQuery.addSearch(parameterDirection, mySort[parameterDirection]);
                }

                var gridUrl = URI(griLoaddAction).addQuery(gridQuery.search().replace("?", ""));
                //console.log("getGridUrl=", gridUrl);

                if (renderRowsOnly) {
                    gridUrl.addQuery(renderRowsOnly);
                } else {
                    gridUrl = gridUrl.removeSearch("renderRowsOnly").removeSearch("page");
                }
                gridUrl = URI.decode(gridUrl);
                return gridUrl;
            };

            self.SetupGridHeaderEvents = function () {
                self.jqContainer.on('click', '.grid-header-title > a', function (e) {
                    e.preventDefault();
                    self.currentPage = 1;
                    sorted(this);
                });
                self.jqContainer.on('click', '.grid-header-title > span', function (e) {
                    e.preventDefault();
                    self.currentPage = 1;
                    var aHref = $(this).prev('a');
                    sorted(aHref);
                });
            };
            self.SetupGridHeaderEvents();
        }
    });
})(jQuery);