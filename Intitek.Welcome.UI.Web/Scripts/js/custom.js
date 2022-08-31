// Loading config
var loading;
var loadingOptions;

if (window.matchMedia('(min-width: 320px) and (max-width: 480px)').matches) {
    loadingOptions = {
        loadingBgColor: 'transparent',
        loadingAnimation: 'image',
        animationSrc: '../../Content/images/loader_small.svg',
        animationWidth: 80,
        animationHeight: 80,
        maskBgColor: 'rgba(0, 0, 0, .7)',
        defaultApply: true,
    };
} else {
    loadingOptions = {
        loadingBgColor: 'transparent',
        loadingAnimation: 'image',
        animationSrc: '../../Content/images/loader.svg',
        animationWidth: 105,
        animationHeight: 105,
        maskBgColor: 'rgba(0, 0, 0, .7)',
        defaultApply: true,
    };
}

// Popups vars : please, don't forget to add here when a new popup is added
var popups = [
    { id: 'listColumns', value: document.getElementById('listColumns') },
    { id: 'personnal-request', value: document.getElementById('personnal-request') },
    { id: 'filterClient', value: document.getElementById('filterClient') },
    { id: 'filter-projet', value: document.getElementById('filter-projet') }
]

var modalConfirm = function (message, callback) {
    $('#confirmModal').on('show.bs.modal', function () {
        $("div.modal-body > p").empty().html(message);
    });
    $('#confirmModal').modal({
        backdrop: 'static'
    });
    $("#confirmModal").appendTo("body");

    $('#confirmModal a.buttonYes').off().on('click', function () {
        $('#confirmModal').modal('hide');
        callback(true);
    });
    $('#confirmModal a.buttonNo').off().on('click', function () {
        $('#confirmModal').modal('hide');
        callback(false);
    });
}
 
// Call include file on w3-include-html attribute
includeHTML();

$(document).ready(function() {
    $.each($("select"), function() {
        if (!$.trim(this.value)) $(this).addClass('placeholder');
    });
    $("select").change(function() {
        $.trim(this.value) ? $(this).removeClass('placeholder') : $(this).addClass('placeholder')
    });
    $.each($('input[type="file"]'), function() {
        if (!$.trim(this.value)) $(this).addClass('placeholder');
    });
    $('input[type="file"]').change(function() {
        $.trim(this.value) ? $(this).removeClass('placeholder') : $(this).addClass('placeholder')
    });
    $('body').on('hidden.bs.modal', function () {
        $('.modal .modal-dialog').css({ top: '', left: '' });
    });
    $('body').on('show.bs.modal', '.modal', centerModal);
    $('body').on("shown.bs.modal", dragModal);
    $(window).on("resize", function () {
        $('.modal:visible').each(centerModal);
    });
    $('textarea').on('keydown', autosize);
    $('.bootstrap-select').on('hide.bs.dropdown', function () {
        $(this).removeClass("dropup");
    });
})

$(document).on('mouseenter', ".one-row-text", function() {
    var $this = $(this);
    if (this.offsetWidth < this.scrollWidth && !$this.attr('title')) {
        $this.tooltip({
            title: $this.text(),
            placement: "top"
        });
        $this.tooltip('show');
    }
});

// Hide dropdown when clicking over
$(document).click(function() {
    for (var i = 0; i < popups.length; i++) {
        popups[i].value ? popups[i].value.classList.remove('showDropdown') : null;
    }
});

// Close other popups by id
function closeOthers(id) {
    for (i = 0; i < popups.length; i++) {
        if (popups[i].id != id) {
            popups[i].value.classList.remove('showDropdown');
        }
    }
}

// Function to include file's content
function includeHTML() {
    var z, i, elmnt, file, xhttp;
    z = document.getElementsByTagName("*");
    for (i = 0; i < z.length; i++) {
        elmnt = z[i];
        file = elmnt.getAttribute("w3-include-html");
        if (file) {
            file = file  + "?" + new Date().getTime();
            xhttp = new XMLHttpRequest();
            xhttp.onreadystatechange = function() {
                if (this.readyState == 4) {
                    if (this.status == 200) { elmnt.innerHTML = this.responseText; }
                    if (this.status == 404) { elmnt.innerHTML = "File not found !"; }
                    elmnt.removeAttribute("w3-include-html");
                    includeHTML();
                }
            }
            xhttp.open("GET", file, false);
            xhttp.send();
            return;
        }
    }
}

// Show/Hide dropdowns from buttons
function openDropdown(id) {
    document.getElementById(id).classList.toggle("showDropdown");
}

// Show / hide password
function showPassword(id) {
    var x = document.getElementById(id);
    if (x.type === "password" && !x.disabled) {
        x.type = "text";
        document.getElementById('icon-eye').classList.remove('fa-eye');
        document.getElementById('icon-eye').classList.add('fa-eye-slash');
    } else {
        x.type = "password";
        document.getElementById('icon-eye').classList.remove('fa-eye-slash');
        document.getElementById('icon-eye').classList.add('fa-eye');
    }
}

// On click Event + Scroll to
function clickDOMById(id, scrollTo) {
    var x = document.getElementById(id);
    x.click();
    if (scrollTo) {
        $('html,body').animate({
            scrollTop: $("#" + scrollTo).offset().top - 80
        }, 'slow');
    }
}

