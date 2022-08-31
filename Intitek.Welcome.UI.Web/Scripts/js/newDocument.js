var isInputDisabled = false;
var isInformative = true;
var $_GET = {};
if (document.location.toString().indexOf('?') !== -1) {
    var query = document.location
        .toString()
        .replace(/^.*?\?/, '')
        .replace(/#.*$/, '')
        .split('&');

    for (var i = 0, l = query.length; i < l; i++) {
        var aux = decodeURIComponent(query[i]).split('=');
        $_GET[aux[0]] = aux[1];
    }
}

$(document).ready(function () {
    var documentName = "";
    var selectedDocId = 0;
    if ($_GET['edit'] === '0') {
        setTimeout(function () {
            enableForm();
        }, 0);
    }
    if ($_GET['edit'] !== undefined) {
        $('#visualizeIcon').removeClass('hide');
        $('#visualizeIcon').attr('style', 'pointer-events:all;');
        //populateForm();
    }

    $('.newVersion').hide();


    if ($("input[name='Name']").val() != "") {
        $("input[name='Name']").removeClass('form-control-required');
    }

    if ($("#TypeAffectation").val().indexOf("Agence") > 0) {
        $(".attachment").show();
        $("#EntityAffectation").parent().show();
        $("#ProfilAffectation").parent().hide();
    }
    else if ($("#TypeAffectation").val() === "Profil") {
        $(".attachment").show();
        $("#EntityAffectation").parent().hide();
        $("#ProfilAffectation").parent().show();
        //$("#ProfilAffectation").parent().show();
    }
    else {
        $(".attachment").hide();
        $("#EntityAffectation").parent().hide();
        $("#ProfilAffectation").parent().hide();
    }

    $("input[name='Name']").autocomplete({
        appendTo: '#name-autocomplete',
        source: function (request, response) {
            $.ajax({
                async: true,
                type: "POST",
                url: host + "Admin/Document/GetAllNames",
                contentType: "application/json; charset=utf-8",
                data: '{"exact": "false","term":"' + request.term + '"}',
                dataType: "json",
                processData: true,
                beforeSend: function () {
                    $("input[name='Name']").addClass('loading-input');
                },
                success: function (result) {
                    $("input[name='Name']").removeClass('loading-input');
                    response(result);
                },
                error: function (d) {
                    $("input[name='Name']").removeClass('loading-input');
                }
            });

        },
        delay: 500,
        minLength: 2,
        position: { my: "right top", at: "right bottom" },
        open: function () {
            $('ul.ui-autocomplete').width(300);

        },
        change: function () {
            $.ajax({
                async: false,
                type: "POST",
                url: host + "Admin/Document/GetAllNames",
                contentType: "application/json; charset=utf-8",
                data: '{"exact": "true","term":"' + $(this).val() + '"}',
                dataType: "json",
                processData: true,
                success: function (result) {
                    $('#IsDuplicatedName').val(false);
                    // result.length > 0 => Checking for existing name = true
                  
                    if (result.length > 0 && $('#ID').val() == "0") { //&& $(this).val() !== $("#OriginalName").val()) {
                        $('#IsDuplicatedName').val(true);
                       
                        $("#confirmEditDocument").modal({
                            "show": true,
                            backdrop: 'static',
                            keyboard: false
                        }).css("z-index", parseInt($('.modal-backdrop').css('z-index')) + 100);
                    };
                    $('#newDocumentForm').bootstrapValidator('revalidateField', 'Name');
                },
            })
        },
        select: function (event, arg) {
            if (arg.item) {
                documentName = arg.item.label;
                selectedDocId = arg.item.value;
                $(this).val(documentName);
                $('#ID').val(selectedDocId);
                $.ajax({
                    async: false,
                    type: "POST",
                    url: host + "Admin/Document/GetAllNames",
                    contentType: "application/json; charset=utf-8",
                    data: '{"exact": "true","term":"' + documentName + '"}',
                    dataType: "json",
                    processData: true,
                    success: function (result) {
                        $('#IsDuplicatedName').val(false);
                        // result.length > 0 => Checking for existing name = true
                        if (result.length > 0 && documentName !== $("#OriginalName").val()) {
                            $('#IsDuplicatedName').val(true);
                           
                            $("#confirmEditDocument").modal({
                                "show": true,
                                backdrop: 'static',
                                keyboard: false
                            }).css("z-index", parseInt($('.modal-backdrop').css('z-index')) + 100);
                        
                        };
                        $('#newDocumentForm').bootstrapValidator('revalidateField', 'Name');
                    },

                });

            }

        },
        close: function () {
            if (selectedDocId != 0) {
                $(this).val(documentName);
            }
            else {

            }
            documentName = "";
            $('#newDocumentForm').bootstrapValidator('revalidateField', 'Name');
            //$(this).removeClass('form-control-required');
        }
    });

    $("#confirmEditDocument").on('hidden.bs.modal', function () {
        $("input[name='Name']").val("");
        $('#ID').val(0);
        $('#IsDuplicatedName').val(false);
        $('#newDocumentForm').bootstrapValidator('revalidateField', 'Name');
    });

    $("#confirmEditDocument").on('show.bs.modal', function () {
        var doc = ($("div.modal-body  p").html()).replace("##documentName##", '<span class="label-error delete-content">' + documentName + '</span>');
        $("div.modal-body  p").html(doc);
    });
   
    if ($("#test-1").attr("checked")) {
        $('.quizz-group').show(); //prop('disabled', '');
        if ($("#IdQcm").val() == null) $("#IdQcm").addClass('form-control-required');
    }
    else {
        $('.quizz-group').hide();
        $('#IdQcm').removeClass('form-control-required');
    }

    if ($("input[name='IsNoActionRequired']")[0].checked) {
        $("input[name='Approbation']").prop("disabled", isInformative);
        //$("input[name='assignmentType']").prop("disabled", isInformative);
        $("input[name='Test']").prop("disabled", isInformative);
        $('.quizz-group').hide();
        isInformative = !isInformative;
    }

    $("#uploadLabel").removeClass('uploadLabel-holder').addClass('uploadLabel');

    $('#informativeCheckbox').on('click', function () {
        if ($("input[name='IsNoActionRequired']")[0].checked) {
            $("input[name='Approbation']").prop('checked', '').prop("disabled", isInformative);
            $("input[name='Test']").prop('checked', '').prop("disabled", isInformative);
            //$("input[name='assignmentType']").prop("disableda, isInformative);

            $('.quizz-group').hide();
            $('#IdQcm').removeClass('form-control-required');
            $('#IsNoActionRequired').val(isInformative);
            isInformative = !isInformative;

        }
    });

    $(".is-test").on('click', function (e) {
        if ($("#test-1").is(":checked")) {
            $('.quizz-group').show();
            $('#IdQcm').addClass('form-control-required');
        }
        else {
            $('.quizz-group').hide();
            $('#IdQcm').removeClass('form-control-required').val(null);
            $('#newDocumentForm').bootstrapValidator('revalidateField', 'IdQcm');
            //$('#newDocumentForm').data('bootstrapValidator').resetForm();

        }
    });

    $(".typeAffectation").on('click', function (e) {
        $(".attachment").show();
        $("#Affectation").val("");
        if ($(this).is(":checked")) {
            $('#TypeAffectation').val($(this).val());
         
            if ($(this).val() === "Profil") {
                $("#EntityAffectation").parent().find("li.selected").removeClass("selected");
                $("#EntityAffectation").parent().hide();
                $("#ProfilAffectation").parents().show();
            }
            else {
                $("#ProfilAffectation").parent().find("li.selected").removeClass("selected");
                $("#ProfilAffectation").parent().hide();
                $("#EntityAffectation").parents().show();
            }
        }
        else {
            $(".attachment").hide();
            $("#EntityAffectation").parent().hide();
            $("#ProfilAffectation").parent().hide();
        }
       
       
        $("div.bootstrap-select button.dropdown-toggle").removeClass("disabled");

    });

    $("#EntityAffectation").on("changed.bs.select", function () {
        $("#Affectation").val($("#EntityAffectation").val());
    });

    $("#ProfilAffectation").on("changed.bs.select", function () {
        $("#Affectation").val($("#ProfilAffectation").val());
    });



    $("#chkDelete").on('click', function (e) {
        $(this).val($(this).is(":checked"));
        if (!$("this").is(":checked")) {
            $("span.histo-delete").remove();
        }
    });

    $("#chkReadBrowser").on('click', function (e) {
        $(this).val($(this).is(":checked"));
        $('#checkReadingMode').val($(this).is(":checked") || $("#chkReadDownload").is(":checked"));
        $('#newDocumentForm').bootstrapValidator('revalidateField', 'checkReadingMode');
    });

    $("#chkReadDownload").on('click', function (e) {
        $(this).val($(this).is(":checked"));
        $('#checkReadingMode').val($(this).is(":checked") || $("#chkReadBrowser").is(":checked"));
        $('#newDocumentForm').bootstrapValidator('revalidateField', 'checkReadingMode');

    });

    $("#chkIsMetier").on('click', function (e) {
        $(this).val($(this).is(":checked"));
        $('#checkType').val($(this).is(":checked") || $("#chkIsMetier").is(":checked"));
        $('#newDocumentForm').bootstrapValidator('revalidateField', 'checkType');
    });

    $("#chkIsStructure").on('click', function (e) {
        $(this).val($(this).is(":checked"));
        $('#checkType').val($(this).is(":checked") || $("#chkIsStructure").is(":checked"));
        $('#newDocumentForm').bootstrapValidator('revalidateField', 'checkType');

    });

    $("#chkPhaseOnboarding").on('click', function (e) {
        $(this).val($(this).is(":checked"));
        $('#checkDocPhase').val($(this).is(":checked") || $("#chkPhaseEmployee").is(":checked"));
        $('#newDocumentForm').bootstrapValidator('revalidateField', 'checkDocPhase');

    });

    $("#chkPhaseEmployee").on('click', function (e) {
        $(this).val($(this).is(":checked"));
        $('#checkDocPhase').val($(this).is(":checked") || $("#chkPhaseOnboarding").is(":checked"));
        $('#newDocumentForm').bootstrapValidator('revalidateField', 'checkDocPhase');

    });


    

    //$(".btnValid").on("click", function (e) {
    //    e.preventDefault();
    //    $("#Affectation").val($("#EntityAffectation").val());
       
    //    $('#newDocumentForm').submit();

    //});

    $("#modalBtnConfirmeEdit").on("click", function (e) {
        e.preventDefault();
        document.location = host + 'Admin/Document/' + $("#ID").val();
        //$.ajax({
        //    async: false,
        //    type: "POST",
        //    url: host + "Admin/Document/Delete",
        //    contentType: "application/json; charset=utf-8",
        //    data: '{"id": "' + $("#ID").val() + '"}',
        //    dataType: "json",
        //    processData: true,
        //    success: function (result) {
        //        document.location = host + 'Admin/Document/Index';
        //    },

        //});
    });

    $(".priorityVersion").on("click", function () {

        if ($("#ID").val() > 0) {
            var version = getNewDocumentVersion($("#ID").val());
            $("#NewVersion").val(version);
            $(".newVersion").show();
        }
    });

    $("a.admin-lang").on("click", function () {
        var src = '../../Content/images/flags/';
        $('#adminlang-img').attr('src', src + $(this).data("code") + '.png');

         $.ajax({
            async: false,
            type: "POST",
            url: host + "Admin/Document/GetTranslatedDoc",
            contentType: "application/json; charset=utf-8",
            data: '{"id": "' + $("#ID").val() + '", "idLang" :"' + $(this).data("langid") + '"}',
            dataType: "json",
            processData: true,
            success: function (result) {
                console.log(result);
                $("input[name='IsMajor']").prop('disabled', 'disabled');              
                $(".newVersion").val($("input[name=Version]").val()).hide(); 
                $('#IdLang').val(result.doc.IdLang);
                $('input[name=Name]').val(result.doc.Name);
                $('#uploadLabel').html(UploadDefaultText);
                if (result.doc.NomOrigineFichier != '') {
                    $('#uploadLabel').html(result.doc.NomOrigineFichier);
                    $('#NomOrigineFichier').val(result.doc.NomOrigineFichier);
                }
                $('select#ID_Category option').remove();
                $('select#ID_Category').append('<option value="">' + SelectCategoryText +'</option>');
                $.each(result.doc.Categories, function (ind, value) {
                    $('select#ID_Category').append('<option value="'+ value.ID + '">' + value.Name + '</option>');
                });
                $('table#user-document-tab2 tbody tr').remove();
                //$('ul#myTab > li:last').hide();
               
                    //$('ul#myTab > li:last').show();
                $("span.nbdocversion").html(result.doc.Versions.length);
                if (result.doc.Versions.length > 0) {
                    $.each(result.doc.Versions, function (ind, value) {
                        var downloadLink = '';
                        if (value.NomOrigineFichier !== '')
                            downloadLink = '<a href="/Admin/Document/Download?versionID=' + value.ID + '&langID=' + result.IdLang + '" class="action-icon icon-green" ><i class="far fa-arrow-alt-circle-down"></i></a>';
                        $('table#user-document-tab2 tbody').append(
                            '<tr>' +
                            '<td>' + value.DateCre + '</td>' +
                            '<td>' + value.Version + '</td>' +
                            '<td>' + value.NomOrigineFichier + '</td>' +
                            '<td>' + value.UserName + '</td>' +
                            '<td>' + downloadLink + '</td>' +

                            '</tr>');
                    });
                }
                $('select#IdQcm option').remove();
                $('select#IdQcm').append('<option value="">' + SelectQuizzText + '</option>');
                $.each(result.qcms, function (ind, value) {
                    $('select#IdQcm').append('<option value="' + value.IdQcm + '">' + value.QcmName + '</option>');
                });
                $('#newDocumentForm').bootstrapValidator('revalidateField', 'Name');
            },

        });
    });

    $('#newDocumentForm').bootstrapValidator({
        excluded: [':disabled'],
        fields: {
            Name: {
                validators: {
                    notEmpty: {
                        message: "<span>" + ErrorDocumentNameRequired + "</span>"
                    },
                    callback: {
                        message: "<span>" + ErrorDocumentNameDuplicated + "</span>",
                        callback: function (value, validator, $field) {
                            // Determine the numbers which are generated in captchaOperation

                            if ($("#IsDuplicatedName").val() === "true") {
                                return false;
                            }
                            $field.removeClass('form-control-required');
                            return true;
                        }
                    }
                }
            },
            IdQcm: {
                validators: {
                    callback: {
                        message: "<span>" + ErrorQcmRequired + "</span>",
                        callback: function (value, validator, $field) {
                            $field.removeClass('form-control-required');
                            if ($("#test-1").is(":checked") && value == 0) {
                                return false;
                            }

                            return true;
                        }
                    }
                }
            },

            FileUpload: {
                validators: {
                    file: {
                        extension: 'pdf,mp4',
                        valid: false,
                        message: "<span>" + ErrorFileSelected + "</span>"
                    },
                    callback: {

                        callback: function (value, validator, $field) {
                            //alert($field.prop("accept"));
                            // Determine the numbers which are generated in captchaOperation
                            if (value.length == 0 && ($("#ID").val() == 0 || $("#IsNotAlreadyTrad").val().toLowerCase() == "true")){
                                return {
                                    valid: false,
                                    message: "<span>" + ErrorFileRequired + "</span>"
                                }

                            }

                            if ($("#uploadedFileSize").val() > maxFileUploadSize && $("#uploadedFileSize").val().indexOf('.pdf') >= 0) {
                                return {
                                    valid: false,
                                    message: "<span>" + ErrorFileTooBig + "</span>"
                                }

                            }
                            $field.removeClass('form-control-required');
                            return true;
                        }
                    },

                }
            },
            checkReadingMode: {
                container: "#readingRequiredErrorMessage",
                validators: {
                    callback: {
                        callback: function (value, validator, $field) {
                            // Determine the numbers which are generated in captchaOperation

                            if (!$("#chkReadDownload").is(":checked") && !$("#chkReadBrowser").is(":checked")) {
                                return {
                                    valid: false,
                                    message: "<span  class='ErrorReadingModeRequired'>" + ErrorReadingModeRequired + "</span>"
                                }

                            }
                            return true;
                        }
                    }

                }
            },
            checkType: {
                container: "#typeRequiredErrorMessage",
                validators: {
                    callback: {
                        callback: function (value, validator, $field) {
                            // Determine the numbers which are generated in captchaOperation

                            if (!$("#chkIsMetier").is(":checked") && !$("#chkIsStructure").is(":checked")) {
                                return {
                                    valid: false,
                                    message: "<span  class='ErrorTypeRequired'>" + ErrorTypeRequired + "</span>"
                                }

                            }
                            return true;
                        }
                    }

                }
            },
            checkDocPhase: {
                container: "#phaseRequiredErrorMessage",
                validators: {
                    callback: {

                        callback: function (value, validator, $field) {
                            // Determine the numbers which are generated in captchaOperation
                            if (!$("#chkPhaseEmployee").is(":checked") && !$("#chkPhaseOnboarding").is(":checked")) {
                                return {
                                    valid: false,
                                    message: "<span  class='ErrorDocumentPhaseRequired'>" + ErrorDocumentPhaseRequired + "</span>"
                                }

                            }

                            $("span.ErrorDocumentPhaseRequired").remove();
                            return true;
                        }
                    }

                }
            }
        }

    })
    .on('success.form.bv', function (e) {
        var typeAffect = $("#TypeAffectation").val();
        //console.log("typeAffect", typeAffect);
        if (typeAffect == "Profil"){
            $("#Affectation").val($("#ProfilAffectation").val());
        }
        else if (typeAffect.indexOf("Agence") > 0){
            $("#Affectation").val($("#EntityAffectation").val());
        }
        openLoading();
    });
});

function enableForm() {
    changeInputState('newDocumentForm', isInputDisabled);
    isInputDisabled = !isInputDisabled;
}

// Enable/Disable all inputs
function changeInputState(id, isInputDisabled) {
    if (!isInputDisabled) {
        $("#" + id + " :input,textarea").prop("disabled", true);
        $.each($("#" + id + " .date"), function () {
            $(this).prop("disabled", true);
        });
        $.each($("#" + id + " .btn.dropdown-toggle"), function () {
            $(this).addClass('disabled');
        });
        $.each($("#" + id + " .dropzone"), function () {
            $(this).addClass('disabled');
        });
        // editor.setReadOnly(true);
        $('.editor').each(function () {
            $(this).ckeditorGet().setReadOnly(true);
        });
        $.each($("#" + id + " .cke_mef"), function () {
            $(this).addClass('cke_top_hidden');
        });
        $.each($("#" + id + " .action"), function () {
            $(this).addClass('hide');
        });
        $.each($("#" + id + " #customer-action"), function () {
            $(this).addClass('hide');
        });
        $.each($('.help-block[data-bv-result="INVALID"]'), function () {
            $(this).css({ display: 'none' });
        });
        $.each($("#" + id + " #customerNameDiv"), function () {
            $(this).removeClass('col-6');
            $(this).addClass('col-9');
        });
    } else {
        $("#" + id + " :input,textarea").prop("disabled", false);
        $.each($("#" + id + " .date"), function () {
            $(this).prop("disabled", false);
        });
        $.each($("#" + id + " .btn.dropdown-toggle"), function () {
            $(this).removeClass('disabled');
        });
        // editor.setReadOnly(false);
        $('.editor').each(function () {
            $(this).ckeditorGet().setReadOnly(false);
        });
        $.each($("#" + id + " .cke_mef"), function () {
            $(this).removeClass('cke_top_hidden');
        });
        $.each($("#" + id + " .action"), function () {
            $(this).removeClass('hide');
        });
        $.each($("#" + id + " #customer-action"), function () {
            $(this).removeClass('hide');
        });
        $.each($("#" + id + " #customerNameDiv"), function () {
            $(this).removeClass('col-9');
            $(this).addClass('col-6');
        });

        $.each($('.help-block[data-bv-result="INVALID"]'), function () {
            if ($(this).css("display", "none")) $(this).css({ display: 'block' });
        });
    }
}

function populateForm() {
    $("input[name='documentName']").attr('value', 'Affichage Obligatoire INTITEK LYON');
    $("input[name='version']").attr('value', '01B');
    $("input[id='priorityMajor']").attr('checked', true);
    $("#catQuality").attr('selected', true);
}

function getNewDocumentVersion(id) {
    var newVersion = '';
    $.ajax({
        async: false,
        type: "POST",
        url: host + "Admin/Document/GetVersion",
        contentType: "application/json; charset=utf-8",
        data: '{"Id": ' + id + ',"IsMajorVersion":"' + $("#priorityMajor").is(":checked") + '"}',
        dataType: "json",
        processData: true,
        success: function (result) {

            newVersion = result.data;
        },

    });

    return newVersion;
}

function checkDocumentVersionLang(id, idLang, version) {
    var exist = false;
    $.ajax({
        async: false,
        type: "POST",
        url: host + "Admin/Document/ExistVersionTrad",
        contentType: "application/json; charset=utf-8",
        data: '{"Id": ' + id + ',"IdLang":"' + idLang + '","Version":"'+ version + '"}',
        dataType: "json",
        processData: true,
        success: function (result) {

            exist = result.success;
        },

    });

    return exist;
}