// Back to previous page
function goBack() {
    openLoading(true);
    window.history.back();
}

/* Open loading
 * Auto-hide loading after 3 secondes
 * if isAutoLoadOut = true, auto-hide loading after 3 secondes
 */
function openLoading(isAutoLoadOut = false) {
    loading = new Loading(loadingOptions);
    if(isAutoLoadOut) {
        setTimeout(() => loading.out(), 2000)
    }
}

// Hide loading on click
//Bug sur closeLoading : div.modal-mask ne peut pas supprimer (solution : ajout isCloseForcing=true)
function closeLoading(isCloseForcing=false) {
    if (isCloseForcing) {
        loading.set.animationDuration = 100;
    }
    loading.out();
    //Après loading.out(), si div.modal-mask existe encore, il faut le supprimer
    if (isCloseForcing) {
        var modalmask = $("div.modal-mask");
        if (modalmask.length > 0)
            modalmask.remove();
    }
}

// Simulate redirection after submit data
function _redirectTo(url){
    openLoading();
    setTimeout(() => window.location = url, 2000);
}
function _redirectToWithHash(url, hash) {
    openLoading();
    setTimeout(() => { window.location.href = url+ "#" + hash; }, 2000);
}
// End

function _getQueryParam(query, id) {
    var $_GET = {};
    if(document.location.toString().indexOf('?') !== -1) {
        var query = document.location
                    .toString()
                    .replace(/^.*?\?/, '')
                    .replace(/#.*$/, '')
                    .split('&');
    
        for(var i=0, l=query.length; i<l; i++) {
            var aux = decodeURIComponent(query[i]).split('=');
            $_GET[aux[0]] = aux[1];
        }
    }
    return $_GET[id];  
}

function showErrorMessage(errorMessage) {
    $("#errorMessage").empty().append(errorMessage);
    $("#errorModal").modal('show');

}

function showApproveMessage(approveMessage) {
    $("#approveMessage").empty().append(approveMessage);
    $("#approveModal").modal({
        keyboard: false,
        backdrop: 'static'
    });
}

function showConfirmMessage(confirmMessage) {
    $("#confirmMessage").empty().append(confirmMessage);
    $("#confirmModal").modal({
        keyboard: false,
        backdrop: 'static'
    });
}

function showDeleteMessage(message, id, controller) {
    $("div.modal-body > p").empty().html(message);
    $("#IDtoDelete").val(id);
    $("#controller").val(controller);
    $("#deleteModal").modal('show');
    $("#deleteModal").appendTo("body");
}
function showSharableLink(documentName,link) {
    //reset button copy
    /*$("#div-result-copy").empty();
    $("#div-result-copy").append("<a href=\"#\" class=\"btn btn-green btn-wme btn-xs\"  id=\"copylink\" onclick=\"copyLink()\">" + btnCopyText+"</a>");
    */
    //set the document name
    $("#link-dialog-docname").empty();
    $("#link-dialog-docname").append(documentName);

    //set the link
    $("#linkMessage").val(link);
    $("#linkModal").modal('show');
    $("#linkModal").appendTo("body");
    closeLoading();

}


function _collapseSidebar() {
    $('#sidebar').toggleClass('active');
    $('#content').toggleClass('expanded');
    $('#iconCollapse').toggleClass('fa-rotate-180');
}


function toggleButtonPlusMinus(element, expand) {
    if (expand) {
        element.removeClass('fa-minus-square');
        element.addClass('fa-plus-square');
    } else {
        element.removeClass('fa-plus-square');
        element.addClass('fa-minus-square');
    }
}

$(function () {
    $('.expand-plus-minus').on('click', function () {
        if ($(this).attr('aria-expanded') === 'true') {
            toggleButtonPlusMinus($(this).find('i'), true);
        } else {
            toggleButtonPlusMinus($(this).find('i'), false);
        }
    });
})

function centerModal() {
    $(this).css('display', 'block');
    var $dialog = $(this).find(".modal-dialog"),
        offset = ($(window).height() - $dialog.height()) / 2,
        bottomMargin = parseInt($dialog.css('marginBottom'), 10);
    if (offset < bottomMargin) offset = bottomMargin;
    $dialog.css("margin-top", offset);
}

function dragModal() {
    var modalDialog = $(this).find('.modal-dialog');
    modalDialog.draggable({
        cursor: "move"
    });
}

function autosize() {
    var el = this;
    setTimeout(function () {
        el.style.cssText = 'height:auto;';
        el.style.cssText = 'height:' + el.scrollHeight + 'px';
    }, 0);
}

function setInputLabelErrorRowHeight($field) {
    $("label[for='" + $field.attr('id') + "']").parent().height('50px');
}

function debounce(func, wait, immediate) {
    var timeout;
    return function () {
        var context = this, args = arguments;
        var later = function () {
            timeout = null;
            if (!immediate) func.apply(context, args);
        };
        var callNow = immediate && !timeout;
        clearTimeout(timeout);
        timeout = setTimeout(later, wait);
        if (callNow) func.apply(context, args);
    };
};